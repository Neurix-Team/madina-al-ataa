using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.DTO.User
{
    public class GetUserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDay { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
