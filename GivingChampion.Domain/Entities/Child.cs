using GivingChampion.Common.Interfaces;
using GivingChampion.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GivingChampion.Domain.Entities
{
    [PrimaryKey("Id")]
    public class Child : ISoftDeletable, IHasStatus
    {
        public Guid Id { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal DailyLimit { get; set; }
        public bool AllowDonations { get; set; }
        public ObjectStatus Status { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? ApprovedAt { get; set; }
        [ForeignKey("Approver")]
        public Guid? ApprovedById { get; set; }
        public ApplicationUser? Approver { get; set; }
        [ForeignKey("Parent")]
        public Guid ParentId { get; set; }
        public ApplicationUser Parent { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
