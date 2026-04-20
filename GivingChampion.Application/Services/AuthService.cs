using GivingChampion.Common.DTO.Auth;
using GivingChampion.Persistance.Interfaces;
using global::GivingChampion.Application.Auth.Interfaces;
using global::GivingChampion.Application.Interfaces.Auth;
using global::GivingChampion.Common.Results;
using global::GivingChampion.Domain.Entities;

namespace GivingChampion.Application.Services
{
    public sealed class AuthService : IAuthService
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IJwtTokenFactory _jwtTokenFactory;
        private readonly IExternalLoginCodeStore _externalLoginCodeStore;
        private readonly IDonorRepository _donorRepository;

        public AuthService(
            IIdentityRepository identityRepository,
            IJwtTokenFactory jwtTokenFactory,
            IExternalLoginCodeStore externalLoginCodeStore,
            IDonorRepository donorRepository)
        {
            _identityRepository = identityRepository;
            _jwtTokenFactory = jwtTokenFactory;
            _externalLoginCodeStore = externalLoginCodeStore;
            _donorRepository = donorRepository;
        }

        public async Task<AuthServiceResult<TokenResponse>> RegisterAsync(
            RegisterRequest request,
            CancellationToken cancellationToken = default)
        {
            var existingUser = await _identityRepository.FindByEmailAsync(request.Email, cancellationToken);
            if (existingUser is not null)
            {
                return AuthServiceResult<TokenResponse>.Failure(
                    new ServiceError("DuplicateEmail", "A user with this email already exists."));
            }

            var createResult = await _identityRepository.CreateLocalUserAsync(
                request.Email,
                request.Password,
                request.Fullname,
                cancellationToken);

            if (!createResult.Succeeded || createResult.Data is null)
            {
                return AuthServiceResult<TokenResponse>.Failure(createResult.Errors);
            }

            var user = createResult.Data;

            var addRoleResult = await _identityRepository.AddToRoleAsync(user, "User", cancellationToken);
            if (!addRoleResult.Succeeded)
            {
                return AuthServiceResult<TokenResponse>.Failure(addRoleResult.Errors);
            }
            else
            {
                await _identityRepository.AddToRoleAsync(user, "Donor", cancellationToken);
                await _identityRepository.AddToRoleAsync(user, "Volunteer", cancellationToken);
            }

            var roles = await _identityRepository.GetRolesAsync(user, cancellationToken);
            var token = _jwtTokenFactory.Create(user, roles);

            return AuthServiceResult<TokenResponse>.Success(token);
        }

