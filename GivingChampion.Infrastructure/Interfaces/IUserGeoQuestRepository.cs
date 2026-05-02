using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IUserGeoQuestRepository
    {
        Task<PagedList<UserGeoQuest>> GetAllByUserIdAsync( Guid userId,PageParameters pageParameters); 
        Task<UserGeoQuest?> GetByIdAsync(Guid id);
        Task<UserGeoQuest?> GetByUserIdAndGeoQuestIdAsync(Guid userId, Guid geoQuestId);
        Task AddAsync(UserGeoQuest userGeoQuest);
        void Update(UserGeoQuest userGeoQuest);
    }
}
