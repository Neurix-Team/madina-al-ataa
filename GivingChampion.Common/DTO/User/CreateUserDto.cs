using GivingChampion.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GivingChampion.Common.DTO.User
{
    public class CreateUserDto
    {
        [EmailAddress(ErrorMessage = "Email must be a Valid Email Address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password Must be Entered")]
        public string Password { get; set; }
        [Required(ErrorMessage = "The Full Name Must be Entered")]
        public string FullName { get; set; }
        [MinimumAge(18, ErrorMessage = "Your Age must be 18 Years or Older")]
        public DateTime BirthDay { get; set; }
    }
}
