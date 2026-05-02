using GivingChampion.Application.Interfaces.Admin;
using GivingChampion.Application.DTO.Admin;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.Application.Services
{
    public class AdministratorService : IAdministratorService
    {
        private readonly IAdministratorRepository _administratorRepository;

        public AdministratorService(IAdministratorRepository administratorRepository)
        {
            _administratorRepository = administratorRepository;
        }

        public async Task<DashboardMetricsDto> GetDashboardMetricsAsync()
        {
            var metrics = await _administratorRepository.GetDashboardMetricsAsync();

            return new DashboardMetricsDto
            {
                TotalUsers = metrics.TotalUsers,
                TotalDonors = metrics.TotalDonors,
                TotalVolunteers = metrics.TotalVolunteers,
                TotalPartners = metrics.TotalPartners,
                TotalChildren = metrics.TotalChildren,
                TotalDonationRequests = metrics.TotalDonationRequests,
                PendingDonationRequests = metrics.PendingDonationRequests,
                CompletedDonationRequests = metrics.CompletedDonationRequests,
                TotalServiceRequests = metrics.TotalServiceRequests,
                PendingServiceRequests = metrics.PendingServiceRequests,
                CompletedServiceRequests = metrics.CompletedServiceRequests,
                TotalDonationOrders = metrics.TotalDonationOrders,
                PendingDonationOrders = metrics.PendingDonationOrders,
                CompletedDonationOrders = metrics.CompletedDonationOrders,
                TotalVolunteerOrders = metrics.TotalVolunteerOrders,
                PendingVolunteerOrders = metrics.PendingVolunteerOrders,
                CompletedVolunteerOrders = metrics.CompletedVolunteerOrders,
                TotalDonationAmount = metrics.TotalDonationAmount,
                TotalDonationAmountRemaining = metrics.TotalDonationAmountRemaining,
                GeneratedAtUtc = DateTime.UtcNow
            };
        }
    }
}
