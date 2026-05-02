using GivingChampion.API.Handlers;
using GivingChampion.API.Interfaces;
using GivingChampion.API.Repositories;
using GivingChampion.API.Services;
using GivingChampion.Application;
using GivingChampion.Application.Auth.Interfaces;
using GivingChampion.Application.Interfaces;
using GivingChampion.Application.Interfaces.Admin;
using GivingChampion.Application.Interfaces.Auth;
using GivingChampion.Application.Interfaces.Certificate;
using GivingChampion.Application.Interfaces.DonationOrderService;
using GivingChampion.Application.Interfaces.Location;
using GivingChampion.Application.Interfaces.Mission;
using GivingChampion.Application.Interfaces.Partner;
using GivingChampion.Application.Interfaces.ServiceRequestService;
using GivingChampion.Application.Interfaces.User;
using GivingChampion.Application.Interfaces.Volunteer;
using GivingChampion.Application.Interfaces.VolunteerHistoryService;
using GivingChampion.Application.Interfaces.VolunteerOrderService;
using GivingChampion.Application.Mapper;
using GivingChampion.Application.Services;
using GivingChampion.Application.Services.Certificate;
using GivingChampion.Application.Services.DonationOrderService;
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
using GivingChampion.Persistance;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

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
    builder.AddNpgsqlDbContext<AppDbContext>(
        "DefaultConnection",
        configureDbContextOptions: options =>
        {
            options.UseNpgsql(npgsql =>
                npgsql.MigrationsAssembly("GivingChampion.Domain"));
        });
}
else
{
    var connectionString = _conf.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString, npgsql =>
            npgsql.MigrationsAssembly("GivingChampion.Domain")));
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


builder.Services.AddGivingChampionRepositories();
builder.Services.AddGivingChampionServices();

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

app.UseExceptionHandler();

app.MapDefaultEndpoints();

app.UseCors("Frontend");

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
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
//}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
