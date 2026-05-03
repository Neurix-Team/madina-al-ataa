using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IGeoQuestRepository
    {
        Task<PagedList<GeoQuest>> GetAllAsync(PageParameters pageParameters);
        Task<GeoQuest?> GetByIdAsync(Guid id);
        Task AddAsync(GeoQuest geoQuest);
        void Update(GeoQuest geoQuest);
    }
}
