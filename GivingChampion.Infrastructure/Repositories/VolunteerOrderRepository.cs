using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using System;
using Microsoft.EntityFrameworkCore;

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
                   .Include(vo => vo.ServiceRequest)
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

        public async Task<VolunteerOrder> CreateAsync(VolunteerOrder volunteerOrder)
        {
            _context.VolunteerOrders.Add(volunteerOrder);
            await _context.SaveChangesAsync();
            return volunteerOrder;
        }


        #endregion

        public Task UpdateAsync(VolunteerOrder volunteerOrder)
        {
            _context.VolunteerOrders.Update(volunteerOrder);
            return Task.CompletedTask;
        }

        public Task SoftDeleteAsync(VolunteerOrder volunteerOrder)
        {
            volunteerOrder.IsDeleted = true;
            volunteerOrder.DeletedAt = DateTime.UtcNow;

            _context.VolunteerOrders.Update(volunteerOrder);

            return Task.CompletedTask;
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

            
        }
    }

