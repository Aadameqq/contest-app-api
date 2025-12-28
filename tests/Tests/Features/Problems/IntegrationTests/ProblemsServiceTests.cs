using App.Common.Infrastructure.Persistence;
using App.Common.Logic.Exceptions;
using App.Features.Problems.Domain;
using App.Features.Problems.Logic;
using App.Features.Problems.Logic.Inputs;
using App.Features.Tags.Domain;
using Shouldly;
using Tests.Tools.IntegrationTests;

namespace Tests.Features.Problems.IntegrationTests;

[Collection("IntegrationTests")]
public class ProblemsServiceTests(TestWebApplicationFactory factory)
	: IntegrationTestBase<ProblemsService>(factory)
{
	private readonly Tag testTag1 = new() { Title = "Tag One", Slug = "tag-one" };
	private readonly Tag testTag2 = new() { Title = "Tag Two", Slug = "tag-two" };
	private readonly Problem testProblem = new()
	{
		Title = "Test Problem",
		Slug = "test-problem",
		CreatedAt = DateTime.UtcNow.AddDays(-1),
		UpdatedAt = DateTime.UtcNow.AddDays(-1),
	};

	protected override async Task Seed(AppDbContext ctx)
	{
		await ctx.Tags.AddRangeAsync(testTag1, testTag2);
		testProblem.Tags = [testTag1];
		await ctx.Problems.AddAsync(testProblem);
	}

	[Fact]
	public async Task testFindShouldReturnProblem()
	{
		using var scope = UseScope();
		var task = scope.Service.Find(new FindProblemInput(testProblem.Slug));

		await task.ShouldNotThrowAsync();
		var foundProblem = await task;
		foundProblem.ShouldNotBeNull();
		foundProblem.Title.ShouldBe(testProblem.Title);
		foundProblem.Slug.ShouldBe(testProblem.Slug);
		foundProblem.Tags.Count.ShouldBe(1);
		foundProblem.Tags[0].Slug.ShouldBe(testTag1.Slug);
	}

	[Fact]
	public async Task testFindShouldFailIfProblemNotFound()
	{
		using var scope = UseScope();

		await Assert.ThrowsAsync<NoSuch>(() =>
			scope.Service.Find(new FindProblemInput("invalid-problem"))
		);
	}

	[Fact]
	public async Task testDeleteShouldRemoveProblemWithGivenSlug()
	{
		using var scope = UseScope();
		await scope.Service.Delete(new FindProblemInput(testProblem.Slug));

		await Assert.ThrowsAsync<NoSuch>(() =>
			scope.Service.Find(new FindProblemInput(testProblem.Slug))
		);
	}

	[Fact]
	public async Task testDeleteShouldFailIfProblemNotFound()
	{
		using var scope = UseScope();
		await Assert.ThrowsAsync<NoSuch>(() =>
			scope.Service.Delete(new FindProblemInput("invalid-problem"))
		);
	}

	[Fact]
	public async Task testUpdate()
	{
		var newTitle = "Updated Problem Title";
		var newTagSlugs = new List<string> { testTag2.Slug };

		using var scope = UseScope();

		await scope.Service.Update(
			new UpdateProblemInput(testProblem.Slug, newTitle, newTagSlugs)
		);

		var task = scope.Service.Find(new FindProblemInput(testProblem.Slug));
		await task.ShouldNotThrowAsync();
		var updatedProblem = await task;

		updatedProblem.Title.ShouldBe(newTitle);
		updatedProblem.Tags.Count.ShouldBe(1);
		updatedProblem.Tags[0].Slug.ShouldBe(testTag2.Slug);
	}

	[Fact]
	public async Task testUpdateShouldFailIfProblemNotFound()
	{
		using var scope = UseScope();
		await Assert.ThrowsAsync<NoSuch>(() =>
			scope.Service.Update(new UpdateProblemInput("invalid-problem", "title", []))
		);
	}

	[Fact]
	public async Task testUpdateShouldFailIfTagNotFound()
	{
		using var scope = UseScope();
		await Assert.ThrowsAsync<InvalidArgument>(() =>
			scope.Service.Update(
				new UpdateProblemInput(testProblem.Slug, "title", ["invalid-tag"])
			)
		);
	}

	[Fact]
	public async Task testCreateShouldGenerateProblem()
	{
		using var scope = UseScope();
		var title = "New Problem";
		var tagSlugs = new List<string> { testTag1.Slug, testTag2.Slug };

		var problem = await scope.Service.Create(new CreateProblemInput(title, tagSlugs));

		problem.ShouldNotBeNull();
		problem.Title.ShouldBe(title);
		problem.Slug.ShouldBe("new-problem");
		problem.Tags.Count.ShouldBe(2);
		problem.Tags.ShouldContain(t => t.Slug == testTag1.Slug);
		problem.Tags.ShouldContain(t => t.Slug == testTag2.Slug);

		var task = scope.Service.Find(new FindProblemInput(problem.Slug));
		await task.ShouldNotThrowAsync();
		var foundProblem = await task;
		foundProblem.Title.ShouldBe(problem.Title);
	}

	[Fact]
	public async Task testCreateShouldFailIfTagNotFound()
	{
		using var scope = UseScope();
		await Assert.ThrowsAsync<InvalidArgument>(() =>
			scope.Service.Create(new CreateProblemInput("title", ["invalid-tag"]))
		);
	}

	[Fact]
	public async Task testCreateWithNoTags()
	{
		using var scope = UseScope();
		var title = "Problem Without Tags";

		var problem = await scope.Service.Create(new CreateProblemInput(title, []));

		problem.ShouldNotBeNull();
		problem.Title.ShouldBe(title);
		problem.Tags.Count.ShouldBe(0);
	}
}
