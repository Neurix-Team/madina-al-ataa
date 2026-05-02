using GivingChampion.Common.Enums;

namespace GivingChampion.Application.DTO.Mission
{
    public class UserMissionDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid MissionId { get; set; }
        public int Progress { get; set; }
        public string Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string MissionTitle { get; set; } = string.Empty;   // For display
        public int KPReward { get; set; }
        public int XPReward { get; set; }
    }
}