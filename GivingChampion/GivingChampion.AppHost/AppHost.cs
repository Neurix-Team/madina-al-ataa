using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Protocol;

var builder = DistributedApplication.CreateBuilder(args);

// Add Docker Compose environment
var compose = builder.AddDockerComposeEnvironment("compose")
    .WithDashboard(dashboard => dashboard.WithHostPort(8080));  // Expose dashboard on port 8080

// Retrieve secrets from environment variables (e.g., Google OAuth ClientSecret, JWT Key)
var googleClientId = builder.AddParameter("google-client-id", secret: true);
var googleClientSecret = builder.AddParameter("google-client-secret", secret: true);
var jwtKey = builder.AddParameter("jwt-key", secret: true);

// FIX 1: Change to IResourceBuilder<IResourceWithConnectionString>
// This allows the variable to be properly passed into .WithReference() calls.
IResourceBuilder<IResourceWithConnectionString> db;

if (builder.Environment.IsDevelopment())
{
    // FIX 2: AddParameter default values require a Func<string>, so use () => "postgres"
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
    // Production/External: Use a connection string parameter instead of a container
    // This expects "ConnectionStrings:DefaultConnection" to be in your config/env
    db = builder.AddConnectionString("DefaultConnection");
}

// Migrator service configuration
var migrator = builder.AddProject<Projects.GivingChampion_Migrator>("migrator")
    // FIX 3: Replaced the undefined `defaultConnection` with the `db` variable
    .WithReference(db, "DefaultConnection")
    .WaitFor(db)
    .PublishAsDockerComposeService((resource, service) =>
    {
        service.Name = "migrator";
    });

// Seeder service configuration
var seeder = builder.AddProject<Projects.GivingChampion_Seeder>("seeder")
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
    .WithReference(db, "DefaultConnection")
    .WithReference(seeder)
    .WithEnvironment("Jwt__Issuer", "GivingChampion")
    .WithEnvironment("Jwt__Audience", "GivingChampion.Client")
    .WithEnvironment("Jwt__AccessTokenMinutes", "60")
    .WithEnvironment("Authentication__Google__ClientId", googleClientId)
    .WithEnvironment("Authentication__Google__ClientSecret", googleClientSecret)
    .WithEnvironment("Jwt__Key", jwtKey)
    .WaitFor(db)
    .WaitForCompletion(seeder)
    .PublishAsDockerComposeService((resource, service) =>
    {
        service.Name = "api";
        service.Ports.Add("5000:8080");
    });

// Build and run the application
builder.Build().Run();