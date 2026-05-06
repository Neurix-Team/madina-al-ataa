using GivingChampion.Common.Interfaces;

namespace GivingChampion.Domain.Entities.Base
{
    public abstract class BaseEntity : ISoftDeletable
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
