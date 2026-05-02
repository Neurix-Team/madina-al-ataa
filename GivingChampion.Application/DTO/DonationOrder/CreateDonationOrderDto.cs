using GivingChampion.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GivingChampion.Application.DTO.DonationOrder
{
    public class CreateDonationOrderDto
    {
       
            [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
            public decimal Amount { get; set; }

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
            /// Target location of the donation impact
            /// </summary>
            [Required, MaxLength(250)]
            public string TargetLocation { get; set; } = string.Empty;

            /// <summary>
            /// Receipt of payment (file path or URL)
            /// </summary>
            [MaxLength(500)]
            public string? Receipt { get; set; }

            /// <summary>
            /// Impact report after donation completion
            /// </summary>
            [MaxLength(2000)]
            public string? ImpactReport { get; set; }
            /// <summary>
            /// Foreign key to Donation Request
            /// </summary>
            public Guid DonationRequestId { get; set; }


        }
    }


