using GivingChampion.Persistance.Models;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IAdministratorRepository
    {
        Task<DashboardMetricsData> GetDashboardMetricsAsync();
    }
}
