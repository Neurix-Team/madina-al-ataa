using GivingChampion.API.Services;
using GivingChampion.API.Tests.Infrastructure;
using GivingChampion.Application.DTO.BadgeDto;
using GivingChampion.Application.DTO.LevelDto;
using GivingChampion.Application.DTO.UserBadge;
using GivingChampion.Application.DTO.UserLevelDto;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Mapper;
using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using DomainProfile = GivingChampion.Domain.Entities.Profile;

namespace GivingChampion.API.Tests.Services;

public class BadgeServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsMappedPagedBadges()
    {
        var badgeRepository = new FakeBadgeRepository
        {
            PagedBadges = new PagedList<Badge>(
                [
                    new Badge { Id = Guid.NewGuid(), Name = "Starter", Category = "Progress" },
                    new Badge { Id = Guid.NewGuid(), Name = "Helper", Category = "Community" }
                ],
                2,
                10,
                12)
        };
        var unitOfWork = new FakeUnitOfWork();
        var service = new BadgeService(unitOfWork, badgeRepository, ProfileServiceTestMapper.Create());

        var result = await service.GetAllAsync(new PageParameters { PageNumber = 2, PageSize = 10 });

        Assert.True(result.Succeeded);
        Assert.Equal(2, result.Value!.PageNumber);
        Assert.Equal(12, result.Value.TotalCount);
        Assert.Collection(
            result.Value.Items,
            badge => Assert.Equal("Starter", badge.Name),
            badge => Assert.Equal("Helper", badge.Name));
    }

    [Fact]
    public async Task CreateAsync_AddsBadge_AndSavesChanges()
    {
        var badgeRepository = new FakeBadgeRepository();
        var unitOfWork = new FakeUnitOfWork();
        var service = new BadgeService(unitOfWork, badgeRepository, ProfileServiceTestMapper.Create());

        var result = await service.CreateAsync(new CreateBadgeDto
        {
            Name = "Explorer",
            Category = "Adventure",
            Description = "Awarded for exploration",
            Requirement = "Complete a quest"
        });

        Assert.True(result.Succeeded);
        var addedBadge = Assert.Single(badgeRepository.AddedBadges);
        Assert.Equal("Explorer", addedBadge.Name);
        Assert.Equal("Adventure", addedBadge.Category);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task SoftDeleteAsync_MarksBadgeDeleted_AndSavesChanges()
    {
        var badge = new Badge { Id = Guid.NewGuid(), Name = "Explorer", Category = "Adventure" };
        var badgeRepository = new FakeBadgeRepository { BadgeById = badge };
        var unitOfWork = new FakeUnitOfWork();
        var service = new BadgeService(unitOfWork, badgeRepository, ProfileServiceTestMapper.Create());

        var result = await service.SoftDeleteAsync(badge.Id);

        Assert.True(result.Succeeded);
        Assert.True(badge.IsDeleted);
        Assert.NotNull(badge.DeletedAt);
        Assert.Same(badge, badgeRepository.UpdatedBadge);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }
}

public class LevelServiceTests
{
    [Fact]
    public async Task CreateAsync_WithInvalidLevelNumber_ThrowsBadRequest()
    {
        var levelRepository = new FakeGenericRepository<Level>();
        var unitOfWork = new FakeUnitOfWork();
        unitOfWork.RegisterRepository(levelRepository);
        var service = new LevelService(unitOfWork, ProfileServiceTestMapper.Create());

        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            service.CreateAsync(new CreateLevelDto { Number = 0, MaxXp = 100 }));

        Assert.Equal("Level number must be greater than zero.", ex.Message);
        Assert.Empty(levelRepository.AddedEntities);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task GetByIdAsync_WhenLevelIsDeleted_ThrowsNotFound()
    {
        var level = new Level { Id = Guid.NewGuid(), Number = 3, MaxXp = 500, IsDeleted = true };
        var levelRepository = new FakeGenericRepository<Level> { EntityById = level };
        var unitOfWork = new FakeUnitOfWork();
        unitOfWork.RegisterRepository(levelRepository);
        var service = new LevelService(unitOfWork, ProfileServiceTestMapper.Create());

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(level.Id));
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_UpdatesLevel_AndSavesChanges()
    {
        var level = new Level { Id = Guid.NewGuid(), Number = 1, MaxXp = 100 };
        var levelRepository = new FakeGenericRepository<Level> { EntityById = level };
        var unitOfWork = new FakeUnitOfWork();
        unitOfWork.RegisterRepository(levelRepository);
        var service = new LevelService(unitOfWork, ProfileServiceTestMapper.Create());

        var result = await service.UpdateAsync(level.Id, new UpdateLevelDto { Number = 2, MaxXp = 250 });

        Assert.True(result.Succeeded);
        Assert.Equal(2, level.Number);
        Assert.Equal(250, level.MaxXp);
        Assert.Same(level, levelRepository.UpdatedEntity);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }
}

