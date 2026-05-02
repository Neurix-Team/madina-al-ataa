using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Application.DTO.Mission
{
    public class UpdateProgressDto
    {
        [Range(0, 100)]
        public int Progress { get; set; }
    }
}