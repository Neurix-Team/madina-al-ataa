using GivingChampion.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GivingChampion.Domain.Entities
{
	public class ServiceRequest
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();
		// Title of the request
		[Required]
		[MaxLength(200)]
		public string Title { get; set; } = string.Empty;

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

		// Duration in hours
		[Range(1, 1000)]
		public int Duration { get; set; }

		// Status of the request (Pending, Approved, Completed, etc.)
		[Required]
		public RequestStatus Status { get; set; }
        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Short description
        [MaxLength(1000)]
		public string? BriefDescription { get; set; }

		// Foreign Key
		[Required]
		public Guid PartnerId { get; set; }

		// Navigation Property
		[ForeignKey("PartnerId")]
		public Partner Partner { get; set; } = null!;
	}


	
}