public class UserBadgeServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenActiveAssignmentExists_ThrowsBadRequest()
    {
        var existing = new UserBadge
        {
            Id = Guid.NewGuid(),
            ProfileId = Guid.NewGuid(),
            BadgeId = Guid.NewGuid()
        };
        var repository = new FakeUserBadgeRepository { UserBadgeByProfileAndBadge = existing };
        var profileRepository = new FakeProfileRepository();
        var service = new UserBadgeService(
            new FakeUnitOfWork(),
            repository,
            profileRepository,
            ProfileServiceTestMapper.Create(),
            TestAuthContextFactory.CreateHttpContextAccessor(Guid.NewGuid()));

        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            service.CreateAsync(new CreateUserBadgeDto
            {
                ProfileId = existing.ProfileId,
                BadgeId = existing.BadgeId
            }));

        Assert.Equal("This badge is already assigned to this profile.", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenDeletedAssignmentExists_RestoresExistingRow()
    {
        var deletedAssignment = new UserBadge
        {
            Id = Guid.NewGuid(),
            ProfileId = Guid.NewGuid(),
            BadgeId = Guid.NewGuid(),
            IsDeleted = true,
            DeletedAt = DateTime.UtcNow.AddDays(-1)
        };
        var repository = new FakeUserBadgeRepository { UserBadgeByProfileAndBadge = deletedAssignment };
        var unitOfWork = new FakeUnitOfWork();
        var profileRepository = new FakeProfileRepository();
        var service = new UserBadgeService(
            unitOfWork,
            repository,
            profileRepository,
            ProfileServiceTestMapper.Create(),
            TestAuthContextFactory.CreateHttpContextAccessor(Guid.NewGuid()));

        var result = await service.CreateAsync(new CreateUserBadgeDto
        {
            ProfileId = deletedAssignment.ProfileId,
            BadgeId = deletedAssignment.BadgeId
        });

        Assert.False(deletedAssignment.IsDeleted);
        Assert.Null(deletedAssignment.DeletedAt);
        Assert.Same(deletedAssignment, repository.UpdatedUserBadge);
        Assert.Empty(repository.AddedUserBadges);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
        Assert.Equal(deletedAssignment.Id, result.Id);
    }

    [Fact]
    public async Task GetAllByUserIdAsync_UsesAuthenticatedUserId()
    {
        var userId = Guid.NewGuid();
        var profileId = Guid.NewGuid();
        var repository = new FakeUserBadgeRepository
        {
            UserBadgesByProfileId =
            [
                new UserBadge { Id = Guid.NewGuid(), ProfileId = profileId, BadgeId = Guid.NewGuid() }
            ]
        };
        var profileRepository = new FakeProfileRepository
        {
            ProfileByUserId = new DomainProfile { Id = profileId, UserId = userId }
        };
        var service = new UserBadgeService(
            new FakeUnitOfWork(),
            repository,
            profileRepository,
            ProfileServiceTestMapper.Create(),
            TestAuthContextFactory.CreateHttpContextAccessor(userId));

        var result = await service.GetAllByUserIdAsync();

        Assert.Equal(userId, profileRepository.LastRequestedUserId);
        Assert.Equal(profileId, repository.LastRequestedProfileId);
        Assert.Single(result);
    }
}

