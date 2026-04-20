using GivingChampion.Common.DTO.Auth;
using GivingChampion.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GivingChampion.Common.DTO.DonationRequest
{
    public class CreateDonationRequestDto
    {

       
            [Required(ErrorMessage = "Title is required")]
            [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
            public string Title { get; set; } = string.Empty;

            [Required(ErrorMessage = "Location is required")]
        public Guid LocationId { get; set; }

            [Range(1, double.MaxValue, ErrorMessage = "Donate amount must be greater than zero")]
            public decimal DonateAmount { get; set; }

            [Required(ErrorMessage = "Urgency level is required")]
            public UrgencyLevel UrgencyLevel { get; set; }

            [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
            public string? BriefDescription { get; set; }

        [Required(ErrorMessage = "Partner is required")]
        public Guid PartnerId { get; set; }
        }
    }
