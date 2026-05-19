using GivingChampion.Application.DTO.Auth;
using GivingChampion.Domain.Enums;
using GivingChampion.Persistance.Interfaces;
using global::GivingChampion.Application.Auth.Interfaces;
using global::GivingChampion.Application.Interfaces.Auth;
using global::GivingChampion.Common.Results;
using global::GivingChampion.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace GivingChampion.Application.Services
{
    public sealed class AuthService : BaseService, IAuthService
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IJwtTokenFactory _jwtTokenFactory;
        private readonly IExternalLoginCodeStore _externalLoginCodeStore;
        private readonly IDonorRepository _donorRepository;
        private readonly IVolunteerRepository _volunteerRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IAvatarRepository _avatarRepository;
        private readonly IChildRepository _childRepository;

        public AuthService(
            IIdentityRepository identityRepository,
            IJwtTokenFactory jwtTokenFactory,
            IExternalLoginCodeStore externalLoginCodeStore,
            IDonorRepository donorRepository,
            IVolunteerRepository volunteerRepository,
            IProfileRepository profileRepository,
            IAvatarRepository avatarRepository,
            IChildRepository childRepository,
            IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _identityRepository = identityRepository;
            _jwtTokenFactory = jwtTokenFactory;
            _externalLoginCodeStore = externalLoginCodeStore;
            _donorRepository = donorRepository;
            _volunteerRepository = volunteerRepository;
            _profileRepository = profileRepository;
            _avatarRepository = avatarRepository;
            _childRepository = childRepository;
        }

        public async Task<AuthServiceResult> RegisterAsync(
            RegisterRequest request,
            CancellationToken cancellationToken = default)
        {
            var existingUser = await _identityRepository.FindByEmailAsync(request.Email, cancellationToken);

            if (existingUser is not null)
            {
                // 2. Check if the user is soft-deleted
                if (existingUser.IsDeleted)
                {
                    return AuthServiceResult.Failure(
                        new ServiceError("AccountDisabled", "This account has been deactivated. Please contact support to reactivate your account."));
                }

                return AuthServiceResult.Failure(
                    new ServiceError("DuplicateEmail", "A user with this email already exists."));
            }

            if (request.Password != request.ConfirmPassword)
            {
                return AuthServiceResult.Failure(
                    new ServiceError("PasswordMismatch", "The password and confirm password do not match."));
            }

            var createResult = await _identityRepository.CreateLocalUserAsync(
                request.Email,
                request.Password,
                request.Fullname,
                request.BirthDate,
                cancellationToken);

            if (!createResult.Succeeded || createResult.Data is null)
            {
                return AuthServiceResult.Failure(createResult.Errors);
            }

            var user = createResult.Data;

            var addRoleResult = await _identityRepository.AddToRoleAsync(user, "User", cancellationToken);
            if (!addRoleResult.Succeeded)
            {
                return AuthServiceResult.Failure(addRoleResult.Errors);
            }
            else
            {
                await _identityRepository.AddToRoleAsync(user, "Donor", cancellationToken);
                await _identityRepository.AddToRoleAsync(user, "Volunteer", cancellationToken);
            }

            return AuthServiceResult.Success();
        }

        public async Task<AuthServiceResult<TokenResponse>> LoginAsync(
    LoginRequest request,
    CancellationToken cancellationToken = default)
        {
            var user = await _identityRepository.FindByEmailCaseSensitiveAsync(request.Email, cancellationToken);

            // 1. Check if user exists
            if (user is null)
            {
                return AuthServiceResult<TokenResponse>.Failure(
                    new ServiceError("InvalidCredentials", "Invalid email or password."));
            }

            // 2. Check if the account is soft-deleted/disabled
            if (user.IsDeleted) // or user.IsDisabled, depending on your property name
            {
                return AuthServiceResult<TokenResponse>.Failure(
                    new ServiceError("AccountDisabled", "Your account has been deactivated. Please contact support."));
            }

            // 3. Validate password
            var passwordValid = await _identityRepository.CheckPasswordAsync(user, request.Password, cancellationToken);
            if (!passwordValid)
            {
                return AuthServiceResult<TokenResponse>.Failure(
                    new ServiceError("InvalidCredentials", "Invalid email or password."));
            }

            var roles = await _identityRepository.GetRolesAsync(user, cancellationToken);

            if (roles.Contains("Child"))
            {
                var child = await _childRepository.GetByUserIdAsync(user.Id);

                if (child == null || child.Status != ObjectStatus.Approved)
                {
                    return AuthServiceResult<TokenResponse>.Failure(
                        new ServiceError("ChildNotApproved", "Child account is pending approval."));
                }
            }

            // 4. Generate Token
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

                    user = createUserResult.Data;
                    isNewUser = true;

                    //await _donorRepository.CreateAsync(user.Id);
                    //await _volunteerRepository.AddAsync(user.Id);
                    //await _profileRepository.AddAsync(user.Id);
                    //await _avatarRepository.AddAsync(user.Id);

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

            user.BirthDay = DateTime.SpecifyKind(request.BirthDate.Date, DateTimeKind.Utc);

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

        public async Task<CurrentUserDto> GetCurrentUserAsync(CancellationToken cancellationToken = default)
        {
            var user = await _identityRepository.FindByIdAsync(UserId.ToString(), cancellationToken);

            if (user is null)
                throw new InvalidOperationException("Current user was not found.");

            var roles = await _identityRepository.GetRolesAsync(user, cancellationToken);

            return new CurrentUserDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.FullName,
                BirthDay = user.BirthDay,
                Roles = roles.ToArray()
            };
        }
    }
}
