using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IGeoQuestRepository
    {
        Task<List<GeoQuest>> GetAllAsync(PageParameters pageParameters);
        Task<GeoQuest?> GetByIdAsync(Guid id);
        Task AddAsync(GeoQuest geoQuest);
        void Update(GeoQuest geoQuest);
        Task SaveChangesAsync();
    }
}