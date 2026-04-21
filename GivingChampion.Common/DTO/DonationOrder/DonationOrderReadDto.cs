using System;

namespace GivingChampion.Common.DTO.DonationOrder
{
    public class DonationOrderReadDto
    {
        /// <summary>
        /// Unique identifier for the donation order
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Donation request basic info
        /// </summary>
        public Guid DonationRequestId { get; set; }
        public string? DonationRequestTitle { get; set; }

        /// <summary>
        /// Order payment info
        /// </summary>
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";
        public string PaymentMethod { get; set; } = string.Empty;

        /// <summary>
        /// Status of the donation order
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Date when the donation order was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
        public object PaymentStatus { get; set; }
    }
}