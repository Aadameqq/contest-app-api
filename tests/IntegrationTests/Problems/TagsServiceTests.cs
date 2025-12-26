using App.Common.Infrastructure.Persistence;
using App.Common.Logic.Exceptions;
using App.Features.Tags.Domain;
using App.Features.Tags.Logic;
using App.Features.Tags.Logic.Inputs;
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
	public async Task testFindShouldReturnTag()
	{
		using var scope = UseScope();
		var task = scope.Service.Find(new FindTagInput(testTag.Slug));

		await task.ShouldNotThrowAsync();
		var foundTag = await task;
		foundTag.ShouldNotBeNull();
		foundTag.ShouldBeEquivalentTo(testTag);
	}

	[Fact]
	public async Task testFindShouldFailIfTagNotFound()
	{
		using var scope = UseScope();

		await Assert.ThrowsAsync<NoSuch>(() =>
			scope.Service.Find(new FindTagInput("invalid-tag"))
		);
	}

	[Fact]
	public async Task testDeleteShouldRemoveTagWithGivenSlug()
	{
		using var scope = UseScope();
		await scope.Service.Delete(new FindTagInput(testTag.Slug));

		await Assert.ThrowsAsync<NoSuch>(() =>
			scope.Service.Find(new FindTagInput(testTag.Slug))
		);
	}

	[Fact]
	public async Task testDeleteShouldFailIfTagNotFound()
	{
		using var scope = UseScope();
		await Assert.ThrowsAsync<NoSuch>(() =>
			scope.Service.Delete(new FindTagInput("invalid-tag"))
		);
	}

	[Fact]
	public async Task testUpdate()
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
	public async Task testUpdateShouldFailIfTagNotFound()
	{
		using var scope = UseScope();
		await Assert.ThrowsAsync<NoSuch>(() =>
			scope.Service.Update(new UpdateTagInput("invalid-tag", "sth"))
		);
	}

	[Fact]
	public async Task testCreateShouldGenerateTag()
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
	public async Task testCreateShouldHandleSlugCollision()
	{
		using var scope = UseScope();

		var title = testTag.Title;
		var tag = await scope.Service.Create(new CreateTagInput(title));

		tag.ShouldNotBeNull();
		tag.Title.ShouldBe(title);
		tag.Slug.ShouldBe("test-tag1");
	}

	[Fact]
	public async Task testCreateShouldHandleMultipleSlugCollisions()
	{
		using var scope = UseScope();
		await scope.Service.Create(new CreateTagInput(testTag.Title));
		var tag = await scope.Service.Create(new CreateTagInput(testTag.Title));

		tag.ShouldNotBeNull();
		tag.Title.ShouldBe(testTag.Title);
		tag.Slug.ShouldBe("test-tag2");
	}

	[Fact]
	public async Task testListShouldReturnAllTags()
	{
		using var scope = UseScope();
		var other = await scope.Service.Create(new CreateTagInput("Another Tag"));

		var tags = await scope.Service.List();

		tags.ShouldNotBeNull();
		tags.Count.ShouldBe(2);
		tags.ShouldContain(t => t.Slug == testTag.Slug);
		tags.ShouldContain(t => t.Slug == other.Slug);
	}

	[Theory]
	[InlineData("Test Title", "test-title")]
	[InlineData("Test@ Title!", "test-title")]
	[InlineData("Tęśt TĄg !!! .", "test-tag1")]
	[InlineData("   Test   Title   ", "test-title")]
	[InlineData("Test-Title", "test-title")]
	[InlineData("Test_Title", "test_title")]
	[InlineData("Test--Title", "test-title")]
	[InlineData("123 Test Title", "123-test-title")]
	[InlineData("Test  -  _  Title", "test-title")]
	[InlineData("Hello World!", "hello-world")]
	public async Task testCreateShouldGenerateCorrectSlugs(
		string inputTitle,
		string expectedSlug
	)
	{
		using var scope = UseScope();

		var tag = await scope.Service.Create(new CreateTagInput(inputTitle));

		tag.ShouldNotBeNull();
		tag.Title.ShouldBe(inputTitle);
		tag.Slug.ShouldBe(expectedSlug);
	}
}
