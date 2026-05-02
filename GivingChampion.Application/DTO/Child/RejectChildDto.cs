using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GivingChampion.Application.DTO.Child
{
    public class RejectChildDto
    {
        public Guid ChildId { get; set; }
        [Required]
        public string RejectionReason { get; set; } = string.Empty;
    }
}
