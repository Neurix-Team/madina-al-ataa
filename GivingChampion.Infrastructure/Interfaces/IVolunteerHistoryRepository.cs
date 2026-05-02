using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IVolunteerHistoryRepository
    {
       
            Task AddAsync(VolunteerHistories history);
            Task<List<VolunteerHistories>> GetByUserIdAsync(Guid userId);
            Task<List<VolunteerHistories>> GetByRequestIdAsync(Guid requestId);
        }
    }
