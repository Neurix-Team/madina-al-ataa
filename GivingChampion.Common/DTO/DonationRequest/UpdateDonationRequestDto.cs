using GivingChampion.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GivingChampion.Common.DTO.DonationRequest
{
    public class UpdateDonationRequestDto
    {
          [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
            public string? Title { get; set; }

            [MaxLength(250, ErrorMessage = "Location cannot exceed 250 characters")]
            public string? Location { get; set; }

            [Range(1, double.MaxValue, ErrorMessage = "Donate amount must be greater than zero")]
            public decimal? DonateAmount { get; set; }

            public UrgencyLevel? UrgencyLevel { get; set; }

            [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
            public string? BriefDescription { get; set; }
        }
    }
