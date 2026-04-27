using GivingChampion.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GivingChampion.Common.DTO.DonationRequest
{
    public class DonationRequestDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid LocationId { get; set; }
        public decimal DonateAmount { get; set; }
        public decimal AmountRemaining { get; set; }
        public string Status { get; set; }
        public string UrgencyLevel { get; set; }
        public string? BriefDescription { get; set; }
        public Guid PartnerId { get; set; }
    }
}