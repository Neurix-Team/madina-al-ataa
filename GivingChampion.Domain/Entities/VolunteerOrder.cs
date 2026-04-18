using System;
using System.ComponentModel.DataAnnotations;
using GivingChampion.Domain.Enums;

namespace GivingChampion.Domain.Entities
{
	/// <summary>
	/// Represents a volunteer order where a volunteer is assigned to a service request.
	/// Tracks scheduling, status, and assignment details.
	/// </summary>
	public class VolunteerOrder
	{
		/// <summary>
		/// Primary key (unique identifier)
		/// </summary>
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		/// <summary>
		/// Type of service provided by the volunteer (Medical, Teaching, etc.)
		/// </summary>
		[Required, MaxLength(100)]
		public string ServiceType { get; set; } = string.Empty;

		/// <summary>
		/// Description of the volunteer task
		/// </summary>
		[MaxLength(1000)]
		public string? Description { get; set; }

		/// <summary>
		/// Scheduled date for the volunteer work
		/// </summary>
		public DateTime ScheduleDate { get; set; }

		/// <summary>
		/// Duration of the task in hours
		/// </summary>
		[Range(1, int.MaxValue)]
		public int Duration { get; set; }

		/// <summary>
		/// Location where the volunteer work will take place
		/// </summary>
		[Required, MaxLength(250)]
		public string Location { get; set; } = string.Empty;

		/// <summary>
		/// Current status of the volunteer order (Pending, Approved, Rejected, Completed)
		/// </summary>
		[Required]
		public OrderStatus Status { get; set; } = OrderStatus.Pending;

		/// <summary>
		/// Additional notes from admin or volunteer
		/// </summary>
		[MaxLength(1000)]
		public string? Notes { get; set; }

		/// <summary>
		/// Foreign key to ServiceRequest
		/// </summary>
		public Guid ServiceRequestId { get; set; }

		/// <summary>
		/// Foreign key to Volunteer
		/// </summary>
		public Guid VolunteerId { get; set; }

		/// <summary>
		/// Record creation timestamp (UTC)
		/// </summary>
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}