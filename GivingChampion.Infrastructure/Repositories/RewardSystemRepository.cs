using GivingChampion.Common.Enums;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using GivingChampion.Persistance.Models;
using GivingChampion.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.Persistance.Repositories
{
    public class RewardSystemRepository : IRewardSystemRepository
    {
        private readonly AppDbContext _context;

        public RewardSystemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<RewardActionData>> GetServiceRequestRewardDataAsync(
            Guid serviceRequestId,
            CancellationToken cancellationToken = default)
        {
            var orders = await _context.VolunteerOrders
                .AsNoTracking()
                .Include(o => o.ServiceRequest)
                .Where(o =>
                    o.ServiceRequestId == serviceRequestId &&
                    !o.IsDeleted &&
                    (o.Status == Domain.Enums.OrderStatus.Approved ||
                     o.Status == Domain.Enums.OrderStatus.InProgress ||
                     o.Status == Domain.Enums.OrderStatus.Completed))
                .ToListAsync(cancellationToken);

            return orders
                .Where(o => o.ServiceRequest != null)
                .Select(o => new RewardActionData
                {
                    UserId = o.UserId,
                    SourceType = RewardSourceType.Service,
                    SourceEntityId = o.ServiceRequestId,
                    ActionEntityId = o.Id,
                    XPReward = o.ServiceRequest!.XPReward,
                    KPReward = o.ServiceRequest.KPReward,
                    ImpactReward = o.ServiceRequest.ImpactReward,
                    Reason = $"Completed service request: {o.ServiceRequest.Title}",
                    CompletionTitle = o.ServiceRequest.Title,
                    CertificateHours = o.ServiceRequest.Duration
                })
                .ToList();
        }

        public async Task<RewardActionData?> GetVolunteerOrderRewardDataAsync(
            Guid volunteerOrderId,
            CancellationToken cancellationToken = default)
        {
            var order = await _context.VolunteerOrders
                .AsNoTracking()
                .Include(o => o.ServiceRequest)
                .FirstOrDefaultAsync(o => o.Id == volunteerOrderId && !o.IsDeleted, cancellationToken);

            if (order?.ServiceRequest == null)
                return null;

            return new RewardActionData
            {
                UserId = order.UserId,
                SourceType = RewardSourceType.Service,
                SourceEntityId = order.ServiceRequestId,
                ActionEntityId = order.Id,
                XPReward = order.ServiceRequest.XPReward,
                KPReward = order.ServiceRequest.KPReward,
                ImpactReward = order.ServiceRequest.ImpactReward,
                Reason = $"Completed service request: {order.ServiceRequest.Title}",
                CompletionTitle = order.ServiceRequest.Title,
                CertificateHours = order.ServiceRequest.Duration
            };
        }

        public async Task<RewardActionData?> GetDonationOrderRewardDataAsync(
            Guid donationOrderId,
            CancellationToken cancellationToken = default)
        {
            var order = await _context.DonationOrders
                .AsNoTracking()
                .Include(o => o.DonationRequest)
                .FirstOrDefaultAsync(o => o.Id == donationOrderId && !o.IsDeleted, cancellationToken);

            if (order?.DonationRequest == null)
                return null;

            return new RewardActionData
            {
                UserId = order.DonorId,
                SourceType = RewardSourceType.Donation,
                SourceEntityId = order.DonationRequestId,
                ActionEntityId = order.Id,
                XPReward = order.DonationRequest.XPReward,
                KPReward = order.DonationRequest.KPReward,
                ImpactReward = order.DonationRequest.ImpactReward,
                Reason = $"Completed donation request: {order.DonationRequest.Title}"
            };
        }

        public async Task<RewardActionData?> GetUserMissionRewardDataAsync(
            Guid userMissionId,
            CancellationToken cancellationToken = default)
        {
            var userMission = await _context.UserMissions
                .AsNoTracking()
                .Include(um => um.Mission)
                .FirstOrDefaultAsync(um => um.Id == userMissionId && !um.IsDeleted, cancellationToken);

            if (userMission?.Mission == null)
                return null;

            return new RewardActionData
            {
                UserId = userMission.UserId,
                SourceType = RewardSourceType.Mission,
                SourceEntityId = userMission.MissionId,
                ActionEntityId = userMission.Id,
                XPReward = userMission.Mission.XPReward,
                KPReward = userMission.Mission.KPReward,
                ImpactReward = userMission.Mission.ImpactReward,
                Reason = $"Completed mission: {userMission.Mission.Title}"
            };
        }

        public async Task<RewardActionData?> GetUserGeoQuestRewardDataAsync(
            Guid userGeoQuestId,
            CancellationToken cancellationToken = default)
        {
            var userGeoQuest = await _context.UserGeoQuests
                .AsNoTracking()
                .Include(ugq => ugq.GeoQuest)
                .FirstOrDefaultAsync(ugq => ugq.Id == userGeoQuestId && !ugq.IsDeleted, cancellationToken);

            if (userGeoQuest?.GeoQuest == null)
                return null;

            return new RewardActionData
            {
                UserId = userGeoQuest.UserId,
                SourceType = RewardSourceType.GeoQuest,
                SourceEntityId = userGeoQuest.GeoQuestId,
                ActionEntityId = userGeoQuest.Id,
                XPReward = userGeoQuest.GeoQuest.XPReward,
                KPReward = userGeoQuest.GeoQuest.KPReward,
                ImpactReward = userGeoQuest.GeoQuest.ImpactReward,
                Reason = $"Completed geo quest: {userGeoQuest.GeoQuest.Title}"
            };
        }

        public Task<Profile?> GetProfileWithUserLevelByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return _context.Profiles
                .Include(p => p.UserLevel)
                .FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted, cancellationToken);
        }

        public Task<Profile?> GetProfileWithUserLevelByIdAsync(
            Guid profileId,
            CancellationToken cancellationToken = default)
        {
            return _context.Profiles
                .Include(p => p.UserLevel)
                .FirstOrDefaultAsync(p => p.Id == profileId && !p.IsDeleted, cancellationToken);
        }

        public async Task<UserLevel> GetOrCreateUserLevelAsync(
            Profile profile,
            CancellationToken cancellationToken = default)
        {
            if (profile.UserLevel != null)
                return profile.UserLevel;

            var userLevel = await _context.UserLevels
                .FirstOrDefaultAsync(ul => ul.ProfileId == profile.Id && !ul.IsDeleted, cancellationToken);

            if (userLevel != null)
                return userLevel;

            userLevel = new UserLevel
            {
                Id = Guid.NewGuid(),
                ProfileId = profile.Id,
                LevelId = profile.LevelId,
                Xp = 0,
                Kp = 0,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _context.UserLevels.AddAsync(userLevel, cancellationToken);

            return userLevel;
        }

        public Task<bool> RewardTransactionExistsAsync(
            Guid profileId,
            RewardSourceType sourceType,
            Guid actionEntityId,
            CancellationToken cancellationToken = default)
        {
            return _context.RewardTransactions
                .AnyAsync(rt =>
                    rt.ProfileId == profileId &&
                    rt.SourceType == sourceType &&
                    rt.ActionEntityId == actionEntityId &&
                    !rt.IsDeleted,
                    cancellationToken);
        }

        public async Task AddRewardTransactionAsync(
            RewardTransaction rewardTransaction,
            CancellationToken cancellationToken = default)
        {
            await _context.RewardTransactions.AddAsync(rewardTransaction, cancellationToken);
        }

        public async Task<IReadOnlyList<Badge>> GetActiveBadgesNotOwnedByProfileAsync(
            Guid profileId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Badges
                .AsNoTracking()
                .Where(b =>
                    !b.IsDeleted &&
                    !string.IsNullOrWhiteSpace(b.Requirement) &&
                    !b.UserBadges.Any(ub =>
                        ub.ProfileId == profileId &&
                        !ub.IsDeleted))
                .ToListAsync(cancellationToken);
        }

        public Task<int> GetCompletedCountAsync(
            Guid profileId,
            RewardSourceType? sourceType,
            Guid? sourceEntityId,
            CancellationToken cancellationToken = default)
        {
            var query = _context.RewardTransactions
                .AsNoTracking()
                .Where(rt => rt.ProfileId == profileId && !rt.IsDeleted);

            if (sourceType.HasValue)
            {
                query = query.Where(rt => rt.SourceType == sourceType.Value);
            }

            if (sourceEntityId.HasValue)
            {
                query = query.Where(rt => rt.SourceEntityId == sourceEntityId.Value);
            }

            return query.CountAsync(cancellationToken);
        }

        public async Task AddUserBadgeAsync(
            UserBadge userBadge,
            CancellationToken cancellationToken = default)
        {
            await _context.UserBadges.AddAsync(userBadge, cancellationToken);
        }
    }
}
