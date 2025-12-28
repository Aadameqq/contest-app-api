namespace Tests.Tools.IntegrationTests;

[CollectionDefinition("IntegrationTests")]
public class IntegrationTestsCollection
	: ICollectionFixture<TestWebApplicationFactory> { }
