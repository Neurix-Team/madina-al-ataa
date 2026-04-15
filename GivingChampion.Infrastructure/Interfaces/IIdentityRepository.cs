using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;

namespace GivingChampion.Application.Auth.Interfaces;

public interface IIdentityRepository
{
    Task<ApplicationUser?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> FindByExternalLoginAsync(string provider, string providerKey, CancellationToken cancellationToken = default);

    Task<ServiceResult<ApplicationUser>> CreateLocalUserAsync(
        string email,
        string password,
        string? fullName,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<ApplicationUser>> CreateExternalUserAsync(
        string email,
        string? fullName,
        CancellationToken cancellationToken = default);

    Task<bool> CheckPasswordAsync(
        ApplicationUser user,
        string password,
        CancellationToken cancellationToken = default);

    Task<ServiceResult> AddToRoleAsync(
        ApplicationUser user,
        string role,
        CancellationToken cancellationToken = default);

    Task<IList<string>> GetRolesAsync(
        ApplicationUser user,
        CancellationToken cancellationToken = default);

    Task<bool> HasExternalLoginAsync(
        ApplicationUser user,
        string provider,
        string providerKey,
        CancellationToken cancellationToken = default);

    Task<ServiceResult> AddExternalLoginAsync(
        ApplicationUser user,
        string provider,
        string providerKey,
        string providerDisplayName,
        CancellationToken cancellationToken = default);

    Task EnsureRolesExistAsync(
        IEnumerable<string> roles,
        CancellationToken cancellationToken = default);

    Task<ApplicationUser?> FindByIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<bool> HasPasswordAsync(
        ApplicationUser user,
        CancellationToken cancellationToken = default);

    Task<ServiceResult> AddPasswordAsync(
        ApplicationUser user,
        string newPassword,
        CancellationToken cancellationToken = default);

    Task<ServiceResult> UpdateAsync(
        ApplicationUser user,
        CancellationToken cancellationToken = default);
}