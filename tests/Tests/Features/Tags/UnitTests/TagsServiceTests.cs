using App.Common.Logic;
using App.Common.Logic.Exceptions;
using App.Common.Logic.Stubs;
using App.Features.Tags.Domain;
using App.Features.Tags.Infrastructure;
using App.Features.Tags.Logic;
using App.Features.Tags.Logic.Inputs;
using App.Features.Tags.Logic.Stubs;
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
		var createEvents = GetCreatedTags(tracker);
		createEvents.Count.ShouldBe(1);
		createEvents.First().ShouldBeEquivalentTo(tag);
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
			var deleteEvents = GetDeletedTags(tracker);
			deleteEvents.Count.ShouldBe(1);
			deleteEvents.First().ShouldBeEquivalentTo(existingTags[0]);
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
		out OutputTracker<TagsTrackerEvent> outputTracker,
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
		out OutputTracker<TagsTrackerEvent> outputTracker,
		string title = "Test Tag"
	)
	{
		var service = CreateServiceWithTracker(out outputTracker);
		return service.Delete(new FindTagInput(title));
	}

	private TagsService CreateServiceWithTracker(
		out OutputTracker<TagsTrackerEvent> outputTracker
	)
	{
		var slugGen = SlugGenerator.CreateNull();

		var repo = new StubTagsRepository(existingTags);
		outputTracker = repo.GetTracker();

		var uow = new StubUnitOfWork(outputTracker);

		return new TagsService(slugGen, repo, uow);
	}

	private List<Tag> GetCreatedTags(OutputTracker<TagsTrackerEvent> tracker)
	{
		return GetTrackedTagsByEventType(tracker, TagsTrackerEvent.EventType.Created);
	}

	private List<Tag> GetDeletedTags(OutputTracker<TagsTrackerEvent> tracker)
	{
		return GetTrackedTagsByEventType(tracker, TagsTrackerEvent.EventType.Deleted);
	}

	private List<Tag> GetTrackedTagsByEventType(
		OutputTracker<TagsTrackerEvent> tracker,
		TagsTrackerEvent.EventType type
	)
	{
		return tracker
			.GetOutput()
			.Where(e => e.Type == type)
			.Select(e => e.Payload)
			.ToList();
	}
}
