using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Common.DTO.Mission
{
    public class UpdateProgressDto
    {
        [Range(0, 100)]
        public int Progress { get; set; }
    }
}