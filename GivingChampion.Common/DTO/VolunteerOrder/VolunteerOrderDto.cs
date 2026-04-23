using GivingChampion.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Common.DTO.VolunteerOrder
{
    public class VolunteerOrderDto
    {
          public Guid Id { get; set; }

            public string ServiceType { get; set; } = string.Empty;

            public string? Description { get; set; }

            public DateTime ScheduleDate { get; set; }



            public string Status { get; set; }

            

            public Guid ServiceRequestId { get; set; }

            public DateTime CreatedAt { get; set; }
        }
    }