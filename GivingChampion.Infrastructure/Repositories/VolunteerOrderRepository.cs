using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using System;using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Persistance.Repositories
{
    public class VolunteerOrderRepository : IVolunteerOrderRepository
    {
        /// <summary>
        /// Repository implementation for managing volunteer orders.
        /// Handles data access operations and supports soft deletion.
        /// </summary>
          #region Fields

            private readonly AppDbContext _context;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="VolunteerOrderRepository"/> class.
            /// </summary>
            /// <param name="context">Application database context.</param>
            public VolunteerOrderRepository(AppDbContext context)
            {
                _context = context;
            }

            #endregion

            #region Query Methods

            #region Get All

            /// <summary>
            /// Retrieves all volunteer orders that are not soft deleted.
            /// </summary>
            /// <returns>A list of active volunteer orders.</returns>
            public async Task<List<VolunteerOrder>> GetAllAsync()
            {
                return await _context.VolunteerOrders
                    .Where(vo => !vo.IsDeleted)
                    .AsNoTracking()
                    .ToListAsync();
            }

            #endregion

            #region Get By Id

            /// <summary>
            /// Retrieves a single volunteer order by its unique identifier.
            /// Returns null if the order does not exist or is soft deleted.
            /// </summary>
            /// <param name="id">The unique identifier of the volunteer order.</param>
            /// <returns>The matching volunteer order if found; otherwise, null.</returns>
            public async Task<VolunteerOrder?> GetByIdAsync(Guid id)
            {
                return await _context.VolunteerOrders
                    .FirstOrDefaultAsync(vo => vo.Id == id && !vo.IsDeleted);
            }

            #endregion

            #region Exists

            /// <summary>
            /// Checks whether a volunteer order exists and is not soft deleted.
            /// </summary>
            /// <param name="id">The unique identifier of the volunteer order.</param>
            /// <returns>True if the volunteer order exists and is active; otherwise, false.</returns>
            public async Task<bool> ExistsAsync(Guid id)
            {
                return await _context.VolunteerOrders
                    .AnyAsync(vo => vo.Id == id && !vo.IsDeleted);
            }

            #endregion

            #endregion

            #region Command Methods

            #region Add

            /// <summary>
            /// Adds a new volunteer order to the DbContext.
            /// Note: This does not save changes to the database until SaveChangesAsync is called.
            /// </summary>
            /// <param name="volunteerOrder">The volunteer order entity to add.</param>
            public async Task AddAsync(VolunteerOrder volunteerOrder)
            {
                await _context.VolunteerOrders.AddAsync(volunteerOrder);
            }

            #endregion

            #region Update

            /// <summary>
            /// Marks an existing volunteer order as modified.
            /// Note: This does not save changes to the database until SaveChangesAsync is called.
            /// </summary>
            /// <param name="volunteerOrder">The volunteer order entity to update.</param>
            public void Update(VolunteerOrder volunteerOrder)
            {
                _context.VolunteerOrders.Update(volunteerOrder);
            }

            #endregion

            #region Soft Delete

            /// <summary>
            /// Soft deletes a volunteer order instead of physically removing it from the database.
            /// Usually sets IsDeleted to true and DeletedAt to the current UTC date and time.
            /// Note: This does not save changes to the database until SaveChangesAsync is called.
            /// </summary>
            /// <param name="volunteerOrder">The volunteer order entity to soft delete.</param>
            public void SoftDelete(VolunteerOrder volunteerOrder)
            {
                volunteerOrder.IsDeleted = true;
                volunteerOrder.DeletedAt = DateTime.UtcNow;

                _context.VolunteerOrders.Update(volunteerOrder);
            }

            #endregion

            #region Save Changes

            /// <summary>
            /// Saves all pending changes to the database.
            /// </summary>
            public async Task SaveChangesAsync()
            {
                await _context.SaveChangesAsync();
            }

            #endregion

            #endregion
        }
    }

