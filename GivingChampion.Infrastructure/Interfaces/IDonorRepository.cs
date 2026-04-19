using GivingChampion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IDonorRepository
    {
        Task<Donor?> GetByIdAsync(Guid id);
        Task<Donor?> GetByUserIdAsync(Guid userId);
        Task<bool> ExistsByUserIdAsync(Guid userId);

        /// <summary>
        /// Creates a new Donor profile for a user
        /// </summary>
        Task CreateAsync(Donor donor);

        /// <summary>
        /// Updates donor profile (e.g. TotalDonated, PreferredCategory)
        /// </summary>
        Task UpdateAsync(Donor donor);

        /// <summary>
        /// Soft deletes the donor profile (recommended when user is deleted)
        /// </summary>
        Task SoftDeleteAsync(Guid userId);
    }
}
