using System;
using System.ComponentModel.DataAnnotations;
using GivingChampion.Common.Enums;
using GivingChampion.Domain.Enums;

namespace GivingChampion.Domain.Entities
{
	/// <summary>
	/// Represents a donation request created by a partner organization.
	/// Used to collect and track donation progress.
	/// </summary>
	public class DonationRequest
	{
		/// <summary>
		/// Primary key (unique identifier)
		/// </summary>
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		/// <summary>
		/// Title of the donation request (e.g. "Help Hospital Cases")
		/// </summary>
		[Required(ErrorMessage = "Title is required")]
		[MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
		public string Title { get; set; } = string.Empty;

		/// <summary>
		/// Location where the donation is needed
		/// </summary>
		[Required(ErrorMessage = "Location is required")]
		[MaxLength(250, ErrorMessage = "Location cannot exceed 250 characters")]
		public string Location { get; set; } = string.Empty;

		/// <summary>
		/// Total amount required for this donation request
		/// </summary>
		[Range(1, double.MaxValue, ErrorMessage = "Donate amount must be greater than zero")]
		public decimal DonateAmount { get; set; }

		/// <summary>
		/// Remaining amount needed to complete the donation
		/// </summary>
		[Range(0, double.MaxValue, ErrorMessage = "Amount remaining cannot be negative")]
		public decimal AmountRemaining { get; set; }

		/// <summary>
		/// Indicates whether the request is verified by admin
		/// </summary>
		public bool IsVerified { get; set; } = false;

		/// <summary>
		/// Indicates whether the donation request is fully funded
		/// </summary>
		public bool IsFulfilled => AmountRemaining == 0;

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

		/// <summary>
		/// Navigation property to Partner
		/// </summary>
		public Partner Partner { get; set; } = null!;

		/// <summary>
		/// Timestamp when the request was created (UTC)
		/// </summary>
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}