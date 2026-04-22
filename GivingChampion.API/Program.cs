using GivingChampion.API.Handlers;
using GivingChampion.API.Interfaces;
using GivingChampion.API.Repositories;
using GivingChampion.API.Services;
using GivingChampion.Application.Auth.Interfaces;
using GivingChampion.Application.Interfaces;
using GivingChampion.Application.Interfaces.Auth;
using GivingChampion.Application.Interfaces.ServiceRequestService;
using GivingChampion.Application.Interfaces.VolunteerOrderService;
using GivingChampion.Application.Interfaces.Location;
using GivingChampion.Application.Interfaces.Mission;
using GivingChampion.Application.Interfaces.User;
using GivingChampion.Application.Mapper;
using GivingChampion.Application.Services;
using GivingChampion.Application.Transformers;
using GivingChampion.Common.DTO.Auth;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Infrastructure.Persistence.Repositories;
using GivingChampion.Persistance.Interfaces;
using GivingChampion.Persistance.Repositories;
using GivingChampion.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using GivingChampion.Application.Interfaces.Partner;
using GivingChampion.Application.Interfaces.Volunteer;
using GivingChampion.Application.Interfaces.Certificate;
using GivingChampion.Application.Services.Certificate;
using GivingChampion.Application.Interfaces.DonationRequest;
using GivingChampion.Application.Interfaces.DonationOrderService;
using GivingChampion.Application.Services.DonationOrderService;
using GivingChampion.API.Interfaces;
using GivingChampion.API.Services;
using GivingChampion.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
var _conf = builder.Configuration;
var _env = builder.Environment;


var jwtOptions = builder.Configuration
    .GetSection(JwtOptions.SectionName)
    .Get<JwtOptions>()
    ?? throw new InvalidOperationException("Jwt configuration is missing.");

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new string[] { "http://localhost:5173" };

// Add services to the container.

if (!builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

if (_env.IsDevelopment())
{
    // for testing
    builder.AddNpgsqlDbContext<AppDbContext>("givingchampion");
}
else
{
    var connectionstring = _conf.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    // for live
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionstring, b => b.MigrationsAssembly("GivingChampion.Domain")));
}

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.User.RequireUniqueEmail = true;

    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
//builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IAvatarRepository, AvatarRepository>();
builder.Services.AddScoped<IAvatarService, AvatarService>();
builder.Services.AddScoped<IAiAvatarRepository, AiAvatarRepository>();
builder.Services.AddScoped<IAiAvatarService, AiAvatarService>();
builder.Services.AddScoped<ILevelRepository, LevelRepository>();
builder.Services.AddScoped<ILevelService, LevelService>();
builder.Services.AddScoped<IBadgeRepository, BadgeRepository>();
builder.Services.AddScoped<IBadgeService, BadgeService>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IUserBadgeRepository, UserBadgeRepository>();
builder.Services.AddScoped<IUserBadgeService, UserBadgeService>();
builder.Services.AddScoped<IUserLevelRepository, UserLevelRepository>();
builder.Services.AddScoped<IUserLevelService, UserLevelService>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IGeoQuestRepository, GeoQuestRepository>();
builder.Services.AddScoped<IUserGeoQuestRepository, UserGeoQuestRepository>();
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.Key)),
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    })
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.SignInScheme = IdentityConstants.ExternalScheme;
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]
            ?? throw new InvalidOperationException("Missing Authentication:Google:ClientId");
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]
            ?? throw new InvalidOperationException("Missing Authentication:Google:ClientSecret");
        options.CallbackPath = "/signin-google";
        options.SaveTokens = true;
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(allowedOrigins) 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IIdentityRepository, IdentityRepository>();


builder.Services.Configure<JwtOptions>(
            _conf.GetSection(JwtOptions.SectionName));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDonorRepository, DonorRepository>();
builder.Services.AddScoped<IDonorService, DonorService>(); 
builder.Services.AddScoped<IChildRepository, EfChildRepository>();
builder.Services.AddScoped<IChildService, ChildService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IMissionRepository, MissionRepository>();
builder.Services.AddScoped<IMissionService, MissionService>();
builder.Services.AddScoped<IUserMissionService, UserMissionService>();
builder.Services.AddScoped<IUserMissionRepository, UserMissionRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IGeoQuestService, GeoQuestService>();
builder.Services.AddScoped<IUserGeoQuestService, UserGeoQuestService>();

builder.Services.AddScoped<IJwtTokenFactory, JwtTokenFactory>();
builder.Services.AddSingleton<IExternalLoginCodeStore, InMemoryExternalLoginCodeStore>();
// Service Request dependencies
builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
builder.Services.AddScoped<IVolunteerOrderService, VolunteerOrderService>();
builder.Services.AddScoped<IVolunteerOrderRepository, VolunteerOrderRepository>();
builder.Services.AddScoped<IPartnerRepository, PartnerRepository>();    
builder.Services.AddScoped<IPartnerService, PartnerService>();
builder.Services.AddScoped<IVolunteerRepository, VolunteerRepository>();
builder.Services.AddScoped<IVolunteerService, VolunteerService>();
builder.Services.AddScoped<ICertificateRepository, CertificateRepository>();
builder.Services.AddScoped<ICertificateService, CertificateService>();
builder.Services.AddScoped<IDonationRequestService,DonationRequestService>();
builder.Services.AddScoped<IDonationRequestRepository, DonationRequestRepository>();
builder.Services.AddScoped<IDonationOrderService, DonationOrderService>();
builder.Services.AddScoped<IDonationOrderRepository, DonationOrderRepository>();

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});
//builder.Services.AddOpenApi("v2");

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseCors("Frontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(option =>
    {
        option.Title = "Giving Champion Game API";

        option
            .AddDocument("v1", "API Version 1.0", "/openapi/v1.json", isDefault: true);
        //.AddDocument("v2", "API Version 2.0", "/openapi/v2.json");
        option.AddPreferredSecuritySchemes("Bearer")
        .AddHttpAuthentication("Bearer", auth =>
        {
            auth.Token = "00000000.00000.0000000";
        }).EnablePersistentAuthentication();
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
