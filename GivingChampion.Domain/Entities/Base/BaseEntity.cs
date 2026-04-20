using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Domain.Entities.Base
{
    public abstract class BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
