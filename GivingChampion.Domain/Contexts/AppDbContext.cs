using GivingChampion.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Domain.Contexts
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<ApplicationRole> Roles { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Avatar> Avatars { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<AiAvatar> AiAvatars { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }
        public DbSet<UserLevel> UserLevels { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<Donor> Donors { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<UserMission> UserMissions { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<DonationOrder> DonationOrders { get; set; }
        public DbSet<DonationRequest> DonationRequests { get; set; }
        public DbSet<VolunteerOrder> VolunteerOrders { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<Certificate> Certificates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
        }
    }
}
