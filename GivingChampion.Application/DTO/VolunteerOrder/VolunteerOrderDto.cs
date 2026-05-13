using GivingChampion.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.DTO.VolunteerOrder
{
    public class VolunteerOrderDto
    {
          public Guid Id { get; set; }

            public string ServiceType { get; set; }
           public int Progress { get; set; }

        public string? Description { get; set; }

            public DateTime ScheduleDate { get; set; }
        public Guid UserId { get; set; }

        public string Status { get; set; }

            public Guid ServiceRequestId { get; set; }

            public DateTime CreatedAt { get; set; }
        }
    }