using GivingChampion.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Common.DTO.VolunteerOrder
{

        public class VolunteerHistoryDto
        {
            public Guid Id { get; set; }

            public Guid UserId { get; set; }

            public Guid ServiceRequestId { get; set; }

            public Guid VolunteerOrderId { get; set; }

            public VolunteerHistoryAction Action { get; set; }


        }
    }
