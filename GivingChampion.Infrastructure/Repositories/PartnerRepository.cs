using GivingChampion.Persistence.Contexts;
using GivingChampion.Common.Extensions.Pagination;
using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GivingChampion.Persistance.Repositories
{
    public class PartnerRepository : IPartnerRepository
    {

            #region Constructor

            // Constructor initializes the AppDbContext for the PartnerRepository
            private readonly AppDbContext _context;

            public PartnerRepository(AppDbContext context)
            {
                _context = context; // Initialize the context to interact with the database
            }

        #endregion


        /// <summary>
        /// Adds a new Partner to the database asynchronously.
        /// </summary>
        public async Task AddAsync(Partner partner)
        {
            // Adding the partner to the Partners DbSet
            await _context.Partners.AddAsync(partner);
        }


        /// <summary>
        /// Updates an existing Partner in the database.
        /// </summary>
        public void Update(Partner partner)
        {
            // Updating the partner in the Partners DbSet
            _context.Partners.Update(partner);
        }


        /// <summary>
        /// Soft deletes a Partner by marking it as deleted.
        /// </summary>
        public void SoftDelete(Partner partner)
        {
            // Marking the partner as deleted (soft delete)
            partner.IsDeleted = true;
            partner.DeletedAt = DateTime.UtcNow; // Set the current time for the deletion timestamp
            _context.Partners.Update(partner); // Updating the record after modification
        }


        /// <summary>
        /// Retrieves a Partner by its unique identifier (ID).
        /// It ensures that the partner is not deleted (soft deleted).
        /// </summary>
        public async Task<Partner?> GetByIdAsync(Guid id)
        {
            // Retrieving the partner using the ID, ensuring it's not deleted
            return await _context.Partners
                .Where(p => p.Id == id && !p.IsDeleted) // Ensure the partner is not soft-deleted
                .FirstOrDefaultAsync(); // Retrieve the first matching partner
        } 

        /// <summary>
        /// Retrieves all Partners from the database, excluding soft-deleted ones.
        /// </summary>
        public async Task<PagedList<Partner>> GetAllAsync(PageParameters pageParameters)
        {
            return await _context.Partners
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Id)
                .AsNoTracking()
                .ToPagedListAsync(pageParameters);
        }

    }
}

