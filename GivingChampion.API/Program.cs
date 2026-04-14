using GivingChampion.API.Handlers;
using GivingChampion.Application.Mapper;
using GivingChampion.Domain.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
var _conf = builder.Configuration;
// Add services to the container.

// for testing
//builder.AddNpgsqlDbContext<AppDbContext>("givingchampion");

var connectionstring = _conf.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
// for live
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionstring, b => b.MigrationsAssembly("GivingChampion.Domain")));


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => _conf.Bind("JwtSettings", options));


builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());



builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi("v1");
//builder.Services.AddOpenApi("v2");

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.MapDefaultEndpoints();

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
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
