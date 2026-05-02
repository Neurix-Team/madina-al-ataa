using GivingChampion.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.DTO.DonationRequest
{
    public class FilterDonationRequestDto
    {
       
      
            public string? SearchTerm { get; set; }

            public bool? IsVerified { get; set; }

            public bool? IsFulfilled { get; set; }

            public UrgencyLevel? UrgencyLevel { get; set; }

            public Guid? PartnerId { get; set; }


        }
    }
