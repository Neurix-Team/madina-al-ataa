using GivingChampion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IVolunteerHistoryRepository
    {
       
            Task AddAsync(VolunteerHistories history);
            Task<List<VolunteerHistories>> GetByUserIdAsync(Guid userId);
            Task<List<VolunteerHistories>> GetByRequestIdAsync(Guid requestId);
            Task SaveChangesAsync();
        }
    }
