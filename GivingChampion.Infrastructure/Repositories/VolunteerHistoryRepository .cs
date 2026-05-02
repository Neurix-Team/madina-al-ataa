using GivingChampion.Persistence.Contexts;
using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.Persistance.Repositories
{
    public class VolunteerHistoryRepository : IVolunteerHistoryRepository
    {
        private readonly AppDbContext _context;

        public VolunteerHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(VolunteerHistories history)
        {
            await _context.VolunteerHistories.AddAsync(history);
        }

        public async Task<PagedList<VolunteerHistories>> GetByUserIdAsync(
            Guid userId,
            PageParameters pageParameters)
        {
            var query = _context.VolunteerHistories
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt);

            return await PagedList<VolunteerHistories>.CreateAsync(
                query,
                pageParameters.PageNumber,
                pageParameters.PageSize);
        }

        public async Task<PagedList<VolunteerHistories>> GetByRequestIdAsync(
            Guid requestId,
            PageParameters pageParameters)
        {
            var query = _context.VolunteerHistories
                .AsNoTracking()
                .Where(x => x.ServiceRequestId == requestId)
                .OrderByDescending(x => x.CreatedAt);

            return await PagedList<VolunteerHistories>.CreateAsync(
                query,
                pageParameters.PageNumber,
                pageParameters.PageSize);
        }

    }
}
