using GivingChampion.Common.Enums;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Models;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IRewardSystemRepository
    {
        Task<IReadOnlyList<RewardActionData>> GetServiceRequestRewardDataAsync(
            Guid serviceRequestId,
            CancellationToken cancellationToken = default);

        Task<RewardActionData?> GetVolunteerOrderRewardDataAsync(
            Guid volunteerOrderId,
            CancellationToken cancellationToken = default);

        Task<RewardActionData?> GetDonationOrderRewardDataAsync(
            Guid donationOrderId,
            CancellationToken cancellationToken = default);

        Task<RewardActionData?> GetUserMissionRewardDataAsync(
            Guid userMissionId,
            CancellationToken cancellationToken = default);

        Task<RewardActionData?> GetUserGeoQuestRewardDataAsync(
            Guid userGeoQuestId,
            CancellationToken cancellationToken = default);

        Task<Profile?> GetProfileWithUserLevelByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<Profile?> GetProfileWithUserLevelByIdAsync(
            Guid profileId,
            CancellationToken cancellationToken = default);

        Task<UserLevel> GetOrCreateUserLevelAsync(
            Profile profile,
            CancellationToken cancellationToken = default);

        Task<bool> RewardTransactionExistsAsync(
            Guid profileId,
            RewardSourceType sourceType,
            Guid actionEntityId,
            CancellationToken cancellationToken = default);

        Task AddRewardTransactionAsync(
            RewardTransaction rewardTransaction,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Badge>> GetActiveBadgesNotOwnedByProfileAsync(
            Guid profileId,
            CancellationToken cancellationToken = default);

        Task<int> GetCompletedCountAsync(
            Guid profileId,
            RewardSourceType? sourceType,
            Guid? sourceEntityId,
            CancellationToken cancellationToken = default);

        Task AddUserBadgeAsync(
            UserBadge userBadge,
            CancellationToken cancellationToken = default);
    }
}
