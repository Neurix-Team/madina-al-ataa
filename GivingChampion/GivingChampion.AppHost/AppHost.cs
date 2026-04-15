var builder = DistributedApplication.CreateBuilder(args);

var db_username = builder.AddParameter("username", "postgres", secret: true);
var db_password = builder.AddParameter("password", "postgres", secret: true);

var postgres = builder.AddPostgres("postgres", db_username, db_password, 5432);

var db = postgres.AddDatabase("givingchampion");

var defaultConnection = builder.AddConnectionString("DefaultConnection");

var migrator = builder.AddProject<Projects.GivingChampion_Migrator>("migrator")
    .WithReference(defaultConnection, "DefaultConnection")
    .WithReference(db)
    .WaitFor(db);

var seeder = builder.AddProject<Projects.GivingChampion_Seeder>("seeder")
    .WithReference(defaultConnection, "DefaultConnection")
    .WithReference(db)
    .WithReference(migrator)
    .WaitFor(db)
    .WaitForCompletion(migrator);


builder.AddProject<Projects.GivingChampion_API>("api")
    .WithReference(defaultConnection, "DefaultConnection")
    .WithReference(db)
    .WithReference(seeder)
    .WithEnvironment("Jwt__Issuer", "GivingChampion")
    .WithEnvironment("Jwt__Audience", "GivingChampion.Client")
    .WithEnvironment("Jwt__AccessTokenMinutes", "60")
    .WaitFor(db)
    .WaitForCompletion(seeder);




builder.Build().Run();
