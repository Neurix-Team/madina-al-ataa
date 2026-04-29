using GivingChampion.Application.Interfaces.Admin;
using GivingChampion.Common.DTO.Admin;
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
            return await _administratorRepository.GetDashboardMetricsAsync();
        }
    }
}
