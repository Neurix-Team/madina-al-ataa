using GivingChampion.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Common.DTO.Child
{
    public class ChildDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public decimal DailyLimit { get; set; }
        public bool AllowDonations { get; set; }
        public Guid ParentId { get; set; }
        public Guid UserId { get; set; }
        public ObjectStatus Status { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }
}
