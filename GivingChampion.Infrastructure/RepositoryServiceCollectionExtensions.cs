using GivingChampion.API.Repositories;
using GivingChampion.Application.Interfaces;
using GivingChampion.Infrastructure.Persistence.Repositories;
using GivingChampion.Persistance.Interfaces;
using GivingChampion.Persistance.Repositories;
using GivingChampion.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Persistance;

public static class RepositoryServiceCollectionExtensions
{
    public static IServiceCollection AddGivingChampionRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDonorRepository, DonorRepository>();
        services.AddScoped<IChildRepository, EfChildRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IMissionRepository, MissionRepository>();
        services.AddScoped<IUserMissionRepository, UserMissionRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
        services.AddScoped<IVolunteerOrderRepository, VolunteerOrderRepository>();
        services.AddScoped<IPartnerRepository, PartnerRepository>();
        services.AddScoped<IVolunteerRepository, VolunteerRepository>();
        services.AddScoped<ICertificateRepository, CertificateRepository>();
        services.AddScoped<IDonationRequestRepository, DonationRequestRepository>();
        services.AddScoped<IDonationOrderRepository, DonationOrderRepository>();

        services.AddScoped<IAvatarRepository, AvatarRepository>();
        services.AddScoped<IAdministratorRepository, AdministratorRepository>();
        services.AddScoped<IAiAvatarRepository, AiAvatarRepository>();
        services.AddScoped<ILevelRepository, LevelRepository>();
        services.AddScoped<IBadgeRepository, BadgeRepository>();
        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.AddScoped<IUserBadgeRepository, UserBadgeRepository>();
        services.AddScoped<IUserLevelRepository, UserLevelRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();

        services.AddScoped<IGeoQuestRepository, GeoQuestRepository>();
        services.AddScoped<IUserGeoQuestRepository, UserGeoQuestRepository>();

        services.AddScoped<IVolunteerHistoryRepository, VolunteerHistoryRepository>();

        return services;
    }
}
