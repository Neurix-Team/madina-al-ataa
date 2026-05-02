using System;
using System.Collections.Generic;
using System.Text;
using GivingChampion.Common.Enums;

namespace GivingChampion.Application.DTO
{
    using System;
    using System.ComponentModel.DataAnnotations;

    namespace GivingChampion.Application.DTO.ActivityDto
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
