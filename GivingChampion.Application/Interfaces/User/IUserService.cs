using GivingChampion.Common.DTO.User;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GivingChampion.Application.Interfaces.User
{
    public interface IUserService
    {
        // Read operations (available to users + admins)
        Task<PagedList<GetUserDto>> GetAllUsersAsync(PageParameters pageParameters, string? search = null);
        Task<GetUserDto?> GetUserByIdAsync(Guid id);
        Task<GetUserDto?> GetUserByEmailAsync(string email);

        // Self-service operations (usually for current user)
        Task<Result<GetUserDto>> CreateUserAsync(CreateUserDto dto);

        Task<Result<bool>> DeleteMyAccountAsync(Guid userId, string? reason = null);

        // Admin-only operations (protected by authorization policy)
        Task<Result<GetUserDto>> CreateUserByAdminAsync(CreateUserDto dto);
        Task<Result<GetUserDto>> CreateAdminUserAsync(CreateUserDto dto);         // dedicated for elevated creation
        Task<Result<bool>> UpdateUserByAdminAsync(Guid id, UpdateUser dto);
        Task<Result<bool>> DeleteUserByAdminAsync(Guid id, string? reason = null);

        // Role management (often separate, but sometimes included)
        Task<Result<bool>> AssignRolesAsync(Guid userId, IEnumerable<string> roles);
        Task<Result<bool>> RemoveRolesAsync(Guid userId, IEnumerable<string> roles);
    }
}
