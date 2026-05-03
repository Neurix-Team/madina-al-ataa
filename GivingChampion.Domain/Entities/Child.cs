using GivingChampion.Common.Interfaces;
using GivingChampion.Domain.Entities.Base;
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
    public class Child : BaseEntity, IHasStatus
    {
        public Guid Id { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal DailyLimit { get; set; }
        public bool AllowDonations { get; set; }
        public ObjectStatus Status { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public Guid? ApprovedById { get; set; }
        public ApplicationUser? Approver { get; set; }
        public Guid ParentId { get; set; }
        public ApplicationUser Parent { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        //public DateTime CreatedAt { get; set; }
        //public bool IsDeleted { get; set; }
        //public DateTime? DeletedAt { get; set; }
    }
}
