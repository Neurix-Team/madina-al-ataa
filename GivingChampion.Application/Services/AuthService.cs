using global::GivingChampion.Application.Auth.Interfaces;
using global::GivingChampion.Application.Interfaces.Auth;
using global::GivingChampion.Common.Auth;
using global::GivingChampion.Common.Results;
using global::GivingChampion.Domain.Entities;

namespace GivingChampion.Application.Services
{
    public sealed class AuthService : IAuthService
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IJwtTokenFactory _jwtTokenFactory;
        private readonly IExternalLoginCodeStore _externalLoginCodeStore;

        public AuthService(
            IIdentityRepository identityRepository,
            IJwtTokenFactory jwtTokenFactory,
            IExternalLoginCodeStore externalLoginCodeStore)
        {
            _identityRepository = identityRepository;
            _jwtTokenFactory = jwtTokenFactory;
            _externalLoginCodeStore = externalLoginCodeStore;
        }

        public async Task<ServiceResult<TokenResponse>> RegisterAsync(
            RegisterRequest request,
            CancellationToken cancellationToken = default)
        {
            var existingUser = await _identityRepository.FindByEmailAsync(request.Email, cancellationToken);
            if (existingUser is not null)
            {
                return ServiceResult<TokenResponse>.Failure(
                    new ServiceError("DuplicateEmail", "A user with this email already exists."));
            }

            var createResult = await _identityRepository.CreateLocalUserAsync(
                request.Email,
                request.Password,
                request.FullName,
                cancellationToken);

            if (!createResult.Succeeded || createResult.Data is null)
            {
                return ServiceResult<TokenResponse>.Failure(createResult.Errors);
            }

            var user = createResult.Data;

            var addRoleResult = await _identityRepository.AddToRoleAsync(user, "Parent", cancellationToken);
            if (!addRoleResult.Succeeded)
            {
                return ServiceResult<TokenResponse>.Failure(addRoleResult.Errors);
            }
            else
            {
                await _identityRepository.AddToRoleAsync(user, "Donor", cancellationToken);
                await _identityRepository.AddToRoleAsync(user, "Volunteer", cancellationToken);
            }

            var roles = await _identityRepository.GetRolesAsync(user, cancellationToken);
            var token = _jwtTokenFactory.Create(user, roles);

            return ServiceResult<TokenResponse>.Success(token);
        }

        public async Task<ServiceResult<TokenResponse>> LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken = default)
        {
            var user = await _identityRepository.FindByEmailAsync(request.Email, cancellationToken);
            if (user is null)
            {
                return ServiceResult<TokenResponse>.Failure(
                    new ServiceError("InvalidCredentials", "Invalid email or password."));
            }

            var passwordValid = await _identityRepository.CheckPasswordAsync(user, request.Password, cancellationToken);
            if (!passwordValid)
            {
                return ServiceResult<TokenResponse>.Failure(
                    new ServiceError("InvalidCredentials", "Invalid email or password."));
            }

            var roles = await _identityRepository.GetRolesAsync(user, cancellationToken);
            var token = _jwtTokenFactory.Create(user, roles);

            return ServiceResult<TokenResponse>.Success(token);
        }

        public async Task<ServiceResult<ExternalLoginCodeResponse>> CompleteGoogleLoginAsync(
            ExternalUserInfo externalUser,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(externalUser.ProviderKey))
            {
                return ServiceResult<ExternalLoginCodeResponse>.Failure(
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
                    return ServiceResult<ExternalLoginCodeResponse>.Failure(
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
                        return ServiceResult<ExternalLoginCodeResponse>.Failure(createUserResult.Errors);
                    }

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
                        return ServiceResult<ExternalLoginCodeResponse>.Failure(addLoginResult.Errors);
                    }
                }
            }

            if (isNewUser)
            {
                var addRoleResult = await _identityRepository.AddToRoleAsync(user, "User", cancellationToken);
                if (!addRoleResult.Succeeded)
                {
                    return ServiceResult<ExternalLoginCodeResponse>.Failure(addRoleResult.Errors);
                }
            }

            var roles = await _identityRepository.GetRolesAsync(user, cancellationToken);
            var token = _jwtTokenFactory.Create(user, roles);
            var code = _externalLoginCodeStore.Store(token);

            return ServiceResult<ExternalLoginCodeResponse>.Success(new ExternalLoginCodeResponse(code));
        }

        public Task<ServiceResult<TokenResponse>> ExchangeExternalCodeAsync(
            string code,
            CancellationToken cancellationToken = default)
        {
            var token = _externalLoginCodeStore.Take(code);

            if (token is null)
            {
                return Task.FromResult(
                    ServiceResult<TokenResponse>.Failure(
                        new ServiceError("InvalidCode", "Invalid or expired code.")));
            }

            return Task.FromResult(ServiceResult<TokenResponse>.Success(token));
        }
    }
}
