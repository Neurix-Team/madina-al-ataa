using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GivingChampion.Common.DTO.User
{
    public class DeleteUser
    {
        [Required(ErrorMessage = "User Id Must not be Empty")]
        public Guid Id { get; set; }
    }
}
