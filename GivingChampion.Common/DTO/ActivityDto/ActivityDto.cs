using System;
using System.Collections.Generic;
using System.Text;
using GivingChampion.Common.Enums;

namespace GivingChampion.Common.DTO
{
    using System;
    using System.ComponentModel.DataAnnotations;

    namespace GivingChampion.Common.DTO.ActivityDto
    {
        public class ActivityDto
        {
            [Key]
            public Guid Id { get; set; }

            public string Name { get; set; } 

            public string Description { get; set; } 

            public DateTime CreatedAt { get; set; }
        }
    }
}
