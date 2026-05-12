using AutoMapper;
using GivingChampion.Application.DTO.BadgeDto;
using GivingChampion.Application.DTO.LevelDto;
using GivingChampion.Application.DTO.UserBadge;
using GivingChampion.Application.DTO.UserLevelDto;
using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;
using GivingChampion.Domain.Entities.Base;
using GivingChampion.Persistance.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using System.Linq.Expressions;
using DomainProfile = GivingChampion.Domain.Entities.Profile;

namespace GivingChampion.API.Tests.Infrastructure;

internal static class ProfileServiceTestMapper
{
    public static IMapper Create()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Badge, BadgeDto>().ReverseMap();
            cfg.CreateMap<CreateBadgeDto, Badge>();
            cfg.CreateMap<UpdateBadgeDto, Badge>();

            cfg.CreateMap<Level, LevelDto>().ReverseMap();
            cfg.CreateMap<CreateLevelDto, Level>();
            cfg.CreateMap<UpdateLevelDto, Level>();

            cfg.CreateMap<UserBadge, UserBadgeDto>()
                .ForMember(dest => dest.BadgeName, opt => opt.MapFrom(src => src.Badge != null ? src.Badge.Name : null))
                .ForMember(dest => dest.BadgeCategory, opt => opt.MapFrom(src => src.Badge != null ? src.Badge.Category : null));
            cfg.CreateMap<CreateUserBadgeDto, UserBadge>();
            cfg.CreateMap<UpdateUserBadgeDto, UserBadge>();

            cfg.CreateMap<UserLevel, UserLevelDto>()
                .ForMember(dest => dest.LevelId, opt => opt.MapFrom(src => src.LevelId))
                .ForMember(dest => dest.LevelNumber, opt => opt.MapFrom(src => src.Level != null ? src.Level.Number : (int?)null))
                .ForMember(dest => dest.LevelMaxXp, opt => opt.MapFrom(src => src.Level != null ? src.Level.MaxXp : (int?)null));
            cfg.CreateMap<CreateUserLevelDto, UserLevel>();
            cfg.CreateMap<UpdateUserLevelDto, UserLevel>();
        }, NullLoggerFactory.Instance);

        return config.CreateMapper();
    }
}

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    private readonly Dictionary<Type, object> _repositories = [];

    public int SaveChangesCallCount { get; private set; }

    public void RegisterRepository<TEntity>(IGenericRepository<TEntity> repository)
        where TEntity : BaseEntity
    {
        _repositories[typeof(TEntity)] = repository;
    }

    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
    {
        if (_repositories.TryGetValue(typeof(TEntity), out var repository))
            return (IGenericRepository<TEntity>)repository;

        throw new NotSupportedException($"Repository for {typeof(TEntity).Name} is not registered.");
    }

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

internal sealed class FakeGenericRepository<TEntity> : IGenericRepository<TEntity>
    where TEntity : BaseEntity
{
    public PagedList<TEntity> PagedEntities { get; set; } = new([], 1, 20, 0);
    public TEntity? EntityById { get; set; }
    public TEntity? FirstOrDefaultEntity { get; set; }
    public List<TEntity> AddedEntities { get; } = [];
    public TEntity? UpdatedEntity { get; private set; }

    public IQueryable<TEntity> Query(bool asNoTracking = true) => throw new NotSupportedException();

    public Task<PagedList<TEntity>> GetAllAsync(PageParameters pageParameters, bool asNoTracking = true, CancellationToken cancellationToken = default)
        => Task.FromResult(PagedEntities);

    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(EntityById);

    public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true, CancellationToken cancellationToken = default)
        => Task.FromResult(FirstOrDefaultEntity);

    public Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? predicate = null, bool asNoTracking = true, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        AddedEntities.Add(entity);
        return Task.CompletedTask;
    }

    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public void Update(TEntity entity) => UpdatedEntity = entity;

    public void Remove(TEntity entity) => throw new NotSupportedException();

    public void RemoveRange(IEnumerable<TEntity> entities) => throw new NotSupportedException();
}

internal sealed class FakeBadgeRepository : IBadgeRepository
{
    public PagedList<Badge> PagedBadges { get; set; } = new([], 1, 20, 0);
    public Badge? BadgeById { get; set; }
    public List<Badge> AddedBadges { get; } = [];
    public Badge? UpdatedBadge { get; private set; }

    public Task<PagedList<Badge>> GetAllAsync(PageParameters pageParameters)
        => Task.FromResult(PagedBadges);

    public Task<Badge?> GetByIdAsync(Guid id)
        => Task.FromResult(BadgeById);

    public Task AddAsync(Badge badge)
    {
        AddedBadges.Add(badge);
        return Task.CompletedTask;
    }

    public void Update(Badge badge) => UpdatedBadge = badge;
}

internal sealed class FakeUserBadgeRepository : IUserBadgeRepository
{
    public Guid LastRequestedUserId { get; private set; }
    public List<UserBadge> UserBadgesByUserId { get; set; } = [];
    public UserBadge? UserBadgeById { get; set; }
    public UserBadge? UserBadgeByProfileAndBadge { get; set; }
    public List<UserBadge> AddedUserBadges { get; } = [];
    public UserBadge? UpdatedUserBadge { get; private set; }

    public Task<List<UserBadge>> GetAllByUserIdAsync(Guid userId)
    {
        LastRequestedUserId = userId;
        return Task.FromResult(UserBadgesByUserId);
    }

    public Task<List<UserBadge>> GetAllByProfileIdAsync(Guid profileId)
        => throw new NotSupportedException();

    public Task<UserBadge?> GetByIdAsync(Guid id)
        => Task.FromResult(UserBadgeById);

    public Task<UserBadge?> GetByProfileAndBadgeAsync(Guid profileId, Guid badgeId, bool includeDeleted = false)
        => Task.FromResult(UserBadgeByProfileAndBadge);

    public Task AddAsync(UserBadge userBadge)
    {
        AddedUserBadges.Add(userBadge);
        return Task.CompletedTask;
    }

    public void Update(UserBadge userBadge) => UpdatedUserBadge = userBadge;
}

internal sealed class FakeProfileRepository : IProfileRepository
{
    public DomainProfile? ProfileByUserId { get; set; }

    public Task<List<DomainProfile>> GetAllAsync() => throw new NotSupportedException();

    public Task<DomainProfile?> GetByIdAsync(Guid id) => throw new NotSupportedException();

    public Task<DomainProfile?> GetByUserIdAsync(Guid id) => Task.FromResult(ProfileByUserId);

    public Task<DomainProfile> AddAsync(Guid userId, Guid levelId) => throw new NotSupportedException();

    public Task Update(DomainProfile profile) => throw new NotSupportedException();
}
