namespace FlightManagementSystem.IntegrationTests.Infrastructure;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class IntegrationTestCollection : ICollectionFixture<ApiWebApplicationFactory>
{
    public const string Name = "integration-tests";
}
