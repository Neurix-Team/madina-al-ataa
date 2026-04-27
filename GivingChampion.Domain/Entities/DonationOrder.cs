using GivingChampion.Common.Enums;
using GivingChampion.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GivingChampion.Common.Interfaces;
using GivingChampion.Domain.Entities.Base;
using GivingChampion.Domain.Enums;


namespace GivingChampion.Domain.Entities
{
    /// <summary>
    /// Represents a donation order made by a donor for a specific donation request.
    /// </summary>
    public class DonationOrder : BaseEntity
    {
        /// <summary>
        /// Primary key (unique identifier)
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Donation amount paid by the donor
        /// </summary>
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Currency used in the donation (e.g., EGP, USD)
        /// </summary>
        [Required, MaxLength(10)]
        public string Currency { get; set; } = "EGP";

        /// <summary>
        /// Payment method used (Card, Cash, Wallet, etc.)
        /// </summary>
        [Required, MaxLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        /// <summary>
        /// Donation category (Medical, Education, Food, etc.)
        /// </summary>
        [Required, MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Status of the donation order lifecycle
        /// (Pending, Approved, Rejected)
        /// </summary>
        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        /// <summary>
        /// Receipt of payment (file path or URL)
        /// </summary>
        [MaxLength(500)]
        public string? Receipt { get; set; }

        ///// <summary>
        ///// Impact report after donation completion
        ///// </summary>
        //[MaxLength(2000)]
        //public string? ImpactReport { get; set; }

        /// <summary>
        /// Foreign key to Donor
        /// </summary>
        public Guid DonorId { get; set; }

        /// <summary>
        /// Donor Reference
        /// </summary>
        [ForeignKey("DonorId")]
        public ApplicationUser Donor { get; set; }

        /// <summary>
        /// Foreign key to Donation Request
        /// </summary>
        public Guid DonationRequestId { get; set; }

        // Navigation Property to DonationRequest
        [ForeignKey("DonationRequestId")]
        public DonationRequest DonationRequest { get; set; }
    }
}