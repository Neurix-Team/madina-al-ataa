using GivingChampion.Application.DTO.Rewards;
using GivingChampion.Application.Interfaces.Reward;
using GivingChampion.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GivingChampion.Application.Helpers
{
    public class BadgeRequirementParser : IBadgeRequirementParser
    {
        private static readonly Regex StatRegex = new(
            @"^(?<metric>xp|kp|impact)\s*(?<operator>>=|<=|==|>|<)\s*(?<value>\d+)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex CompletedRegex = new(
            @"^completed:(?<source>service|donation|mission|geoquest)(:(?<sourceId>[0-9a-fA-F-]{36}))?\s*(?<operator>>=|<=|==|>|<)\s*(?<value>\d+)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public IReadOnlyList<ParsedBadgeCondition> Parse(string requirement)
        {
            if (string.IsNullOrWhiteSpace(requirement))
                return Array.Empty<ParsedBadgeCondition>();

            var parts = requirement
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var conditions = new List<ParsedBadgeCondition>();

            foreach (var part in parts)
            {
                var condition = ParseSingleCondition(part);

                if (condition == null)
                    throw new InvalidOperationException($"Invalid badge requirement syntax: {part}");

                conditions.Add(condition);
            }

            return conditions;
        }

        private static ParsedBadgeCondition? ParseSingleCondition(string input)
        {
            var statMatch = StatRegex.Match(input);

            if (statMatch.Success)
            {
                return new ParsedBadgeCondition
                {
                    Metric = ParseMetric(statMatch.Groups["metric"].Value),
                    Operator = ParseOperator(statMatch.Groups["operator"].Value),
                    TargetValue = int.Parse(statMatch.Groups["value"].Value)
                };
            }

            var completedMatch = CompletedRegex.Match(input);

            if (completedMatch.Success)
            {
                Guid? sourceEntityId = null;

                var sourceIdText = completedMatch.Groups["sourceId"].Value;

                if (!string.IsNullOrWhiteSpace(sourceIdText))
                    sourceEntityId = Guid.Parse(sourceIdText);

                return new ParsedBadgeCondition
                {
                    Metric = BadgeConditionMetric.Completed,
                    SourceType = ParseSourceType(completedMatch.Groups["source"].Value),
                    SourceEntityId = sourceEntityId,
                    Operator = ParseOperator(completedMatch.Groups["operator"].Value),
                    TargetValue = int.Parse(completedMatch.Groups["value"].Value)
                };
            }

            return null;
        }

        private static BadgeConditionMetric ParseMetric(string value)
        {
            return value.ToLower() switch
            {
                "xp" => BadgeConditionMetric.XP,
                "kp" => BadgeConditionMetric.KP,
                "impact" => BadgeConditionMetric.Impact,
                _ => throw new InvalidOperationException($"Unsupported metric: {value}")
            };
        }

        private static RewardSourceType ParseSourceType(string value)
        {
            return value.ToLower() switch
            {
                "service" => RewardSourceType.Service,
                "donation" => RewardSourceType.Donation,
                "mission" => RewardSourceType.Mission,
                "geoquest" => RewardSourceType.GeoQuest,
                _ => throw new InvalidOperationException($"Unsupported reward source type: {value}")
            };
        }

        private static BadgeComparisonOperator ParseOperator(string value)
        {
            return value switch
            {
                ">=" => BadgeComparisonOperator.AtLeast,
                ">" => BadgeComparisonOperator.GreaterThan,
                "==" => BadgeComparisonOperator.Equal,
                "<" => BadgeComparisonOperator.LessThan,
                "<=" => BadgeComparisonOperator.AtMost,
                _ => throw new InvalidOperationException($"Unsupported operator: {value}")
            };
        }
    }
}
