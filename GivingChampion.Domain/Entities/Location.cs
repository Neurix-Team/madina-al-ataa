using GivingChampion.Common.Interfaces;
using GivingChampion.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GivingChampion.Domain.Entities
{
    [PrimaryKey("Id")]
    public class Location : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public int RequiredLevel { get; set; }
        public string Longitude { get; set; } = "";
        public string Latitude { get; set; } = "";
        //public bool IsDeleted { get; set; }
        //public DateTime? DeletedAt { get; set; }
    }
}