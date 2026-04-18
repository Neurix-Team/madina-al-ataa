using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FullName { get; set; }
        public DateTime BirthDay { get; set; }
        public string City { get; set; } = "";
        public string Address { get; set; } = "";

        public bool IsExternal { get; set; } = false;
    }
}
