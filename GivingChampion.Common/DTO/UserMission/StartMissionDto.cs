using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Common.DTO.Mission
{
    public class StartMissionDto
    {
        [Required]
        public Guid MissionId { get; set; }
    }
}