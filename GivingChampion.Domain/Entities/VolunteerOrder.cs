using GivingChampion.Domain.Entities.Base;
using GivingChampion.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GivingChampion.Domain.Entities
{
    /// <summary>
    /// Represents a volunteer order where a volunteer is assigned to a service request.
    /// Tracks scheduling, status, and assignment details.
    /// </summary>
    public class VolunteerOrder : BaseEntity
	{
		/// <summary>
		/// Primary key (unique identifier)
		/// </summary>
		[Key]
		public Guid Id { get; set; } 

	
        /// <summary>
        /// Current status of the volunteer order (Pending, Approved, Rejected, Completed)
        /// </summary>
        [Required]
		public OrderStatus Status { get; set; } 


		/// <summary>
		/// Foreign key to ServiceRequest
		/// </summary>
		public Guid ServiceRequestId { get; set; }
        [ForeignKey(nameof(ServiceRequestId))]
        public ServiceRequest? ServiceRequest { get; set; }
        /// <summary>
        /// Foreign key to Volunteer
        /// </summary>
        public Guid UserId { get; set; } // Identity User (Volunteer)

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }  // Navigation property to the related Application User
        public DateTime? RejectedAt { get; set; } 
        public DateTime? ApprovedAt { get; set; } 

        public string ?RejectionReason { get; set; }
    }
}