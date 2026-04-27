using GivingChampion.Application.Auth.Interfaces;
using GivingChampion.Common.DTO.Donor;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace GivingChampion.Persistance.Repositories
{
    public sealed class IdentityRepository : IIdentityRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IDonorRepository _donorRepository;

        public IdentityRepository(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IDonorRepository donorRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _donorRepository = donorRepository;
        }

        public Task<ApplicationUser?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
            => _userManager.FindByEmailAsync(email);

        public Task<ApplicationUser?> FindByExternalLoginAsync(
            string provider,
            string providerKey,
            CancellationToken cancellationToken = default)
            => _userManager.FindByLoginAsync(provider, providerKey);

        public async Task<AuthServiceResult<ApplicationUser>> CreateLocalUserAsync(
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

            if (result.Succeeded)
            {
                Donor donorDto = new()
                {
                    UserId = user.Id,
                };

                await _donorRepository.CreateAsync(donorDto);
            }
            

            return result.Succeeded
                ? AuthServiceResult<ApplicationUser>.Success(user)
                : AuthServiceResult<ApplicationUser>.Failure(
                    result.Errors.Select(x => new ServiceError(x.Code, x.Description)));
        }

        public async Task<AuthServiceResult<ApplicationUser>> CreateExternalUserAsync(
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
                ? AuthServiceResult<ApplicationUser>.Success(user)
                : AuthServiceResult<ApplicationUser>.Failure(
                    result.Errors.Select(x => new ServiceError(x.Code, x.Description)));
        }

        public Task<bool> CheckPasswordAsync(
            ApplicationUser user,
            string password,
            CancellationToken cancellationToken = default)
            => _userManager.CheckPasswordAsync(user, password);

        public async Task<AuthServiceResult> AddToRoleAsync(
            ApplicationUser user,
            string role,
            CancellationToken cancellationToken = default)
        {
            var result = await _userManager.AddToRoleAsync(user, role);

            return result.Succeeded
                ? AuthServiceResult.Success()
                : AuthServiceResult.Failure(
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

        public async Task<AuthServiceResult> AddExternalLoginAsync(
            ApplicationUser user,
            string provider,
            string providerKey,
            string providerDisplayName,
            CancellationToken cancellationToken = default)
        {
            var login = new UserLoginInfo(provider, providerKey, providerDisplayName);
            var result = await _userManager.AddLoginAsync(user, login);

            return result.Succeeded
                ? AuthServiceResult.Success()
                : AuthServiceResult.Failure(
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

        public async Task<AuthServiceResult> AddPasswordAsync(
            ApplicationUser user,
            string newPassword,
            CancellationToken cancellationToken = default)
        {
            var result = await _userManager.AddPasswordAsync(user, newPassword);

            return result.Succeeded
                ? AuthServiceResult.Success()
                : AuthServiceResult.Failure(
                    result.Errors.Select(x => new ServiceError(x.Code, x.Description)));
        }

        public async Task<AuthServiceResult> UpdateAsync(
            ApplicationUser user,
            CancellationToken cancellationToken = default)
        {
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded
                ? AuthServiceResult.Success()
                : AuthServiceResult.Failure(
                    result.Errors.Select(x => new ServiceError(x.Code, x.Description)));
        }
    }
}
