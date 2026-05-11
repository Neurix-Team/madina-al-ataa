using GivingChampion.Common.Extensions.Pagination;
using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistence.Contexts;
using GivingChampion.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.Persistance.Repositories
{
    public class VolunteerRepository : IVolunteerRepository
    {
        private readonly AppDbContext _context;

        public VolunteerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<Volunteer>> GetAllAsync(PageParameters pageParameters)
        {
            return await _context.Volunteers
                .AsNoTracking()
                .Where(v => !v.IsDeleted)
                .OrderByDescending(v => v.CreatedAt)
                .ToPagedListAsync(pageParameters);
        }

        public async Task<Volunteer?> GetByIdAsync(Guid id)
        {
            return await _context.Volunteers
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);
        }

        public async Task<Volunteer?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Volunteers
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.UserId == userId && !v.IsDeleted);
        }

        public async Task AddAsync(Guid userId)
        {
            var volunteer = new Volunteer
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _context.Volunteers.AddAsync(volunteer);
        }

        public void Update(Volunteer volunteer)
        {
            _context.Volunteers.Update(volunteer);
        }
    }
}