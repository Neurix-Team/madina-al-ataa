namespace GivingChampion.API.Tests.Infrastructure;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class DatabaseCollection : ICollectionFixture<DatabaseTestFixture>
{
    public const string Name = "Database";
}