        public async Task<AuthServiceResult<TokenResponse>> LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken = default)
        {
            var user = await _identityRepository.FindByEmailAsync(request.Email, cancellationToken);
            if (user is null)
            {
                return AuthServiceResult<TokenResponse>.Failure(
                    new ServiceError("InvalidCredentials", "Invalid email or password."));
            }

            var passwordValid = await _identityRepository.CheckPasswordAsync(user, request.Password, cancellationToken);
            if (!passwordValid)
            {
                return AuthServiceResult<TokenResponse>.Failure(
                    new ServiceError("InvalidCredentials", "Invalid email or password."));
            }

            var roles = await _identityRepository.GetRolesAsync(user, cancellationToken);
            var token = _jwtTokenFactory.Create(user, roles);

            return AuthServiceResult<TokenResponse>.Success(token);
        }

        public async Task<AuthServiceResult<ExternalLoginCodeResponse>> CompleteGoogleLoginAsync(
            ExternalUserInfo externalUser,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(externalUser.ProviderKey))
            {
                return AuthServiceResult<ExternalLoginCodeResponse>.Failure(
                    new ServiceError("InvalidExternalLogin", "Provider key is missing."));
            }

            ApplicationUser? user = await _identityRepository.FindByExternalLoginAsync(
                externalUser.Provider,
                externalUser.ProviderKey,
                cancellationToken);

            var isNewUser = false;

            if (user is null)
            {
                if (string.IsNullOrWhiteSpace(externalUser.Email))
                {
                    return AuthServiceResult<ExternalLoginCodeResponse>.Failure(
                        new ServiceError("EmailNotProvided", "Google did not provide an email address."));
                }

                user = await _identityRepository.FindByEmailAsync(externalUser.Email, cancellationToken);

                if (user is null)
                {
                    var createUserResult = await _identityRepository.CreateExternalUserAsync(
                        externalUser.Email,
                        externalUser.FullName,
                        cancellationToken);

                    if (!createUserResult.Succeeded || createUserResult.Data is null)
                    {
                        return AuthServiceResult<ExternalLoginCodeResponse>.Failure(createUserResult.Errors);
                    }

                    var donor = new Donor
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        TotalDonated = 0,
                        PreferedCategory = 0,           // or default value
                        CreatedAt = DateTime.UtcNow
                    };

                    await _donorRepository.CreateAsync(donor);

                    user = createUserResult.Data;
                    isNewUser = true;
                }

                var hasLogin = await _identityRepository.HasExternalLoginAsync(
                    user,
                    externalUser.Provider,
                    externalUser.ProviderKey,
                    cancellationToken);

                if (!hasLogin)
                {
                    var addLoginResult = await _identityRepository.AddExternalLoginAsync(
                        user,
                        externalUser.Provider,
                        externalUser.ProviderKey,
                        externalUser.Provider,
                        cancellationToken);

                    if (!addLoginResult.Succeeded)
                    {
                        return AuthServiceResult<ExternalLoginCodeResponse>.Failure(addLoginResult.Errors);
                    }
                }
            }

            if (isNewUser)
            {
                var addRoleResult = await _identityRepository.AddToRoleAsync(user, "User", cancellationToken);
                if (!addRoleResult.Succeeded)
                {
                    return AuthServiceResult<ExternalLoginCodeResponse>.Failure(addRoleResult.Errors);
                }
                else
                {
                    await _identityRepository.AddToRoleAsync(user, "Donor", cancellationToken);
                    await _identityRepository.AddToRoleAsync(user, "Volunteer", cancellationToken);
                }
            }

            var roles = await _identityRepository.GetRolesAsync(user, cancellationToken);
            var token = _jwtTokenFactory.Create(user, roles);
            var code = _externalLoginCodeStore.Store(token);

            var hasPassword = await _identityRepository.HasPasswordAsync(user);

            return AuthServiceResult<ExternalLoginCodeResponse>.Success(new ExternalLoginCodeResponse { Code = code, NeedsRegistration = !hasPassword });
        }

        public async Task<AuthServiceResult<TokenResponse>> CompleteSocialRegistrationAsync(
            CompleteSocialRegistrationRequest request,
            CancellationToken cancellationToken = default)
        {
            var user = await _identityRepository.FindByIdAsync(request.Userid, cancellationToken);

            if (user is null)
            {
                return AuthServiceResult<TokenResponse>.Failure(
                    new ServiceError("UserNotFound", "User was not found."));
            }

            if (!user.IsExternal)
            {
                return AuthServiceResult<TokenResponse>.Failure(
                    new ServiceError("InvalidUserType", "This action is only allowed for external users."));
            }

            var hasPassword = await _identityRepository.HasPasswordAsync(user, cancellationToken);

            if (hasPassword)
            {
                return AuthServiceResult<TokenResponse>.Failure(
                    new ServiceError("PasswordAlreadyExists", "This user already has a local password."));
            }

            var addPasswordResult = await _identityRepository.AddPasswordAsync(
                user,
                request.Newpassword,
                cancellationToken);

            if (!addPasswordResult.Succeeded)
            {
                return AuthServiceResult<TokenResponse>.Failure(addPasswordResult.Errors);
            }

            // Keep IsExternal = true if you want to preserve the source of registration.
            // If you want this flag to mean "still needs completion", set it to false here.
            //user.IsExternal = false;

            var updateResult = await _identityRepository.UpdateAsync(user, cancellationToken);

            if (!updateResult.Succeeded)
            {
                return AuthServiceResult<TokenResponse>.Failure(updateResult.Errors);
            }

            var roles = await _identityRepository.GetRolesAsync(user, cancellationToken);
            var token = _jwtTokenFactory.Create(user, roles);

            return AuthServiceResult<TokenResponse>.Success(token);
        }

        public Task<AuthServiceResult<TokenResponse>> ExchangeExternalCodeAsync(
            string code,
            CancellationToken cancellationToken = default)
        {
            var token = _externalLoginCodeStore.Take(code);

            if (token is null)
            {
                return Task.FromResult(
                    AuthServiceResult<TokenResponse>.Failure(
                        new ServiceError("InvalidCode", "Invalid or expired code.")));
            }

            return Task.FromResult(AuthServiceResult<TokenResponse>.Success(token));
        }
    }
}
