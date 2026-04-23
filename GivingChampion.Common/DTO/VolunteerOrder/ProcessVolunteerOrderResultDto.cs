using System;

namespace GivingChampion.Common.DTO.VolunteerOrder
{
    public class ProcessVolunteerOrderResultDto
    {
        public Guid Id { get; set; }

        public Guid VolunteerId { get; set; }

        public Guid ServiceRequestId { get; set; }

        public string ServiceType { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;


        public string Message { get; set; } = string.Empty;
    }
}