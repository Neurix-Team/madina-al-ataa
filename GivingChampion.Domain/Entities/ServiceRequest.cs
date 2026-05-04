using GivingChampion.Common.Enums;
using GivingChampion.Common.Interfaces;
using GivingChampion.Domain.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GivingChampion.Domain.Entities
{
    public class ServiceRequest : BaseEntity
	{
		[Key]
		public Guid Id { get; set; }
		// Title of the request
		[Required]
		[MaxLength(200)]
		public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Type of service provided by the volunteer (Medical, Teaching, etc.)
        /// </summary>
        [Required, MaxLength(100)]
        public string ServiceType { get; set; } = string.Empty;


        // Required skill for this service
        [Required]
		[MaxLength(150)]
		public string RequiredSkill { get; set; } = string.Empty;

		// Urgency level (Low, Medium, High)
		[Required]
        public UrgencyLevel UrgencyLevel { get; set; }

		// Scheduled date for the service
		[Required]
		public DateTime ScheduleDate { get; set; }
        [Required]
        public Guid LocationId { get; set; }
        public Location? Location { get; set; }
        // Duration in hours
        [Range(1, 1000)]
		public int Duration { get; set; }

		// Status of the request (Pending, Approved, Completed, etc.)
		[Required]
		public RequestStatus Status { get; set; }
 
        // Short description
        [MaxLength(1000)]
		public string? BriefDescription { get; set; }
        /// <summary>
        /// Impact report after donation completion
        /// </summary>
        [MaxLength(2000)]
        public string? ImpactReport { get; set; }
        // Foreign Key
        [Required]
		public Guid PartnerId { get; set; }

		// Navigation Property
		public Partner Partner { get; set; } = null!;
        [Range(0, 100)]
        public int Progress { get; set; }
        public Guid? VolunteerUserId { get; set; }
        public ApplicationUser? Volunteer { get; set; }



    }


	
}