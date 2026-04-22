using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;

namespace GivingChampion.API.Interfaces
{
    public interface IUserGeoQuestRepository
    {
        Task<List<UserGeoQuest>> GetAllAsync(PageParameters pageParameters);
        Task<UserGeoQuest?> GetByIdAsync(Guid id);
        Task AddAsync(UserGeoQuest userGeoQuest);
        void Update(UserGeoQuest userGeoQuest);
        Task SaveChangesAsync();
    }
}