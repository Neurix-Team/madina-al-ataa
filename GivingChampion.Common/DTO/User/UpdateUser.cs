using GivingChampion.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GivingChampion.Common.DTO.User
{
    public class UpdateUser
    {
        //[Required(ErrorMessage = "Id Must not be Empty")]
        //public Guid Id { get; set; }
        [Required(ErrorMessage = "Full Name Must not be Empty")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "City Must not be Empty")]
        public string City { get; set; }
        [Required(ErrorMessage = "Address Must not be Empty")]
        public string Address { get; set; }
        [MinimumAge(18, ErrorMessage = "Your Age must be 18 Years or Older")]
        [Required(ErrorMessage = "BirthDay Must be Entered")]
        public DateTime BirthDay { get; set; }
        [Required(ErrorMessage = "Phone Number Must not Be Empty")]
        public string PhoneNumber { get; set; }
    }
}
