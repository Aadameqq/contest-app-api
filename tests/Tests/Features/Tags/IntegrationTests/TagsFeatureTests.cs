using App.Common.Infrastructure.Persistence;
using App.Common.Logic.Exceptions;
using App.Features.Tags.Domain;
using App.Features.Tags.Logic;
using App.Features.Tags.Logic.Inputs;
using Shouldly;
using Tests.Tools.IntegrationTests;

namespace Tests.Features.Tags.IntegrationTests;

[Collection("IntegrationTests")]
public class TagsFeatureTests(TestWebApplicationFactory factory)
	: IntegrationTestBase<TagsService>(factory)
{
	private readonly Tag testTag = new() { Title = "Test Tag", Slug = "test-tag" };

	protected override async Task Seed(AppDbContext ctx)
	{
		await ctx.Tags.AddAsync(testTag);
	}

	[Fact]
	public async Task TestFindShouldReturnTag()
	{
		using var scope = UseScope();
		var task = scope.Service.Find(new FindTagInput(testTag.Slug));

		await task.ShouldNotThrowAsync();
		var foundTag = await task;
		foundTag.ShouldNotBeNull();
		foundTag.ShouldBeEquivalentTo(testTag);
	}

	[Fact]
	public async Task TestFindShouldFailIfTagNotFound()
	{
		using var scope = UseScope();

		await Assert.ThrowsAsync<NoSuch>(() =>
			scope.Service.Find(new FindTagInput("invalid-tag"))
		);
	}

	[Fact]
	public async Task TestDeleteShouldRemoveTagWithGivenSlug()
	{
		using var scope = UseScope();
		await scope.Service.Delete(new FindTagInput(testTag.Slug));

		await Assert.ThrowsAsync<NoSuch>(() =>
			scope.Service.Find(new FindTagInput(testTag.Slug))
		);
	}

	[Fact]
	public async Task TestDeleteShouldFailIfTagNotFound()
	{
		using var scope = UseScope();
		await Assert.ThrowsAsync<NoSuch>(() =>
			scope.Service.Delete(new FindTagInput("invalid-tag"))
		);
	}

	[Fact]
	public async Task TestUpdate()
	{
		var newTitle = "new-title";
		using var scope = UseScope();
		await scope.Service.Update(new UpdateTagInput(testTag.Slug, newTitle));

		var task = scope.Service.Find(new FindTagInput(testTag.Slug));

		await task.ShouldNotThrowAsync();
		var updatedTag = await task;
		updatedTag.Title.ShouldBe(newTitle);
	}

	[Fact]
	public async Task TestUpdateShouldFailIfTagNotFound()
	{
		using var scope = UseScope();
		await Assert.ThrowsAsync<NoSuch>(() =>
			scope.Service.Update(new UpdateTagInput("invalid-tag", "sth"))
		);
	}

	[Fact]
	public async Task TestCreateShouldGenerateTag()
	{
		using var scope = UseScope();
		var title = "New Tag";
		var tag = await scope.Service.Create(new CreateTagInput(title));

		tag.ShouldNotBeNull();
		tag.Title.ShouldBe(title);
		tag.Slug.ShouldBe("new-tag");

		var task = scope.Service.Find(new FindTagInput(tag.Slug));
		await task.ShouldNotThrowAsync();
		var foundTag = await task;
		foundTag.ShouldBeEquivalentTo(tag);
	}

	[Fact]
	public async Task TestListShouldReturnAllTags()
	{
		using var scope = UseScope();
		var other = await scope.Service.Create(new CreateTagInput("Another Tag"));

		var tags = await scope.Service.List();

		tags.ShouldNotBeNull();
		tags.Count.ShouldBe(2);
		tags.ShouldContain(t => t.Slug == testTag.Slug);
		tags.ShouldContain(t => t.Slug == other.Slug);
	}
}
