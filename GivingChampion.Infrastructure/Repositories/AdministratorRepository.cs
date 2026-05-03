using GivingChampion.Common.Enums;
using GivingChampion.Persistence.Contexts;
using GivingChampion.Domain.Enums;
using GivingChampion.Persistance.Interfaces;
using GivingChampion.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.Infrastructure.Persistence.Repositories
{
    public class AdministratorRepository : IAdministratorRepository
    {
        private readonly AppDbContext _context;

        public AdministratorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardMetricsData> GetDashboardMetricsAsync()
        {
            return new DashboardMetricsData
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalDonors = await _context.Donors.CountAsync(d => !d.IsDeleted),
                TotalVolunteers = await _context.Volunteers.CountAsync(v => !v.IsDeleted),
                TotalPartners = await _context.Partners.CountAsync(p => !p.IsDeleted),
                TotalChildren = await _context.Children.CountAsync(c => !c.IsDeleted),

                TotalDonationRequests = await _context.DonationRequests.CountAsync(r => !r.IsDeleted),
                PendingDonationRequests = await _context.DonationRequests.CountAsync(r => !r.IsDeleted && r.Status == RequestStatus.Pending),
                CompletedDonationRequests = await _context.DonationRequests.CountAsync(r => !r.IsDeleted && r.Status == RequestStatus.Completed),

                TotalServiceRequests = await _context.ServiceRequests.CountAsync(r => !r.IsDeleted),
                PendingServiceRequests = await _context.ServiceRequests.CountAsync(r => !r.IsDeleted && r.Status == RequestStatus.Pending),
                CompletedServiceRequests = await _context.ServiceRequests.CountAsync(r => !r.IsDeleted && r.Status == RequestStatus.Completed),

                TotalDonationOrders = await _context.DonationOrders.CountAsync(o => !o.IsDeleted),
                PendingDonationOrders = await _context.DonationOrders.CountAsync(o => !o.IsDeleted && o.Status == OrderStatus.Pending),
                CompletedDonationOrders = await _context.DonationOrders.CountAsync(o => !o.IsDeleted && o.Status == OrderStatus.Completed),

                TotalVolunteerOrders = await _context.VolunteerOrders.CountAsync(o => !o.IsDeleted),
                PendingVolunteerOrders = await _context.VolunteerOrders.CountAsync(o => !o.IsDeleted && o.Status == OrderStatus.Pending),
                CompletedVolunteerOrders = await _context.VolunteerOrders.CountAsync(o => !o.IsDeleted && o.Status == OrderStatus.Completed),

                TotalDonationAmount = await _context.DonationOrders
                    .Where(o => !o.IsDeleted && o.Status == OrderStatus.Completed)
                    .SumAsync(o => (decimal?)o.Amount) ?? 0,
                TotalDonationAmountRemaining = await _context.DonationRequests
                    .Where(r => !r.IsDeleted)
                    .SumAsync(r => (decimal?)r.AmountRemaining) ?? 0
            };
        }
    }
}
