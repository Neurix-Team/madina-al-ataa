using GivingChampion.API.Interfaces;
using GivingChampion.API.Services;
using GivingChampion.Application;
using GivingChampion.Application.Interfaces;
using GivingChampion.Application.Interfaces.Admin;
using GivingChampion.Application.Interfaces.Auth;
using GivingChampion.Application.Interfaces.Certificate;
using GivingChampion.Application.Interfaces.DonationOrderService;
using GivingChampion.Application.Interfaces.Location;
using GivingChampion.Application.Interfaces.Mission;
using GivingChampion.Application.Interfaces.Partner;
using GivingChampion.Application.Interfaces.Reward;
using GivingChampion.Application.Interfaces.ServiceRequestService;
using GivingChampion.Application.Interfaces.User;
using GivingChampion.Application.Interfaces.Volunteer;
using GivingChampion.Application.Interfaces.VolunteerOrderService;
using GivingChampion.Application.Services;
using GivingChampion.Application.Services.DonationOrderService;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application;

public static class AppServiceDependencyInjection
{
    public static IServiceCollection AddGivingChampionServices(
        this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDonorService, DonorService>();
        services.AddScoped<IChildService, ChildService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IMissionService, MissionService>();
        services.AddScoped<IUserMissionService, UserMissionService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IGeoQuestService, GeoQuestService>();
        services.AddScoped<IUserGeoQuestService, UserGeoQuestService>();

        services.AddScoped<IServiceRequestService, ServiceRequestService>();
        services.AddScoped<IRewardSystemService, RewardSystemService>();
        services.AddScoped<IVolunteerOrderService, VolunteerOrderService>();
        services.AddScoped<IPartnerService, PartnerService>();
        services.AddScoped<IVolunteerService, VolunteerService>();
        services.AddScoped<ICertificateService, CertificateService>();
        services.AddScoped<IDonationRequestService, DonationRequestService>();
        services.AddScoped<IDonationOrderService, DonationOrderService>();

        services.AddScoped<IAvatarService, AvatarService>();
        services.AddScoped<IAdministratorService, AdministratorService>();
        services.AddScoped<IAiAvatarService, AiAvatarService>();
        services.AddScoped<ILevelService, LevelService>();
        services.AddScoped<IBadgeService, BadgeService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IUserBadgeService, UserBadgeService>();
        services.AddScoped<IUserLevelService, UserLevelService>();
        services.AddScoped<IReviewService, ReviewService>();

        services.AddScoped<IActivityService, ActivityService>();

        services.AddScoped<IJwtTokenFactory, JwtTokenFactory>();
        services.AddSingleton<IExternalLoginCodeStore, InMemoryExternalLoginCodeStore>();

        return services;
    }

}
