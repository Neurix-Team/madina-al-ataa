using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Application.DTO.Mission
{
    public class StartMissionDto
    {
        [Required]
        public Guid MissionId { get; set; }
    }
}