using GivingChampion.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Persistance.Models
{
    public class RewardActionData
    {
        public Guid UserId { get; set; }

        public RewardSourceType SourceType { get; set; }

        public Guid SourceEntityId { get; set; }

        public Guid ActionEntityId { get; set; }

        public int XPReward { get; set; }

        public int KPReward { get; set; }

        public int ImpactReward { get; set; }

        public string Reason { get; set; } = string.Empty;
    }
}
