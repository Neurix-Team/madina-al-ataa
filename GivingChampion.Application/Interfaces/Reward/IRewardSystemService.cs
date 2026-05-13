namespace GivingChampion.Application.Interfaces.Reward
{
    public interface IRewardSystemService
    {
        Task RewardVolunteerOrderCompletedAsync(
            Guid volunteerOrderId,
            CancellationToken cancellationToken = default);

        Task RewardDonationOrderCompletedAsync(
            Guid donationOrderId,
            CancellationToken cancellationToken = default);

        Task RewardUserMissionCompletedAsync(
            Guid userMissionId,
            CancellationToken cancellationToken = default);

        Task RewardUserGeoQuestCompletedAsync(
            Guid userGeoQuestId,
            CancellationToken cancellationToken = default);
    }
}
