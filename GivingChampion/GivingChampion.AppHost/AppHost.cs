var builder = DistributedApplication.CreateBuilder(args);

// Retrieve secrets from environment variables (e.g., Google OAuth ClientSecret, JWT Key, DefaultConnection)
var googleClientSecret = builder.AddParameter("google-client-secret", secret: true);
var jwtKey = builder.AddParameter("jwt-key", secret: true);
var defaultConnection = builder.AddConnectionString("DefaultConnection");

// Add Docker Compose environment
var compose = builder.AddDockerComposeEnvironment("compose")
    .WithDashboard(dashboard => dashboard.WithHostPort(8080));  // Expose dashboard on port 8080

// Database parameters (PostgreSQL)
var db_username = builder.AddParameter("username", "postgres", secret: true);
var db_password = builder.AddParameter("password", "postgres", secret: true);

// Postgres service configuration
var postgres = builder.AddPostgres("postgres", db_username, db_password, 5432)
    .WithPgAdmin()
    .PublishAsDockerComposeService((resource, service) =>
    {
        service.Name = "postgres";
        service.Ports.Add("5432:5432");  // Expose Postgres on port 5432
    });
    .WithPgAdmin()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

// Create the database in Postgres
var db = postgres.AddDatabase("givingchampion");

// Migrator service configuration
var migrator = builder.AddProject<Projects.GivingChampion_Migrator>("migrator")
    .WithReference(defaultConnection, "DefaultConnection")
    .WithReference(db)
    .WaitFor(db)
    .PublishAsDockerComposeService((resource, service) =>
    {
        service.Name = "migrator";
    });

// Seeder service configuration
var seeder = builder.AddProject<Projects.GivingChampion_Seeder>("seeder")
    .WithReference(defaultConnection, "DefaultConnection")
    .WithReference(db)
    .WithReference(migrator)
    .WaitFor(db)
    .WaitForCompletion(migrator)
    .PublishAsDockerComposeService((resource, service) =>
    {
        service.Name = "seeder";
    });

// API service configuration
builder.AddProject<Projects.GivingChampion_API>("api")
    .WithReference(defaultConnection, "DefaultConnection")
    .WithReference(db)
    .WithReference(seeder)
    .WithEnvironment("Jwt__Issuer", "GivingChampion")
    .WithEnvironment("Jwt__Audience", "GivingChampion.Client")
    .WithEnvironment("Jwt__AccessTokenMinutes", "60")
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