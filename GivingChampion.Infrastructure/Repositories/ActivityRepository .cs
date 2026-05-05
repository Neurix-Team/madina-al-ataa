using GivingChampion.Persistence.Contexts;
using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.Persistance.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly AppDbContext _context;

        public ActivityRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Activity history)
        {
            await _context.Activities.AddAsync(history);
        }

        public async Task<PagedList<Activity>> GetByEntityIdAsync(
            Guid entityId,
            PageParameters pageParameters)
        {
            var query = _context.Activities
                .AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.EntityId == entityId)
                .OrderByDescending(x => x.CreatedAt);

            return await PagedList<Activity>.CreateAsync(
                query,
                pageParameters.PageNumber,
                pageParameters.PageSize);
        }

        //public async Task<PagedList<VolunteerHistories>> GetByRequestIdAsync(
        //    Guid requestId,
        //    PageParameters pageParameters)
        //{
        //    var query = _context.VolunteerHistories
        //        .AsNoTracking()
        //        .Where(x => x.ServiceRequestId == requestId)
        //        .OrderByDescending(x => x.CreatedAt);

        //    return await PagedList<VolunteerHistories>.CreateAsync(
        //        query,
        //        pageParameters.PageNumber,
        //        pageParameters.PageSize);
        //}

    }
}
