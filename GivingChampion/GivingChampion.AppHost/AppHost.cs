using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Protocol;

var builder = DistributedApplication.CreateBuilder(args);

const string ApiComposeServiceName = "givingchampion-api";
const string ApiPublicPort = "5001";
const string ApiContainerPort = "8080";
const string ApiProxyDomain = "champapi.neurix.uk";
const string MigratorImageName = "givingchampion-migrator";
const string SeederImageName = "givingchampion-seeder";
const string ApiImageName = "givingchampion-api";

// Shared image tag for all app services
var imageTag = builder.Configuration["IMAGE_TAG"] ?? "latest";

// Add Docker Compose environment
var compose = builder.AddDockerComposeEnvironment("compose")
    .WithDashboard(dashboard => dashboard.WithHostPort(8090));

var registryEndpoint = builder.AddParameter("registry-endpoint", secret: true);
var registryRepository = builder.AddParameter("registry-repository", secret: true);
var containerRegistry = builder.AddContainerRegistry(
    "private-registry",
    registryEndpoint,
    registryRepository);

// Retrieve secrets from environment variables
var googleClientId = builder.AddParameter("google-client-id", secret: true);
var googleClientSecret = builder.AddParameter("google-client-secret", secret: true);
var jwtKey = builder.AddParameter("jwt-key", secret: true);
var jwtIssuer = builder.AddParameter("jwt-issuer", secret: true);
var jwtAudience = builder.AddParameter("jwt-audience", secret: true);
var jwtAccessTokenMinutes = builder.AddParameter("jwt-access-token-minutes", secret: true);
var corsOrigins = builder.AddParameter("cors-origins", secret: true);
var authCallbackUrl = builder.AddParameter("auth-callback-url", secret: true);

IResourceBuilder<IResourceWithConnectionString> db;

if (builder.Environment.IsDevelopment())
{
    var db_username = builder.AddParameter("username", () => "postgres", secret: true);
    var db_password = builder.AddParameter("password", () => "postgres", secret: true);

    var postgres = builder.AddPostgres("postgres", db_username, db_password)
        .WithPgAdmin()
        .WithDataVolume()
        .PublishAsDockerComposeService((resource, service) =>
        {
            service.Name = "postgres";
            service.Ports.Add("5432:5432");
        });

    db = postgres.AddDatabase("givingchampion");
}
else
{
    db = builder.AddConnectionString("DefaultConnection");
}

// Migrator service configuration
var migrator = builder.AddProject<Projects.GivingChampion_Migrator>("migrator")
    .WithImageTag(imageTag)
    .WithContainerRegistry(containerRegistry)
    .WithRemoteImageName(MigratorImageName)
    .WithRemoteImageTag(imageTag)
    .WithReference(db, "DefaultConnection")
    .WaitFor(db)
    .PublishAsDockerComposeService((resource, service) =>
    {
        service.Name = "migrator";
    });

// Seeder service configuration
var seeder = builder.AddProject<Projects.GivingChampion_Seeder>("seeder")
    .WithImageTag(imageTag)
    .WithContainerRegistry(containerRegistry)
    .WithRemoteImageName(SeederImageName)
    .WithRemoteImageTag(imageTag)
    .WithReference(db, "DefaultConnection")
    .WithReference(migrator)
    .WaitFor(db)
    .WaitForCompletion(migrator)
    .PublishAsDockerComposeService((resource, service) =>
    {
        service.Name = "seeder";
    });

// API service configuration
builder.AddProject<Projects.GivingChampion_API>("api")
    .WithImageTag(imageTag)
    .WithContainerRegistry(containerRegistry)
    .WithRemoteImageName(ApiImageName)
    .WithRemoteImageTag(imageTag)
    .WithReference(db, "DefaultConnection")
    .WithReference(seeder)
    .WithEnvironment("Jwt__Issuer", jwtIssuer)
    .WithEnvironment("Jwt__Audience", jwtAudience)
    .WithEnvironment("Jwt__AccessTokenMinutes", jwtAccessTokenMinutes)
    .WithEnvironment("Authentication__Google__ClientId", googleClientId)
    .WithEnvironment("Authentication__Google__ClientSecret", googleClientSecret)
    .WithEnvironment("Jwt__Key", jwtKey)
    .WithEnvironment("Cors__AllowedOrigins", corsOrigins)
    .WithEnvironment("Frontend__AuthCallbackUrl", authCallbackUrl)
    .WaitFor(db)
    .WaitForCompletion(seeder)
    .PublishAsDockerComposeService((resource, service) =>
    {
        service.Name = ApiComposeServiceName;
        service.Ports.Add($"{ApiPublicPort}:{ApiContainerPort}");
        service.Labels ??= [];
        service.Labels["neurix.proxy.domain"] = ApiProxyDomain;
        service.Labels["neurix.proxy.port"] = ApiPublicPort;
    });

builder.Build().Run();
