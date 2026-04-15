using GivingChampion.Application.Auth.Interfaces;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace GivingChampion.Persistance.Repositories
{
    public sealed class IdentityRepository : IIdentityRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public IdentityRepository(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public Task<ApplicationUser?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
            => _userManager.FindByEmailAsync(email);

        public Task<ApplicationUser?> FindByExternalLoginAsync(
            string provider,
            string providerKey,
            CancellationToken cancellationToken = default)
            => _userManager.FindByLoginAsync(provider, providerKey);

        public async Task<ServiceResult<ApplicationUser>> CreateLocalUserAsync(
            string email,
            string password,
            string? fullName,
            CancellationToken cancellationToken = default)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName
            };

            var result = await _userManager.CreateAsync(user, password);

            return result.Succeeded
                ? ServiceResult<ApplicationUser>.Success(user)
                : ServiceResult<ApplicationUser>.Failure(
                    result.Errors.Select(x => new ServiceError(x.Code, x.Description)));
        }

        public async Task<ServiceResult<ApplicationUser>> CreateExternalUserAsync(
            string email,
            string? fullName,
            CancellationToken cancellationToken = default)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user);

            return result.Succeeded
                ? ServiceResult<ApplicationUser>.Success(user)
                : ServiceResult<ApplicationUser>.Failure(
                    result.Errors.Select(x => new ServiceError(x.Code, x.Description)));
        }

        public Task<bool> CheckPasswordAsync(
            ApplicationUser user,
            string password,
            CancellationToken cancellationToken = default)
            => _userManager.CheckPasswordAsync(user, password);

        public async Task<ServiceResult> AddToRoleAsync(
            ApplicationUser user,
            string role,
            CancellationToken cancellationToken = default)
        {
            var result = await _userManager.AddToRoleAsync(user, role);

            return result.Succeeded
                ? ServiceResult.Success()
                : ServiceResult.Failure(
                    result.Errors.Select(x => new ServiceError(x.Code, x.Description)));
        }

        public async Task<IList<string>> GetRolesAsync(
            ApplicationUser user,
            CancellationToken cancellationToken = default)
            => await _userManager.GetRolesAsync(user);

        public async Task<bool> HasExternalLoginAsync(
            ApplicationUser user,
            string provider,
            string providerKey,
            CancellationToken cancellationToken = default)
        {
            var logins = await _userManager.GetLoginsAsync(user);
            return logins.Any(x => x.LoginProvider == provider && x.ProviderKey == providerKey);
        }

        public async Task<ServiceResult> AddExternalLoginAsync(
            ApplicationUser user,
            string provider,
            string providerKey,
            string providerDisplayName,
            CancellationToken cancellationToken = default)
        {
            var login = new UserLoginInfo(provider, providerKey, providerDisplayName);
            var result = await _userManager.AddLoginAsync(user, login);

            return result.Succeeded
                ? ServiceResult.Success()
                : ServiceResult.Failure(
                    result.Errors.Select(x => new ServiceError(x.Code, x.Description)));
        }

        public async Task EnsureRolesExistAsync(
            IEnumerable<string> roles,
            CancellationToken cancellationToken = default)
        {
            foreach (var role in roles.Distinct())
            {
                if (await _roleManager.RoleExistsAsync(role))
                {
                    continue;
                }

                var result = await _roleManager.CreateAsync(new ApplicationRole(role));
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                    throw new InvalidOperationException($"Failed to create role '{role}': {errors}");
                }
            }
        }

        public Task<ApplicationUser?> FindByIdAsync(
            string userId,
            CancellationToken cancellationToken = default)
            => _userManager.FindByIdAsync(userId);

        public Task<bool> HasPasswordAsync(
            ApplicationUser user,
            CancellationToken cancellationToken = default)
            => _userManager.HasPasswordAsync(user);

        public async Task<ServiceResult> AddPasswordAsync(
            ApplicationUser user,
            string newPassword,
            CancellationToken cancellationToken = default)
        {
            var result = await _userManager.AddPasswordAsync(user, newPassword);

            return result.Succeeded
                ? ServiceResult.Success()
                : ServiceResult.Failure(
                    result.Errors.Select(x => new ServiceError(x.Code, x.Description)));
        }

        public async Task<ServiceResult> UpdateAsync(
            ApplicationUser user,
            CancellationToken cancellationToken = default)
        {
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded
                ? ServiceResult.Success()
                : ServiceResult.Failure(
                    result.Errors.Select(x => new ServiceError(x.Code, x.Description)));
        }
    }
}
