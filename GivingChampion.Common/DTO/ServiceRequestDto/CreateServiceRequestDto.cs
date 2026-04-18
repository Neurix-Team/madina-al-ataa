using GivingChampion.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Common.DTO.ServiceRequestDto
{
    public class CreateServiceRequestDto
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Required skill is required")]
        [MaxLength(150)]
        public string RequiredSkill { get; set; } = string.Empty;

        [Required(ErrorMessage = "Urgency level is required")]
        public UrgencyLevel UrgencyLevel { get; set; }

        [Required(ErrorMessage = "Schedule date is required")]
        public DateTime ScheduleDate { get; set; }

        [Range(1, 1000, ErrorMessage = "Duration must be between 1 and 1000")]
        public int Duration { get; set; }

        [MaxLength(1000)]
        public string BriefDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Partner id is required")]
        public Guid PartnerId { get; set; }
    }
}