using GivingChampion.Common.Extensions.SoftDelete;
using GivingChampion.Common.Interfaces;
using GivingChampion.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GivingChampion.Domain.Contexts
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets
        //public DbSet<ApplicationUser> Users { get; set; }
        //public DbSet<ApplicationRole> Roles { get; set; }
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
        public DbSet<Activity> Activities { get; set; }
        public DbSet<GeoQuest> GeoQuests { get; set; }
        public DbSet<UserGeoQuest> UserGeoQuests { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ====================== GLOBAL SOFT DELETE FILTER ======================
            // This applies automatically to ALL entities that implement ISoftDeletable
            //modelBuilder.ApplySoftDeleteQueryFilter();

            // ====================== Specific Configurations ======================
            // Only add manual configurations here if needed (relationships, indexes, etc.)

            modelBuilder.Entity<Profile>()
                .HasMany(p => p.Badges)
                .WithOne(ub => ub.Profile)
                .HasForeignKey(ub => ub.ProfileId);

            modelBuilder.Entity<Profile>()
                .HasOne(p => p.Level)
                .WithMany(ul => ul.Profiles);

            modelBuilder.Entity<Profile>()
                .HasMany(p => p.Reviews)
                .WithOne(r => r.Profile)
                .HasForeignKey(r => r.ProfileId);
            modelBuilder.Entity<UserGeoQuest>()
                .HasOne(ugq => ugq.GeoQuest)
                .WithMany()
               .HasForeignKey(ugq => ugq.GeoQuestId);

            modelBuilder.Entity<UserGeoQuest>()
                .HasOne(ugq => ugq.User)
                .WithMany()
                .HasForeignKey(ugq => ugq.UserId);

            // Example: If you want to disable soft delete for a specific entity
            // modelBuilder.Entity<SomeEntity>().HasQueryFilter(null);
        }

        // ====================== SOFT DELETE HANDLING ======================
        public override int SaveChanges()
        {
            HandleSoftDeletes();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            HandleSoftDeletes();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void HandleSoftDeletes()
        {
            var deletedEntries = ChangeTracker.Entries<ISoftDeletable>()
                .Where(e => e.State == EntityState.Deleted);

            foreach (var entry in deletedEntries)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = DateTime.UtcNow;
            }
        }

        // ====================== HELPER METHODS ======================

        /// <summary>
        /// Use this when you need to query soft-deleted entities
        /// Example: await _context.Users.IgnoreQueryFilters().Where(...).ToListAsync();
        /// </summary>
        public IQueryable<T> QueryWithDeleted<T>() where T : class, ISoftDeletable
        {
            return Set<T>().IgnoreQueryFilters();
        }

        /// <summary>
        /// Gets only deleted entities of type T
        /// </summary>
        public IQueryable<T> GetDeleted<T>() where T : class, ISoftDeletable
        {
            return Set<T>().IgnoreQueryFilters().Where(x => x.IsDeleted);
        }
    }
}