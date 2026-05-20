using GivingChampion.Application.DTO.Mission;
using GivingChampion.Application.DTO.Rewards;
using GivingChampion.Application.DTO.ActivityDto;
using GivingChampion.Application.Interfaces;
using GivingChampion.Application.Interfaces.Reward;
using GivingChampion.Application.Services;
using GivingChampion.Common.Enums;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Domain.Entities.Base;
using GivingChampion.Persistance.Interfaces;
using GivingChampion.Persistance.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using ActivityHistoryDto = GivingChampion.Application.DTO.GivingChampion.Application.DTO.ActivityDto.ActivityDto;

namespace GivingChampion.API.Tests.Services;

public class RewardSystemServiceTests
{
    [Fact]
    public async Task RewardUserMissionCompletedAsync_WhenRewardDataMissing_DoesNothing()
    {
        var repository = new FakeRewardSystemRepository();
        var parser = new FakeBadgeRequirementParser();
        var unitOfWork = new FakeUnitOfWork();
        var service = new RewardSystemService(
            repository,
            parser,
            new FakeVolunteerRepository(),
            new FakeCertificateRepository(),
            unitOfWork);

        await service.RewardUserMissionCompletedAsync(Guid.NewGuid());

        Assert.Empty(repository.AddedRewardTransactions);
        Assert.Empty(repository.AddedUserBadges);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task RewardUserMissionCompletedAsync_WhenAlreadyRewarded_DoesNotDuplicateTransaction()
    {
        var userId = Guid.NewGuid();
        var profileId = Guid.NewGuid();
        var repository = new FakeRewardSystemRepository
        {
            UserMissionRewardData = new RewardActionData
            {
                UserId = userId,
                SourceType = RewardSourceType.Mission,
                SourceEntityId = Guid.NewGuid(),
                ActionEntityId = Guid.NewGuid(),
                XPReward = 20,
                KPReward = 10,
                ImpactReward = 5,
                Reason = "Completed mission"
            },
            ProfileByUserId = new Profile
            {
                Id = profileId,
                UserId = userId,
                Impact = 7,
                UserLevel = new UserLevel
                {
                    Id = Guid.NewGuid(),
                    ProfileId = profileId,
                    Xp = 100,
                    Kp = 50
                }
            },
            RewardTransactionExists = true
        };

        var service = new RewardSystemService(
            repository,
            new FakeBadgeRequirementParser(),
            new FakeVolunteerRepository(),
            new FakeCertificateRepository(),
            new FakeUnitOfWork());

        await service.RewardUserMissionCompletedAsync(Guid.NewGuid());

        Assert.Empty(repository.AddedRewardTransactions);
        Assert.Empty(repository.AddedUserBadges);
        Assert.Equal(100, repository.ProfileByUserId!.UserLevel!.Xp);
        Assert.Equal(50, repository.ProfileByUserId.UserLevel.Kp);
        Assert.Equal(7, repository.ProfileByUserId.Impact);
    }

    [Fact]
    public async Task RewardUserMissionCompletedAsync_AddsTransaction_UpdatesStats_AndAwardsEligibleBadge()
    {
        var userId = Guid.NewGuid();
        var profileId = Guid.NewGuid();
        var badgeId = Guid.NewGuid();
        var userLevel = new UserLevel
        {
            Id = Guid.NewGuid(),
            ProfileId = profileId,
            Xp = 5,
            Kp = 3
        };

        var profile = new Profile
        {
            Id = profileId,
            UserId = userId,
            Impact = 2,
            UserLevel = userLevel
        };

        var repository = new FakeRewardSystemRepository
        {
            UserMissionRewardData = new RewardActionData
            {
                UserId = userId,
                SourceType = RewardSourceType.Mission,
                SourceEntityId = Guid.NewGuid(),
                ActionEntityId = Guid.NewGuid(),
                XPReward = 10,
                KPReward = 4,
                ImpactReward = 6,
                Reason = "Completed mission"
            },
            ProfileByUserId = profile,
            ProfileById = profile,
            UserLevelToReturn = userLevel,
            ActiveBadges = new List<Badge>
            {
                new()
                {
                    Id = badgeId,
                    Name = "XP Starter",
                    Category = "Progress",
                    Requirement = "xp >= 10"
                }
            }
        };

        var parser = new FakeBadgeRequirementParser
        {
            Conditions = new List<ParsedBadgeCondition>
            {
                new()
                {
                    Metric = BadgeConditionMetric.XP,
                    Operator = BadgeComparisonOperator.AtLeast,
                    TargetValue = 10
                }
            }
        };
        var unitOfWork = new FakeUnitOfWork();
        var service = new RewardSystemService(
            repository,
            parser,
            new FakeVolunteerRepository(),
            new FakeCertificateRepository(),
            unitOfWork);

        await service.RewardUserMissionCompletedAsync(Guid.NewGuid());

        Assert.Equal(15, userLevel.Xp);
        Assert.Equal(7, userLevel.Kp);
        Assert.Equal(8, profile.Impact);

        var transaction = Assert.Single(repository.AddedRewardTransactions);
        Assert.Equal(profileId, transaction.ProfileId);
        Assert.Equal(repository.UserMissionRewardData!.SourceType, transaction.SourceType);
        Assert.Equal(repository.UserMissionRewardData.SourceEntityId, transaction.SourceEntityId);
        Assert.Equal(repository.UserMissionRewardData.ActionEntityId, transaction.ActionEntityId);
        Assert.Equal(10, transaction.XPAmount);
        Assert.Equal(4, transaction.KPAmount);
        Assert.Equal(6, transaction.ImpactAmount);
        Assert.Equal("Completed mission", transaction.Reason);

        var awardedBadge = Assert.Single(repository.AddedUserBadges);
        Assert.Equal(profileId, awardedBadge.ProfileId);
        Assert.Equal(badgeId, awardedBadge.BadgeId);
        Assert.Equal(2, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task RewardServiceRequestCompletedAsync_RewardsAllEligibleVolunteerOrders_AndCreatesCertificates()
    {
        var volunteerAUserId = Guid.NewGuid();
        var volunteerBUserId = Guid.NewGuid();
        var volunteerAId = Guid.NewGuid();
        var volunteerBId = Guid.NewGuid();
        var profileAId = Guid.NewGuid();
        var profileBId = Guid.NewGuid();
        var serviceRequestId = Guid.NewGuid();

        var repository = new FakeRewardSystemRepository
        {
            ServiceRequestRewardData = new List<RewardActionData>
            {
                new()
                {
                    UserId = volunteerAUserId,
                    SourceType = RewardSourceType.Service,
                    SourceEntityId = serviceRequestId,
                    ActionEntityId = Guid.NewGuid(),
                    XPReward = 5,
                    KPReward = 3,
                    ImpactReward = 2,
                    Reason = "Completed service request: Library support",
                    CompletionTitle = "Library support",
                    CertificateHours = 4
                },
                new()
                {
                    UserId = volunteerBUserId,
                    SourceType = RewardSourceType.Service,
                    SourceEntityId = serviceRequestId,
                    ActionEntityId = Guid.NewGuid(),
                    XPReward = 5,
                    KPReward = 3,
                    ImpactReward = 2,
                    Reason = "Completed service request: Library support",
                    CompletionTitle = "Library support",
                    CertificateHours = 4
                }
            }
        };

        repository.ProfilesByUserId[volunteerAUserId] = new Profile
        {
            Id = profileAId,
            UserId = volunteerAUserId,
            Impact = 0,
            UserLevel = new UserLevel { Id = Guid.NewGuid(), ProfileId = profileAId, Xp = 0, Kp = 0 }
        };
        repository.ProfilesByUserId[volunteerBUserId] = new Profile
        {
            Id = profileBId,
            UserId = volunteerBUserId,
            Impact = 1,
            UserLevel = new UserLevel { Id = Guid.NewGuid(), ProfileId = profileBId, Xp = 2, Kp = 1 }
        };
        repository.ProfilesById[profileAId] = repository.ProfilesByUserId[volunteerAUserId]!;
        repository.ProfilesById[profileBId] = repository.ProfilesByUserId[volunteerBUserId]!;

        var volunteerRepository = new FakeVolunteerRepository();
        volunteerRepository.VolunteersByUserId[volunteerAUserId] = new Volunteer { Id = volunteerAId, UserId = volunteerAUserId };
        volunteerRepository.VolunteersByUserId[volunteerBUserId] = new Volunteer { Id = volunteerBId, UserId = volunteerBUserId };

        var certificateRepository = new FakeCertificateRepository();
        var unitOfWork = new FakeUnitOfWork();
        var service = new RewardSystemService(
            repository,
            new FakeBadgeRequirementParser(),
            volunteerRepository,
            certificateRepository,
            unitOfWork);

        await service.RewardServiceRequestCompletedAsync(serviceRequestId);

        Assert.Equal(2, repository.AddedRewardTransactions.Count);
        Assert.Equal(2, certificateRepository.AddedCertificates.Count);
        Assert.Contains(certificateRepository.AddedCertificates, certificate => certificate.IssuedTo == volunteerAId);
        Assert.Contains(certificateRepository.AddedCertificates, certificate => certificate.IssuedTo == volunteerBId);
    }

    [Fact]
    public async Task UserMissionService_UpdateProgressAsync_WhenMissionCompletes_TriggersRewardSystem()
    {
        var userId = Guid.NewGuid();
        var userMissionId = Guid.NewGuid();
        var missionId = Guid.NewGuid();
        var userMission = new UserMission
        {
            Id = userMissionId,
            UserId = userId,
            MissionId = missionId,
            Progress = 90,
            Status = MissionStatus.InProgress,
            Mission = new Mission
            {
                Id = missionId,
                Title = "Deliver supplies",
                KPReward = 7,
                XPReward = 12
            }
        };

        var repository = new FakeUserMissionRepository
        {
            UserMission = userMission
        };
        var rewardService = new FakeRewardSystemService();
        var service = new UserMissionService(
            repository,
            new FakeMissionRepository(),
            new FakeActivityService(),
            rewardService,
            new FakeUnitOfWork(),
            CreateMapper(),
            CreateHttpContextAccessor(userId));

        var result = await service.UpdateProgressAsync(
            userMissionId,
            new UpdateProgressDto { Progress = 100 });

        Assert.True(result.Succeeded);
        Assert.Equal(MissionStatus.Completed, userMission.Status);
        Assert.NotEqual(default, userMission.CompletedAt);
        Assert.Equal(new[] { userMissionId }, rewardService.RewardedUserMissionIds);
        Assert.Same(userMission, repository.UpdatedUserMission);
    }

    private static AutoMapper.IMapper CreateMapper()
    {
        var config = new AutoMapper.MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<UserMission, UserMissionDto>()
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                    .ForMember(dest => dest.KPReward, opt => opt.MapFrom(src => src.Mission.KPReward))
                    .ForMember(dest => dest.XPReward, opt => opt.MapFrom(src => src.Mission.XPReward));
            },
            NullLoggerFactory.Instance);

        return config.CreateMapper();
    }

    private static IHttpContextAccessor CreateHttpContextAccessor(Guid userId)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.User = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity(
                new[]
                {
                    new System.Security.Claims.Claim(
                        System.Security.Claims.ClaimTypes.NameIdentifier,
                        userId.ToString())
                },
                "Test"));

