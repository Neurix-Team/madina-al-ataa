using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GivingChampion.Common.DTO.DonationOrder
{
        public class UpdateDonationOrderDTO
        {
   

            [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
            public decimal Amount { get; set; }

            [Required, MaxLength(50)]
            public string PaymentMethod { get; set; } = string.Empty;

            [Required, MaxLength(100)]
            public string Category { get; set; } = string.Empty;

            [Required, MaxLength(250)]
            public string TargetLocation { get; set; } = string.Empty;

            [MaxLength(500)]
            public string? Receipt { get; set; }

            [MaxLength(2000)]
            public string? ImpactReport { get; set; }


        }
    }

