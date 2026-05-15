using FluentValidation;
using FluentValidation.AspNetCore;
using GivingChampion.API.Handlers;
using GivingChampion.Application;
using GivingChampion.Application.Auth.Interfaces;
using GivingChampion.Application.DTO.Auth;
using GivingChampion.Application.Mapper;
using GivingChampion.Application.Transformers;
using GivingChampion.Application.Validators.Auth;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance;
using GivingChampion.Persistance.Interfaces;
using GivingChampion.Persistance.Repositories;
using GivingChampion.Persistence.Contexts;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Text;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
var _conf = builder.Configuration;
var _env = builder.Environment;


var jwtOptions = builder.Configuration
    .GetSection(JwtOptions.SectionName)
    .Get<JwtOptions>()
    ?? throw new InvalidOperationException("Jwt configuration is missing.");

var rawCorsValue = builder.Configuration["Cors:AllowedOrigins"];

var allowedOrigins = !string.IsNullOrWhiteSpace(rawCorsValue)
    ? rawCorsValue.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    : builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
      ?? new[] { "http://localhost:5173" };

Console.WriteLine($"RAW Cors:AllowedOrigins = {rawCorsValue}");
Console.WriteLine($"Parsed allowed origins = {string.Join(" | ", allowedOrigins)}");

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
                npgsql.MigrationsAssembly("GivingChampion.Persistance"));
        });
}
else
{
    var connectionString = _conf.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString, npgsql =>
            npgsql.MigrationsAssembly("GivingChampion.Persistance")));
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

builder.Services.AddHttpContextAccessor();
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
builder.Services.AddHttpContextAccessor();

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
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
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
