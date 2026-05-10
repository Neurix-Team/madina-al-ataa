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

      
        public async Task<VolunteerOrder?> GetByIdAsync(Guid id)
        {
            return await _context.VolunteerOrders
                .Include(vo => vo.ServiceRequest)
                .AsNoTracking()
                .FirstOrDefaultAsync(vo => vo.Id == id && !vo.IsDeleted);
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
