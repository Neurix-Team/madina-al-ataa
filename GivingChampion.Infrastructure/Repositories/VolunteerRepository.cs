using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using GivingChampion.Persistence.Contexts;
using GivingChampion.Common.Extensions.Pagination;
using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace GivingChampion.Persistance.Repositories
{
    public class VolunteerRepository : IVolunteerRepository
    {


        private readonly AppDbContext _context;


        /// <summary>
        /// Initializes a new instance of the VolunteerRepository class.
        /// </summary>
        /// <param name="context">The database context to interact with the database.</param>
        public VolunteerRepository(AppDbContext context)
        {
            _context = context; // Initialize the context to interact with the database
        }




        public async Task<PagedList<Volunteer>> GetAllAsync(PageParameters pageParameters)
        {
            return await _context.Volunteers
                .Where(v => !v.IsDeleted)
                .AsNoTracking()
                .ToPagedListAsync(pageParameters);
        }

        #endregion

        /// <summary>
        /// Retrieves a volunteer by its unique identifier (ID), ensuring it is not soft-deleted.
        /// </summary>
        /// <param name="id">The unique identifier of the volunteer.</param>
        /// <returns>A volunteer or null if not found or deleted.</returns>
        public async Task<Volunteer?> GetByIdAsync(Guid id)
        {
            return await _context.Volunteers
                .Where(v => v.Id == id && !v.IsDeleted) // Ensure the volunteer is not soft-deleted
                .AsNoTracking() // Use AsNoTracking for better performance
                .FirstOrDefaultAsync()
                .ConfigureAwait(false); // Avoid blocking UI thread in production
        }

        public async Task<Volunteer?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Volunteers
                .Where(v => v.UserId == userId && !v.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
        }



        /// <summary>
        /// Adds a new volunteer to the database asynchronously.
        /// </summary>
        /// <param name="volunteer">The volunteer to be added.</param>
        public async Task AddAsync(Guid UserId)
        {
            var volunteer = new Volunteer()
        {
                UserId = UserId,
            };
            await _context.Volunteers.AddAsync(volunteer).ConfigureAwait(false); // Add the volunteer to the database
        } 

        ///// <summary>
        ///// Updates an existing volunteer in the database.
        ///// </summary>
        ///// <param name="volunteer">The volunteer entity to be updated.</param>
        //public void Update(Volunteer volunteer)
        //{
        //    _context.Volunteers.Update(volunteer); // Update the volunteer in the database
        //} 

    }
}
