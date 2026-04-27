using GivingChampion.Common.Enums;

namespace GivingChampion.Common.DTO.Mission
{
    public class MissionDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Difficulty { get; set; }
        public int RequiredLevel { get; set; }
        public int KPReward { get; set; }
        public int XPReward { get; set; }
        public int ImpactReward { get; set; }
        public string Status { get; set; }
        public Guid LocationId { get; set; }
    }
}