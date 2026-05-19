using System;
using System.ComponentModel.DataAnnotations;
using GivingChampion.Application.DTO.Donor;
using GivingChampion.Application.DTO.UserBadge;
using GivingChampion.Application.DTO.Volunteer;

namespace GivingChampion.Application.DTO.ProfileDto
{
    public class ProfileDto
    {
        [Key]
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public double Rating { get; set; }

        public int Impact { get; set; }

        public Guid UserId { get; set; }

        public Guid AvatarId { get; set; }

        public Guid LevelId { get; set; }

        public string? AvatarName { get; set; }

        public int? LevelNumber { get; set; }
        public VolunteerDto Volunteer { get; set; }
        public DonorDto Donor { get; set; }
        public List<UserBadgeDto> UserBadges { get; set; }
    }
}