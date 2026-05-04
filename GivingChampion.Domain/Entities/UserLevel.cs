using GivingChampion.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Domain.Entities
{
    public class UserLevel : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Range(0, int.MaxValue)]
        public int Xp { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int Kp { get; set; } = 0;

        public Guid LevelId { get; set; }
        public Level Level { get; set; }

        public Guid ProfileId { get; set; }
        public Profile Profile { get; set; }
    }
}