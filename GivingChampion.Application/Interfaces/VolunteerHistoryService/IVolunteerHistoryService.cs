using GivingChampion.Application.DTO.VolunteerOrder;
using GivingChampion.Common.Enums;
using GivingChampion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.Interfaces.VolunteerHistoryService
{
    public interface IVolunteerHistoryService
    {
   
            Task<List<VolunteerHistoryDto>> GetUserHistory(Guid userId);

            Task<List<VolunteerHistoryDto>> GetRequestHistory(Guid requestId);

        
            Task AddAsync(
                Guid userId,
                Guid requestId,
                Guid orderId,
                VolunteerHistoryAction action,
                int? progress = null);
        }
    }
    