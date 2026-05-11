using GivingChampion.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.DTO.Rewards
{
    public class ParsedBadgeCondition
    {
        public BadgeConditionMetric Metric { get; set; }

        public BadgeComparisonOperator Operator { get; set; }

        public int TargetValue { get; set; }

        public RewardSourceType? SourceType { get; set; }

        public Guid? SourceEntityId { get; set; }
    }
}
