using GivingChampion.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Common.DTO.ServiceRequestDto
{
    public class ServiceRequestDto
    {
  
     
            public Guid Id { get; set; }

            public string Title { get; set; } = string.Empty;

            public string RequiredSkill { get; set; } = string.Empty;

            public string UrgencyLevel { get; set; }

            public DateTime ScheduleDate { get; set; }

            public int Duration { get; set; }

            public string BriefDescription { get; set; } = string.Empty;

            public Guid PartnerId { get; set; }

            public string FullName { get; set; } = string.Empty;

            public string Status { get; set; } = string.Empty;

            public DateTime CreatedAt { get; set; }
        }
    }

