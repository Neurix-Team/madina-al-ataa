using GivingChampion.Common.Enums;
using GivingChampion.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Domain.Entities
{
    public class VolunteerHistories : BaseEntity
    {
      
            public Guid Id { get; set; }

            public Guid UserId { get; set; }   
            public Guid ServiceRequestId { get; set; } 

            public Guid VolunteerOrderId { get; set; }

        public VolunteerHistoryAction Action { get; set; }

        public int? ProgressValue { get; set; }

        }

}
