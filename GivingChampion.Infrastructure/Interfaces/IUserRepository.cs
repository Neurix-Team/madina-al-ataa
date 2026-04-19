using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(Guid id);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<PagedList<ApplicationUser>> GetAllAsync(PageParameters pageParameters, string? search = null);
        Task<bool> ExistsByEmailAsync(string email);
        // Add more methods later if needed (e.g., soft delete, custom queries)
    }
}