using GivingChampion.Application.DTO.Rewards;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.Interfaces.Reward
{
    public interface IBadgeRequirementParser
    {
        IReadOnlyList<ParsedBadgeCondition> Parse(string requirement);
    }
}
