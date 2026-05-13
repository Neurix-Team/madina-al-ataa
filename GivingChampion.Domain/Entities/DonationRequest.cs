using GivingChampion.Common.Enums;
using GivingChampion.Common.Interfaces;
using GivingChampion.Domain.Entities.Base;
using GivingChampion.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using GivingChampion.Common.Enums;
using GivingChampion.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GivingChampion.Domain.Entities
{
    /// <summary>
    /// Represents a donation request created by a partner organization.
    /// Used to collect and track donation progress.
    /// </summary>
    public class DonationRequest : BaseEntity
	{
		/// <summary>
		/// Primary key (unique identifier)
		/// </summary>
		[Key]
		public Guid Id { get; set; } 

        /// <summary>
        /// Title of the donation request (e.g. "Help Hospital Cases")
        /// </summary>
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Location where the donation is needed
        /// </summary>
        public Guid LocationId { get; set; }
        public Location? Location { get; set; }

        /// <summary>
        /// Total amount required for this donation request
        /// </summary>
        [Range(1, double.MaxValue, ErrorMessage = "Donate amount must be greater than zero")]
		public decimal DonateAmount { get; set; }

        /// <summary>
        /// Remaining amount needed to complete the donation
        /// </summary>
        [Range(10, double.MaxValue, ErrorMessage = "Amount remaining cannot be negative")]
        public decimal AmountRemaining { get; set; }

		/// <summary>
		/// Indicates whether the request is verified by admin
		/// </summary>
		public RequestStatus Status { get; set; } 
        

		/// <summary>
		/// Urgency level of the request (Low, Medium, High)
		/// </summary>
		[Required(ErrorMessage = "Urgency level is required")]
        public UrgencyLevel UrgencyLevel { get; set; }

        /// <summary>
        /// Optional short description of the donation request
        /// </summary>
        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? BriefDescription { get; set; }

        /// <summary>
        /// Foreign key to Partner entity
        /// </summary>
        [Required]
        public Guid PartnerId { get; set; }

		public Partner Partner { get; set; } = null!;

        [Range(0, int.MaxValue)]
        public int XPReward { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int KPReward { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int ImpactReward { get; set; } = 0;

    }
}