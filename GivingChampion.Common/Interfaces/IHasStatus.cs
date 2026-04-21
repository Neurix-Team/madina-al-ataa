using GivingChampion.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GivingChampion.Common.Interfaces
{
    public interface IHasStatus
    {
        public ObjectStatus Status { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public Guid? ApprovedById { get; set; }
    }
}
