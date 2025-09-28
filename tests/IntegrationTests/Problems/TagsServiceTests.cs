using Core.Common.Infrastructure.Persistence;
using Core.Problems.Application.Services.Tags;
using Core.Problems.Application.Services.Tags.Inputs;
using Core.Problems.Entities;
using Shouldly;

namespace IntegrationTests.Problems;

[Collection("ProblemsTests")]
public class TagsServiceTests(TestWebApplicationFactory factory)
	: TestBase<TagsService>(factory)
{
	private readonly Tag testTag = new() { Title = "Test Tag", Slug = "test-tag" };

	protected override async Task Seed(AppDbContext ctx)
	{
		await ctx.Tags.AddAsync(testTag);
	}

	[Fact]
	public async Task test()
	{
		using var scope = UseScope();
		var found = await scope.Service.Find(new FindTagInput(testTag.Slug));

		found.ShouldNotBeNull().ShouldBeEquivalentTo(testTag);
	}

	[Fact]
	public async Task test2()
	{
		using var scope = UseScope();
		await scope.Service.Delete(new FindTagInput(testTag.Slug));
	}

	[Fact]
	public async Task test3()
	{
		using var scope = UseScope();
		var found = await scope.Service.Find(new FindTagInput(testTag.Slug));

		found.ShouldNotBeNull().ShouldBeEquivalentTo(testTag);
	}
}
