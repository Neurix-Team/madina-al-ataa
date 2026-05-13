using GivingChampion.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Application.DTO.ServiceRequestDto
{
    public class UpdateServiceRequestDto
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Required skill is required")]
        [MaxLength(150, ErrorMessage = "Required skill cannot exceed 150 characters")]
        public string RequiredSkill { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string ServiceType { get; set; } = string.Empty;
        [Required(ErrorMessage = "Urgency level is required")]
        public UrgencyLevel UrgencyLevel { get; set; }

        [Required(ErrorMessage = "Schedule date is required")]
        public DateTime ScheduleDate { get; set; }

        [Range(1, 1000, ErrorMessage = "Duration must be between 1 and 1000 hours")]
        public int Duration { get; set; }

        [MaxLength(1000, ErrorMessage = "Brief description cannot exceed 1000 characters")]
        public string BriefDescription { get; set; } = string.Empty;
        [Required(ErrorMessage = "Location id is required")]
        public Guid LocationId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Max orders must be greater than zero.")]
        public int MaxOrders { get; set; }

        [Required(ErrorMessage = "Required level id is required")]
        public Guid RequiredLevelId { get; set; }
    }
}
