using GivingChampion.Application.DTO.Rewards;
using GivingChampion.Application.Interfaces.Reward;
using GivingChampion.Common.Enums;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using GivingChampion.Persistance.Models;

namespace GivingChampion.Application.Services
{
    public class RewardSystemService : IRewardSystemService
    {
        private readonly IRewardSystemRepository _rewardRepository;
        private readonly IBadgeRequirementParser _badgeRequirementParser;
        private readonly IVolunteerRepository _volunteerRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RewardSystemService(
            IRewardSystemRepository rewardRepository,
            IBadgeRequirementParser badgeRequirementParser,
            IVolunteerRepository volunteerRepository,
            ICertificateRepository certificateRepository,
            IUnitOfWork unitOfWork)
        {
            _rewardRepository = rewardRepository;
            _badgeRequirementParser = badgeRequirementParser;
            _volunteerRepository = volunteerRepository;
            _certificateRepository = certificateRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task RewardServiceRequestCompletedAsync(
            Guid serviceRequestId,
            CancellationToken cancellationToken = default)
        {
            var rewardDataItems = await _rewardRepository
                .GetServiceRequestRewardDataAsync(serviceRequestId, cancellationToken);

            foreach (var rewardData in rewardDataItems)
            {
                await AddRewardAndAwardBadgesAsync(
                    rewardData,
                    cancellationToken,
                    issueCertificate: true);
            }
        }

        public async Task RewardVolunteerOrderCompletedAsync(
            Guid volunteerOrderId,
            CancellationToken cancellationToken = default)
        {
            var rewardData = await _rewardRepository
                .GetVolunteerOrderRewardDataAsync(volunteerOrderId, cancellationToken);

            await AddRewardAndAwardBadgesAsync(
                rewardData,
                cancellationToken,
                issueCertificate: true);
        }

        public async Task RewardDonationOrderCompletedAsync(
            Guid donationOrderId,
            CancellationToken cancellationToken = default)
        {
            var rewardData = await _rewardRepository
                .GetDonationOrderRewardDataAsync(donationOrderId, cancellationToken);

            await AddRewardAndAwardBadgesAsync(rewardData, cancellationToken);
        }

        public async Task RewardUserMissionCompletedAsync(
            Guid userMissionId,
            CancellationToken cancellationToken = default)
        {
            var rewardData = await _rewardRepository
                .GetUserMissionRewardDataAsync(userMissionId, cancellationToken);

            await AddRewardAndAwardBadgesAsync(rewardData, cancellationToken);
        }

        public async Task RewardUserGeoQuestCompletedAsync(
            Guid userGeoQuestId,
            CancellationToken cancellationToken = default)
        {
            var rewardData = await _rewardRepository
                .GetUserGeoQuestRewardDataAsync(userGeoQuestId, cancellationToken);

            await AddRewardAndAwardBadgesAsync(rewardData, cancellationToken);
        }

        private async Task AddRewardAndAwardBadgesAsync(
            RewardActionData? rewardData,
            CancellationToken cancellationToken,
            bool issueCertificate = false)
        {
            if (rewardData == null)
                return;

            var profile = await _rewardRepository
                .GetProfileWithUserLevelByUserIdAsync(rewardData.UserId, cancellationToken);

            if (profile == null)
                return;

            var alreadyRewarded = await _rewardRepository
                .RewardTransactionExistsAsync(
                    profile.Id,
                    rewardData.SourceType,
                    rewardData.ActionEntityId,
                    cancellationToken);

            if (alreadyRewarded)
            {
                if (issueCertificate)
                {
                    await EnsureVolunteerCertificateAsync(rewardData, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                return;
            }

            var userLevel = await _rewardRepository
                .GetOrCreateUserLevelAsync(profile, cancellationToken);

            userLevel.Xp += rewardData.XPReward;
            userLevel.Kp += rewardData.KPReward;
            profile.Impact += rewardData.ImpactReward;

            var rewardTransaction = new RewardTransaction
            {
                Id = Guid.NewGuid(),
                ProfileId = profile.Id,
                SourceType = rewardData.SourceType,
                SourceEntityId = rewardData.SourceEntityId,
                ActionEntityId = rewardData.ActionEntityId,
                XPAmount = rewardData.XPReward,
                KPAmount = rewardData.KPReward,
                ImpactAmount = rewardData.ImpactReward,
                Reason = rewardData.Reason,
                EarnedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _rewardRepository.AddRewardTransactionAsync(
                rewardTransaction,
                cancellationToken);

            if (issueCertificate)
            {
                await EnsureVolunteerCertificateAsync(rewardData, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await AwardEligibleBadgesAsync(profile.Id, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task EnsureVolunteerCertificateAsync(
            RewardActionData rewardData,
            CancellationToken cancellationToken)
        {
            if (rewardData.SourceType != RewardSourceType.Service ||
                string.IsNullOrWhiteSpace(rewardData.CompletionTitle) ||
                rewardData.CertificateHours is null)
            {
                return;
            }

            var volunteer = await _volunteerRepository
                .GetByUserIdAsync(rewardData.UserId);

            if (volunteer == null)
                return;

            var qrCode = BuildVolunteerCertificateQrCode(
                rewardData.SourceEntityId,
                volunteer.Id);

            var exists = await _certificateRepository
                .ExistsByQrCodeAsync(qrCode, cancellationToken);

            if (exists)
                return;

            var certificate = new Certificate
            {
                Id = Guid.NewGuid(),
                Type = $"Volunteer Certificate - {rewardData.CompletionTitle}",
                IssuedDate = DateTime.UtcNow,
                IssuedTo = volunteer.Id,
                TotalHours = Math.Max(1, rewardData.CertificateHours.Value),
                QrCode = qrCode,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _certificateRepository.AddAsync(certificate, cancellationToken);
        }

        private static string BuildVolunteerCertificateQrCode(
            Guid serviceRequestId,
            Guid volunteerId)
            => $"service-request:{serviceRequestId}:volunteer:{volunteerId}";

        private async Task AwardEligibleBadgesAsync(
            Guid profileId,
            CancellationToken cancellationToken)
        {
            var profile = await _rewardRepository
                .GetProfileWithUserLevelByIdAsync(profileId, cancellationToken);

            if (profile == null)
                return;

            var badges = await _rewardRepository
                .GetActiveBadgesNotOwnedByProfileAsync(profileId, cancellationToken);

            foreach (var badge in badges)
            {
                IReadOnlyList<ParsedBadgeCondition> conditions;

                try
                {
                    conditions = _badgeRequirementParser.Parse(badge.Requirement!);
                }
                catch
                {
                    continue;
                }

                if (!conditions.Any())
                    continue;

                var isEligible = true;

                foreach (var condition in conditions)
                {
                    var actualValue = await GetActualConditionValueAsync(
                        profile,
                        condition,
                        cancellationToken);

                    if (!Compare(
                        actualValue,
                        condition.TargetValue,
                        condition.Operator))
                    {
                        isEligible = false;
                        break;
                    }
                }

                if (!isEligible)
                    continue;

                var userBadge = new UserBadge
                {
                    Id = Guid.NewGuid(),
                    ProfileId = profileId,
                    BadgeId = badge.Id,
                    EarnedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _rewardRepository.AddUserBadgeAsync(
                    userBadge,
                    cancellationToken);
            }
        }

        private async Task<int> GetActualConditionValueAsync(
            Profile profile,
            ParsedBadgeCondition condition,
            CancellationToken cancellationToken)
        {
            return condition.Metric switch
            {
                BadgeConditionMetric.XP =>
                    profile.UserLevel?.Xp ?? 0,

                BadgeConditionMetric.KP =>
                    profile.UserLevel?.Kp ?? 0,

                BadgeConditionMetric.Impact =>
                    profile.Impact,

                BadgeConditionMetric.Completed =>
                    await _rewardRepository.GetCompletedCountAsync(
                        profile.Id,
                        condition.SourceType,
                        condition.SourceEntityId,
                        cancellationToken),

                _ => 0
            };
        }

        private static bool Compare(
            int actualValue,
            int targetValue,
            BadgeComparisonOperator comparisonOperator)
        {
            return comparisonOperator switch
            {
                BadgeComparisonOperator.AtLeast => actualValue >= targetValue,
                BadgeComparisonOperator.GreaterThan => actualValue > targetValue,
                BadgeComparisonOperator.Equal => actualValue == targetValue,
                BadgeComparisonOperator.LessThan => actualValue < targetValue,
                BadgeComparisonOperator.AtMost => actualValue <= targetValue,
                _ => false
            };
        }
    }
}
