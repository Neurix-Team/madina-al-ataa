using GivingChampion.Common.Enums;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Persistance.Repositories
{
   
        public class DonationRequestRepository : IDonationRequestRepository
    { 
    
        
            private readonly AppDbContext _context;

            // Constructor to inject the ApplicationDbContext (database context)
            public DonationRequestRepository(AppDbContext context)
            {
                _context = context;
            }

            // Get a single DonationRequest by its ID
            public async Task<DonationRequest?> GetByIdAsync(Guid id)
            {
                return await _context.DonationRequests
                    .AsNoTracking()  // No tracking as we are only reading data
                    .FirstOrDefaultAsync(dr => dr.Id == id);
            }

            // Get all DonationRequests (without tracking changes in memory)
            public async Task<IEnumerable<DonationRequest>> GetAllAsync()
            {
                return await _context.DonationRequests
                    .AsNoTracking()
                    .ToListAsync();
            }

            // Add a new DonationRequest
            public async Task AddAsync(DonationRequest donationRequest)
            {
                await _context.DonationRequests.AddAsync(donationRequest);  // Add the entity to the DbContext
                await _context.SaveChangesAsync();  // Save the changes to the database
            }

            // Update an existing DonationRequest
            public async Task UpdateAsync(DonationRequest donationRequest)
            {
                // Attach the entity to the DbContext to track it
                _context.DonationRequests.Update(donationRequest);
                await _context.SaveChangesAsync();  // Save changes to the database
            }
        }
    }
