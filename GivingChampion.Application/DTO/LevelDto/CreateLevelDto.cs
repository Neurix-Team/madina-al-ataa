using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Application.DTO.LevelDto
{
    public class CreateLevelDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Level number must be greater than 0")]
        public int Number { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Max XP must be 0 or greater")]
        public int MaxXp { get; set; }
    }
}