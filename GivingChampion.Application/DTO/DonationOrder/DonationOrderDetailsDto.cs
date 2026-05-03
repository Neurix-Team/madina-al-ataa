using System;

namespace GivingChampion.Application.DTO.DonationOrder
{
    public class DonationOrderDetailsDto
    {
        public Guid Id { get; set; }

        // Donation request basic info
        public Guid DonationRequestId { get; set; }
        public string? DonationRequestTitle { get; set; }

        // Order payment info
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";
        public string PaymentMethod { get; set; } = string.Empty;

        // Status should usually come from enum in entity, but exposed as string for frontend
        public string? Status { get; set; }
        public string? PaymentStatus { get; set; }

        // Useful for showing whether proof exists without loading full file path
        public bool HasReceipt { get; set; }
        public string? Receipt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}