using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IPartnerRepository
    {
        // Add a new partner to the database
        Task AddAsync(Partner partner);

        // Update an existing partner in the database
        void Update(Partner partner);

        // Delete a partner by soft deletion
        void SoftDelete(Partner partner);

        // Get a partner by its ID
        Task<Partner> GetByIdAsync(Guid id);

        // Get all partners
        Task<PagedList<Partner>> GetAllAsync(PageParameters pageParameters);

        // Save changes to the database
    }
}

