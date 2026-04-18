using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Common.DTO
{
    using System.ComponentModel.DataAnnotations;

    namespace GivingChampion.Common.DTO.ActivityDto
    {
        public class CreateActivityDto
        {
            [Required]
            [StringLength(100)]
            public string Name { get; set; } = string.Empty;

            [Required]
            [StringLength(500)]
            public string Description { get; set; } = string.Empty;
        }
    }
}
