using GivingChampion.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Common.DTO.DonationRequest
{
    public class ListDonationRequestDto
    {
      
            public Guid Id { get; set; }

            public string Title { get; set; } = string.Empty;

            public string Location { get; set; } = string.Empty;

            public decimal DonateAmount { get; set; }

            public decimal AmountRemaining { get; set; }

            public bool IsFulfilled { get; set; }

            public bool IsVerified { get; set; }

            public UrgencyLevel UrgencyLevel { get; set; }

            public DateTime CreatedAt { get; set; }
        }
    }
