using GivingChampion.Persistence.Contexts;
using GivingChampion.Common.Extensions.Pagination;
using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;
using GivingChampion.Domain.Enums;
using GivingChampion.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.Persistance.Repositories
{
    public class VolunteerOrderRepository : IVolunteerOrderRepository
    {
        private readonly AppDbContext _context;


        public VolunteerOrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<VolunteerOrder>> GetAllAsync(PageParameters pageParameters)
        {
            return await _context.VolunteerOrders
                .Include(vo => vo.ServiceRequest)
                .Where(vo => !vo.IsDeleted)
                .OrderByDescending(vo => vo.CreatedAt)
                .AsNoTracking()
                .ToPagedListAsync(pageParameters);
        }

        public async Task<PagedList<VolunteerOrder>> GetPendingAsync(PageParameters pageParameters)
        {
            return await _context.VolunteerOrders
                .Include(vo => vo.ServiceRequest)
                .Where(vo => !vo.IsDeleted && vo.Status == OrderStatus.Pending)
                .OrderByDescending(vo => vo.CreatedAt)
                .AsNoTracking()
                .ToPagedListAsync(pageParameters);
        }
        public async Task<int?> GetVolunteerLevelNumberAsync(Guid volunteerUserId)
        {
            return await _context.UserLevels
                .AsNoTracking()
                .Where(ul =>
                    ul.Profile.UserId == volunteerUserId &&
                    !ul.IsDeleted)
                .Select(ul => (int?)ul.Level.Number)
                .FirstOrDefaultAsync();
        }

        public async Task<VolunteerOrder?> GetByIdAsync(Guid id)
        {
            return await _context.VolunteerOrders
                .Include(vo => vo.ServiceRequest)
                .AsNoTracking()
                .FirstOrDefaultAsync(vo => vo.Id == id && !vo.IsDeleted);
        }

        public async Task<VolunteerOrder?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.VolunteerOrders
                .Include(vo => vo.ServiceRequest)
                .FirstOrDefaultAsync(vo => vo.Id == id && !vo.IsDeleted);
        }
        public async Task<PagedList<VolunteerOrder>> GetByVolunteerIdAsync(
     Guid volunteerId,
     PageParameters pageParameters)
        {
            return await _context.VolunteerOrders
                .Include(vo => vo.ServiceRequest)
                .Where(vo => vo.UserId == volunteerId)
                .OrderByDescending(vo => vo.CreatedAt)
                .AsNoTracking()
                .ToPagedListAsync(pageParameters);
        }
        public async Task<bool> ExistsActiveByUserAndServiceRequestAsync(Guid userId, Guid serviceRequestId)
        {
            return await _context.VolunteerOrders
                .AnyAsync(x =>
                    x.UserId == userId &&
                    x.ServiceRequestId == serviceRequestId &&
                    x.Status != OrderStatus.Rejected);
        }
        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.VolunteerOrders
                .AnyAsync(vo => vo.Id == id && !vo.IsDeleted);
        }

        public async Task<List<VolunteerOrder>> GetEligibleByServiceRequestIdAsync(Guid serviceRequestId)
        {
            return await _context.VolunteerOrders
                .Where(vo =>
                    vo.ServiceRequestId == serviceRequestId &&
                    !vo.IsDeleted &&
                    (vo.Status == OrderStatus.Approved ||
                     vo.Status == OrderStatus.InProgress ||
                     vo.Status == OrderStatus.Completed))
                .ToListAsync();
        }

        public async Task AddAsync(VolunteerOrder volunteerOrder)
        {
            await _context.VolunteerOrders.AddAsync(volunteerOrder);
        }

        public void Update(VolunteerOrder volunteerOrder)
        {
            _context.Entry(volunteerOrder).State = EntityState.Modified;
        }

    }
}
