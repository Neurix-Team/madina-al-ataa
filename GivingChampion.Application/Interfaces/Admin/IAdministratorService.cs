using GivingChampion.Common.DTO.Admin;

namespace GivingChampion.Application.Interfaces.Admin
{
    public interface IAdministratorService
    {
        Task<DashboardMetricsDto> GetDashboardMetricsAsync();
    }
}
