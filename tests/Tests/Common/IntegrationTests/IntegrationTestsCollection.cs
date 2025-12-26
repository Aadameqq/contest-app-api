namespace Tests.Common.IntegrationTests;

[CollectionDefinition("IntegrationTests")]
public class IntegrationTestsCollection
	: ICollectionFixture<TestWebApplicationFactory> { }
