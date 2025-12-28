using App.Common.Logic;
using App.Common.Logic.Exceptions;
using App.Common.Logic.Stubs;
using App.Features.Tags.Domain;
using App.Features.Tags.Infrastructure;
using App.Features.Tags.Logic;
using App.Features.Tags.Logic.Inputs;
using Shouldly;

namespace Tests.Features.Tags.UnitTests;

public class TagsServiceTests
{
	private readonly List<Tag> existingTags =
	[
		new() { Title = "Existing Tag", Slug = "existing-tag" },
	];

	[Fact]
	public async Task testCreateShouldGenerateUniqueSlugAndCreateTag()
	{
		var tag = await RunCreate(out var tracker, "Existing Tag");

		tag.Title.ShouldBe("Existing Tag");
		tag.Slug.ShouldBe("existing-tag-1");
		var createEvents = GetTrackedTagsByBehavior(tracker, "create");
		createEvents.Count.ShouldBe(1);
		createEvents.First().ShouldBeEquivalentTo(tag);
		ShouldPersist(tracker);
	}

	[Fact]
	public async Task testFindShouldReturnExistingTag()
	{
		var slug = existingTags[0].Slug;

		await Should.NotThrowAsync(async () =>
		{
			var found = await RunFind(slug);
			found.ShouldNotBeNull();
			found.ShouldBeEquivalentTo(existingTags[0]);
		});
	}

	[Fact]
	public async Task testFindShouldFailIfTaskDoesNotExist()
	{
		await Should.ThrowAsync<NoSuch>(() => RunFind("non-existent-tag"));
	}

	[Fact]
	public async Task testDeleteShouldRemoveExistingTag()
	{
		var slug = existingTags[0].Slug;

		await Should.NotThrowAsync(async () =>
		{
			await RunDelete(out var tracker, slug);
			var deleteEvents = GetTrackedTagsByBehavior(tracker, "delete");
			deleteEvents.Count.ShouldBe(1);
			deleteEvents.First().ShouldBeEquivalentTo(existingTags[0]);
			ShouldPersist(tracker);
		});
	}

	[Fact]
	public async Task testDeleteShouldFailIfTagDoesNotExist()
	{
		await Should.ThrowAsync<NoSuch>(async () =>
		{
			await RunDelete(out _, "non-existent-tag");
		});
	}

	private Task<Tag> RunCreate(
		out OutputTracker<Tag> outputTracker,
		string title = "Test Tag"
	)
	{
		var service = CreateServiceWithTracker(out outputTracker);
		return service.Create(new CreateTagInput(title));
	}

	private async Task<Tag> RunFind(string slug)
	{
		var service = TagsService.CreateNull(existingTags);
		return await service.Find(new FindTagInput(slug));
	}

	private Task RunDelete(
		out OutputTracker<Tag> outputTracker,
		string title = "Test Tag"
	)
	{
		var service = CreateServiceWithTracker(out outputTracker);
		return service.Delete(new FindTagInput(title));
	}

	private TagsService CreateServiceWithTracker(out OutputTracker<Tag> outputTracker)
	{
		var slugGen = SlugGenerator.CreateNull();

		var repo = new StubTagsRepository(existingTags);
		outputTracker = repo.GetTracker();

		var uow = new StubUnitOfWork(outputTracker.TrackBehaviour);

		return new TagsService(slugGen, repo, uow);
	}

	private List<Tag?> GetTrackedTagsByBehavior(
		OutputTracker<Tag> tracker,
		string behavior
	)
	{
		return tracker
			.GetOutput()
			.Where(e => e.Behavior == behavior)
			.Select(e => e.Subject)
			.ToList();
	}

	private void ShouldPersist(OutputTracker<Tag> tracker)
	{
		var events = tracker.GetOutput();
		events.Last().ShouldNotBeNull();
		events.Last().Behavior.ShouldBe("save-changes");
	}
}