        return new HttpContextAccessor
        {
            HttpContext = httpContext
        };
    }

    private sealed class FakeRewardSystemRepository : IRewardSystemRepository
    {
        public RewardActionData? UserMissionRewardData { get; set; }
        public IReadOnlyList<RewardActionData> ServiceRequestRewardData { get; set; } = [];
        public Profile? ProfileByUserId { get; set; }
        public Profile? ProfileById { get; set; }
        public UserLevel? UserLevelToReturn { get; set; }
        public bool RewardTransactionExists { get; set; }
        public IReadOnlyList<Badge> ActiveBadges { get; set; } = [];
        public List<RewardTransaction> AddedRewardTransactions { get; } = [];
        public List<UserBadge> AddedUserBadges { get; } = [];
        public Dictionary<Guid, Profile?> ProfilesByUserId { get; } = [];
        public Dictionary<Guid, Profile?> ProfilesById { get; } = [];

        public Task<IReadOnlyList<RewardActionData>> GetServiceRequestRewardDataAsync(Guid serviceRequestId, CancellationToken cancellationToken = default)
            => Task.FromResult(ServiceRequestRewardData);

        public Task<RewardActionData?> GetVolunteerOrderRewardDataAsync(Guid volunteerOrderId, CancellationToken cancellationToken = default)
            => Task.FromResult<RewardActionData?>(null);

        public Task<RewardActionData?> GetDonationOrderRewardDataAsync(Guid donationOrderId, CancellationToken cancellationToken = default)
            => Task.FromResult<RewardActionData?>(null);

        public Task<RewardActionData?> GetUserMissionRewardDataAsync(Guid userMissionId, CancellationToken cancellationToken = default)
            => Task.FromResult(UserMissionRewardData);

        public Task<RewardActionData?> GetUserGeoQuestRewardDataAsync(Guid userGeoQuestId, CancellationToken cancellationToken = default)
            => Task.FromResult<RewardActionData?>(null);

        public Task<Profile?> GetProfileWithUserLevelByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => Task.FromResult(ProfilesByUserId.TryGetValue(userId, out var profile) ? profile : ProfileByUserId);

        public Task<Profile?> GetProfileWithUserLevelByIdAsync(Guid profileId, CancellationToken cancellationToken = default)
            => Task.FromResult(ProfilesById.TryGetValue(profileId, out var profile) ? profile : ProfileById);

        public Task<UserLevel> GetOrCreateUserLevelAsync(Profile profile, CancellationToken cancellationToken = default)
            => Task.FromResult(UserLevelToReturn ?? profile.UserLevel!);

        public Task<bool> RewardTransactionExistsAsync(Guid profileId, RewardSourceType sourceType, Guid actionEntityId, CancellationToken cancellationToken = default)
            => Task.FromResult(RewardTransactionExists);

        public Task AddRewardTransactionAsync(RewardTransaction rewardTransaction, CancellationToken cancellationToken = default)
        {
            AddedRewardTransactions.Add(rewardTransaction);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<Badge>> GetActiveBadgesNotOwnedByProfileAsync(Guid profileId, CancellationToken cancellationToken = default)
            => Task.FromResult(ActiveBadges);

        public Task<int> GetCompletedCountAsync(Guid profileId, RewardSourceType? sourceType, Guid? sourceEntityId, CancellationToken cancellationToken = default)
            => Task.FromResult(0);

        public Task AddUserBadgeAsync(UserBadge userBadge, CancellationToken cancellationToken = default)
        {
            AddedUserBadges.Add(userBadge);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeVolunteerRepository : IVolunteerRepository
    {
        public Dictionary<Guid, Volunteer?> VolunteersByUserId { get; } = [];

        public Task<PagedList<Volunteer>> GetAllAsync(PageParameters pageParameters) => throw new NotSupportedException();

        public Task<Volunteer?> GetByIdAsync(Guid id) => throw new NotSupportedException();

        public Task<Volunteer?> GetByUserIdAsync(Guid userId)
            => Task.FromResult(VolunteersByUserId.TryGetValue(userId, out var volunteer) ? volunteer : null);

        public void Update(Volunteer volunteer) => throw new NotSupportedException();

        public Task AddAsync(Guid userId) => throw new NotSupportedException();
    }

    private sealed class FakeCertificateRepository : ICertificateRepository
    {
        public HashSet<string> ExistingQrCodes { get; } = [];
        public List<Certificate> AddedCertificates { get; } = [];

        public Task<PagedList<Certificate>> GetCertificateByIdAsync(Guid volunteerId, PageParameters pageParameters, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<PagedList<Certificate>> GetAllAsync(PageParameters pageParameters, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<bool> VolunteerExistsAsync(Guid volunteerId, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task<bool> ExistsByQrCodeAsync(string qrCode, CancellationToken cancellationToken = default)
            => Task.FromResult(ExistingQrCodes.Contains(qrCode));

        public Task AddAsync(Certificate certificate, CancellationToken cancellationToken = default)
        {
            AddedCertificates.Add(certificate);
            ExistingQrCodes.Add(certificate.QrCode);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeBadgeRequirementParser : IBadgeRequirementParser
    {
        public IReadOnlyList<ParsedBadgeCondition> Conditions { get; set; } = [];

        public IReadOnlyList<ParsedBadgeCondition> Parse(string requirement) => Conditions;
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
            => throw new NotSupportedException();

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            return Task.FromResult(1);
        }

        public void Dispose()
        {
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    private sealed class FakeRewardSystemService : IRewardSystemService
    {
        public List<Guid> RewardedUserMissionIds { get; } = [];
        public List<Guid> RewardedServiceRequestIds { get; } = [];

        public Task RewardServiceRequestCompletedAsync(Guid serviceRequestId, CancellationToken cancellationToken = default)
        {
            RewardedServiceRequestIds.Add(serviceRequestId);
            return Task.CompletedTask;
        }

        public Task RewardVolunteerOrderCompletedAsync(Guid volunteerOrderId, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task RewardDonationOrderCompletedAsync(Guid donationOrderId, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task RewardUserMissionCompletedAsync(Guid userMissionId, CancellationToken cancellationToken = default)
        {
            RewardedUserMissionIds.Add(userMissionId);
            return Task.CompletedTask;
        }

        public Task RewardUserGeoQuestCompletedAsync(Guid userGeoQuestId, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private sealed class FakeUserMissionRepository : IUserMissionRepository
    {
        public UserMission? UserMission { get; set; }
        public UserMission? UpdatedUserMission { get; private set; }

        public Task<UserMission?> GetByIdAsync(Guid id) => Task.FromResult(UserMission);

        public Task<UserMission?> GetByUserAndMissionAsync(Guid userId, Guid missionId)
            => throw new NotSupportedException();

        public Task<PagedList<UserMission>> GetByUserIdAsync(PageParameters pageParameters, Guid userId, MissionStatus status = MissionStatus.InProgress)
            => throw new NotSupportedException();

        public Task<PagedList<UserMission>> GetActiveByUserIdAsync(PageParameters pageParameters, Guid userId)
            => throw new NotSupportedException();

        public Task CreateAsync(UserMission userMission) => throw new NotSupportedException();

        public Task UpdateAsync(UserMission userMission)
        {
            UpdatedUserMission = userMission;
            return Task.CompletedTask;
        }

        public Task SoftDeleteAsync(Guid userMissionId) => throw new NotSupportedException();

        public Task<bool> IsMissionStartedAsync(Guid userId, Guid missionId) => throw new NotSupportedException();

        public Task<bool> IsMissionCompletedAsync(Guid userId, Guid missionId) => throw new NotSupportedException();
    }

    private sealed class FakeMissionRepository : IMissionRepository
    {
        public Task<Mission?> GetByIdAsync(Guid id) => throw new NotSupportedException();

        public Task<PagedList<Mission>> GetAllOpenAsync(PageParameters pageParameters) => throw new NotSupportedException();

        public Task<PagedList<Mission>> GetByDifficultyAsync(DifficultyLevel difficulty, PageParameters pageParameters)
            => throw new NotSupportedException();

        public Task<PagedList<Mission>> GetAvailableForLevelAsync(int userLevel, PageParameters pageParameters)
            => throw new NotSupportedException();

        public Task CreateAsync(Mission mission) => throw new NotSupportedException();

        public Task UpdateAsync(Mission mission) => throw new NotSupportedException();

        public Task SoftDeleteAsync(Guid missionId) => throw new NotSupportedException();
    }

    private sealed class FakeActivityService : IActivityService
    {
        public Task<Result<PagedList<ActivityHistoryDto>>> GetEntityHistory(Guid entityId, PageParameters pageParameters)
            => throw new NotSupportedException();

        public Task AddAsync(CreateActivityDto createActivityDto) => Task.CompletedTask;
    }
}