public class UserLevelServiceTests
{
    [Fact]
    public async Task GetMyLevelAsync_ReturnsMappedUserLevel()
    {
        var userId = Guid.NewGuid();
        var profileId = Guid.NewGuid();
        var level = new Level { Id = Guid.NewGuid(), Number = 4, MaxXp = 900 };
        var profileRepository = new FakeGenericRepository<DomainProfile>
        {
            FirstOrDefaultEntity = new DomainProfile { Id = profileId, UserId = userId }
        };
        var userLevel = new UserLevel
        {
            Id = Guid.NewGuid(),
            ProfileId = profileId,
            LevelId = level.Id,
            Xp = 120,
            Kp = 35,
            Level = level
        };
        var userLevelRepository = new FakeUserLevelRepository
        {
            UserLevelByProfileId = userLevel
        };
        var unitOfWork = new FakeUnitOfWork();
        unitOfWork.RegisterRepository(profileRepository);
        unitOfWork.RegisterRepository(new FakeGenericRepository<UserLevel>());
        var service = new UserLevelService(
            unitOfWork,
            new FakeProfileRepository(),
            userLevelRepository,
            ProfileServiceTestMapper.Create(),
            TestAuthContextFactory.CreateHttpContextAccessor(userId));

        var result = await service.GetMyLevelAsync();

        Assert.NotNull(result);
        Assert.Equal(profileId, result!.ProfileId);
        Assert.Equal(level.Id, result.LevelId);
        Assert.Equal(120, result.Xp);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithEmptyUserId_ThrowsBadRequest()
    {
        var unitOfWork = new FakeUnitOfWork();
        unitOfWork.RegisterRepository(new FakeGenericRepository<DomainProfile>());
        unitOfWork.RegisterRepository(new FakeGenericRepository<UserLevel>());
        var service = new UserLevelService(
            unitOfWork,
            new FakeProfileRepository(),
            new FakeUserLevelRepository(),
            ProfileServiceTestMapper.Create(),
            TestAuthContextFactory.CreateHttpContextAccessor(Guid.NewGuid()));

        await Assert.ThrowsAsync<BadRequestException>(() => service.GetByUserIdAsync(Guid.Empty));
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_UpdatesUserLevel_AndSavesChanges()
    {
        var levelId = Guid.NewGuid();
        var userLevel = new UserLevel
        {
            Id = Guid.NewGuid(),
            ProfileId = Guid.NewGuid(),
            LevelId = Guid.NewGuid(),
            Xp = 10,
            Kp = 5,
            Level = new Level { Id = Guid.NewGuid(), Number = 1, MaxXp = 100 }
        };
        var userLevelRepository = new FakeGenericRepository<UserLevel> { EntityById = userLevel };
        var unitOfWork = new FakeUnitOfWork();
        unitOfWork.RegisterRepository(new FakeGenericRepository<DomainProfile>());
        unitOfWork.RegisterRepository(userLevelRepository);
        var customUserLevelRepository = new FakeUserLevelRepository();
        var service = new UserLevelService(
            unitOfWork,
            new FakeProfileRepository(),
            customUserLevelRepository,
            ProfileServiceTestMapper.Create(),
            TestAuthContextFactory.CreateHttpContextAccessor(Guid.NewGuid()));

        var result = await service.UpdateAsync(userLevel.Id, new UpdateUserLevelDto
        {
            LevelId = levelId,
            Xp = 75,
            Kp = 20
        });

        Assert.True(result);
        Assert.Equal(levelId, userLevel.LevelId);
        Assert.Equal(75, userLevel.Xp);
        Assert.Equal(20, userLevel.Kp);
        Assert.Same(userLevel, userLevelRepository.UpdatedEntity);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }
}

public class ProfileMappingTests
{
    [Fact]
    public void ProfileToDto_MapsUserFields()
    {
        var mapper = new AutoMapper.MapperConfiguration(
            cfg => cfg.AddProfile<MappingProfile>(),
            NullLoggerFactory.Instance).CreateMapper();

        var birthDate = new DateTime(1998, 4, 12, 0, 0, 0, DateTimeKind.Utc);
        var profile = new DomainProfile
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Impact = 42,
            Rating = 4.5,
            User = new ApplicationUser
            {
                Email = "user@example.com",
                FullName = "Test User",
                BirthDay = birthDate
            }
        };

        var dto = mapper.Map<GivingChampion.Application.DTO.ProfileDto.ProfileDto>(profile);

        Assert.Equal("user@example.com", dto.Email);
        Assert.Equal("Test User", dto.FullName);
        Assert.Equal(birthDate, dto.BirthDate);
    }
}
