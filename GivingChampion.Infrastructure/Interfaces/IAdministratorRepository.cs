using GivingChampion.Common.DTO.Admin;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IAdministratorRepository
    {
        Task<DashboardMetricsDto> GetDashboardMetricsAsync();
    }
}
