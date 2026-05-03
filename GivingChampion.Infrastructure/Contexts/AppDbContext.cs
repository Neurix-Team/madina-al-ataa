using GivingChampion.Common.Extensions.SoftDelete;
using GivingChampion.Common.Interfaces;
using GivingChampion.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GivingChampion.Persistence.Contexts
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
        public DbSet<VolunteerHistories> VolunteerHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ====================== GLOBAL SOFT DELETE FILTER ======================
            modelBuilder.ApplySoftDeleteQueryFilter();

            // ====================== Avatar ======================

            modelBuilder.Entity<Avatar>(entity =>
            {
                entity.HasOne(a => a.Profile)
                    .WithMany()
                    .HasForeignKey(a => a.ProfileId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ====================== Profile ======================

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.HasOne(p => p.User)
                    .WithMany()
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Level)
                    .WithMany(l => l.Profiles)
                    .HasForeignKey(p => p.LevelId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.UserBadges)
                    .WithOne(ub => ub.Profile)
                    .HasForeignKey(ub => ub.ProfileId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(p => p.Reviews)
                    .WithOne(r => r.Profile)
                    .HasForeignKey(r => r.ProfileId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(p => p.UserId)
                    .IsUnique();
            });

            // ====================== UserBadge ======================

            modelBuilder.Entity<UserBadge>(entity =>
            {
                entity.HasOne(ub => ub.Profile)
                    .WithMany(p => p.UserBadges)
                    .HasForeignKey(ub => ub.ProfileId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ub => ub.Badge)
                    .WithMany(b => b.UserBadges)
                    .HasForeignKey(ub => ub.BadgeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(ub => new { ub.ProfileId, ub.BadgeId })
                    .IsUnique();
            });

            // ====================== UserLevel ======================

            modelBuilder.Entity<UserLevel>(entity =>
            {
                entity.HasOne(ul => ul.Level)
                    .WithMany()
                    .HasForeignKey(ul => ul.LevelId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ul => ul.Profile)
                    .WithMany()
                    .HasForeignKey(ul => ul.ProfileId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ====================== Review ======================

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasOne(r => r.Reviewer)
                    .WithMany()
                    .HasForeignKey(r => r.ReviewerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Profile)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(r => r.ProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ====================== Child ======================

            modelBuilder.Entity<Child>(entity =>
            {
                entity.HasOne(c => c.Approver)
                    .WithMany()
                    .HasForeignKey(c => c.ApprovedById)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Parent)
                    .WithMany()
                    .HasForeignKey(c => c.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(c => c.UserId)
                    .IsUnique();
            });

            // ====================== Donor ======================

            modelBuilder.Entity<Donor>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(d => d.UserId)
                    .IsUnique();
            });

            // ====================== Volunteer ======================

            modelBuilder.Entity<Volunteer>(entity =>
            {
                entity.HasOne(v => v.User)
                    .WithMany()
                    .HasForeignKey(v => v.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(v => v.UserId)
                    .IsUnique();
            });

            // ====================== Certificate ======================

            modelBuilder.Entity<Certificate>(entity =>
            {
                entity.HasOne(c => c.Volunteer)
                    .WithMany()
                    .HasForeignKey(c => c.IssuedTo)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ====================== DonationRequest ======================

            modelBuilder.Entity<DonationRequest>(entity =>
            {
                entity.HasOne(dr => dr.Location)
                    .WithMany()
                    .HasForeignKey(dr => dr.LocationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(dr => dr.Partner)
                    .WithMany()
                    .HasForeignKey(dr => dr.PartnerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ====================== DonationOrder ======================

            modelBuilder.Entity<DonationOrder>(entity =>
            {
                entity.HasOne(d => d.Donor)
                    .WithMany()
                    .HasForeignKey(d => d.DonorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.DonationRequest)
                    .WithMany()
                    .HasForeignKey(d => d.DonationRequestId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ====================== ServiceRequest ======================

            modelBuilder.Entity<ServiceRequest>(entity =>
            {
                entity.HasOne(sr => sr.Location)
                    .WithMany()
                    .HasForeignKey(sr => sr.LocationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sr => sr.Partner)
                    .WithMany()
                    .HasForeignKey(sr => sr.PartnerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sr => sr.Volunteer)
                    .WithMany()
                    .HasForeignKey(sr => sr.VolunteerUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ====================== VolunteerOrder ======================

            modelBuilder.Entity<VolunteerOrder>(entity =>
            {
                entity.HasOne(vo => vo.ServiceRequest)
                    .WithMany()
                    .HasForeignKey(vo => vo.ServiceRequestId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(vo => vo.User)
                    .WithMany()
                    .HasForeignKey(vo => vo.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ====================== VolunteerHistories ======================

            modelBuilder.Entity<VolunteerHistories>(entity =>
            {
                entity.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(vh => vh.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<ServiceRequest>()
                    .WithMany()
                    .HasForeignKey(vh => vh.ServiceRequestId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<VolunteerOrder>()
                    .WithMany()
                    .HasForeignKey(vh => vh.VolunteerOrderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ====================== GeoQuest ======================

            modelBuilder.Entity<GeoQuest>(entity =>
            {
                entity.HasOne(gq => gq.Location)
                    .WithMany()
                    .HasForeignKey(gq => gq.LocationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ====================== UserGeoQuest ======================

            modelBuilder.Entity<UserGeoQuest>(entity =>
            {
                entity.HasOne(ugq => ugq.GeoQuest)
                    .WithMany()
                    .HasForeignKey(ugq => ugq.GeoQuestId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ugq => ugq.User)
                    .WithMany()
                    .HasForeignKey(ugq => ugq.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(ugq => new { ugq.UserId, ugq.GeoQuestId })
                    .IsUnique();
            });

            // ====================== Mission ======================

            modelBuilder.Entity<Mission>(entity =>
            {
                entity.HasOne(m => m.Location)
                    .WithMany()
                    .HasForeignKey(m => m.LocationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ====================== UserMission ======================

            modelBuilder.Entity<UserMission>(entity =>
            {
                entity.HasOne(um => um.User)
                    .WithMany()
                    .HasForeignKey(um => um.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(um => um.Mission)
                    .WithMany()
                    .HasForeignKey(um => um.MissionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(um => new { um.UserId, um.MissionId })
                    .IsUnique();
            });

            // ====================== Notification ======================

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.User)
                    .WithMany()
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
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