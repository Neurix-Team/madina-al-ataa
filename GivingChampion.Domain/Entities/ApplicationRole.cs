using GivingChampion.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Domain.Entities
{
    public class ApplicationRole : IdentityRole<Guid>, ISoftDeletable
    {
        public ApplicationRole()
        {
        }

        public ApplicationRole(string roleName) : base(roleName)
        {
            NormalizedName = roleName?.ToUpperInvariant();
        }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
