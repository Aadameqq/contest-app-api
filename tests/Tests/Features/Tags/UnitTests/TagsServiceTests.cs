using App.Common.Logic;
using App.Common.Logic.Exceptions;
using App.Common.Logic.Stubs;
using App.Features.Tags.Domain;
using App.Features.Tags.Logic;
using App.Features.Tags.Logic.Inputs;
using App.Features.Tags.Logic.Stubs;
using Shouldly;

namespace Tests.Features.Tags.UnitTests;

public class TagsServiceTests
{
	private OutputTracker<TagsTrackerEvent>? tracker;

	private readonly List<Tag> existingTags =
	[
		new() { Title = "Existing Tag", Slug = "existing-tag" },
	];

	[Fact]
	public async Task TestCreateShouldGenerateUniqueSlugAndCreateTag()
	{
		var existingTitle = existingTags[0].Title;

		await Should.NotThrowAsync(async () =>
		{
			var tag = await RunCreate(existingTitle);

			tag.Title.ShouldBe(existingTitle);
			tag.Slug.ShouldBe("existing-tag-1");
			var createEvents = GetCreatedTags();
			createEvents.Count.ShouldBe(1);
			createEvents.First().ShouldBeEquivalentTo(tag);
		});
	}

	[Fact]
	public async Task TestFindShouldReturnExistingTag()
	{
		var tag = existingTags[0];

		await Should.NotThrowAsync(async () =>
		{
			var found = await RunFind(tag.Slug);
			found.ShouldNotBeNull();
			found.ShouldBeEquivalentTo(tag);
		});
	}

	[Fact]
	public async Task TestFindShouldFailIfTaskDoesNotExist()
	{
		await Should.ThrowAsync<NoSuch>(() => RunFind("non-existent-tag"));
	}

	[Fact]
	public async Task TestDeleteShouldRemoveExistingTag()
	{
		var tag = existingTags[0];

		await Should.NotThrowAsync(async () =>
		{
			await RunDelete(tag.Slug);
			var deleteEvents = GetDeletedTags();
			deleteEvents.Count.ShouldBe(1);
			deleteEvents.First().ShouldBeEquivalentTo(tag);
		});
	}

	[Fact]
	public async Task TestDeleteShouldFailIfTagDoesNotExist()
	{
		await Should.ThrowAsync<NoSuch>(async () =>
		{
			await RunDelete("non-existent-tag");
		});
	}

	[Fact]
	public async Task TestUpdateShouldModifyExistingTag()
	{
		var slug = existingTags[0].Slug;
		var newTitle = "Updated Tag Title";

		await Should.NotThrowAsync(async () =>
		{
			await RunUpdate(slug, newTitle);
			var updateEvents = GetUpdatedTags();
			updateEvents.Count.ShouldBe(1);
			var updatedTag = updateEvents.First();
			updatedTag.Title.ShouldBe(newTitle);
			updatedTag.Slug.ShouldBe(slug);
		});
	}

	[Fact]
	public async Task TestUpdateShouldFailIfTagDoesNotExist()
	{
		await Should.ThrowAsync<NoSuch>(async () =>
		{
			await RunUpdate("non-existent-tag");
		});
	}

	private Task<Tag> RunCreate(string title = "Test Tag")
	{
		var service = CreateServiceWithTracker();
		return service.Create(new CreateTagInput(title));
	}

	private async Task<Tag> RunFind(string slug)
	{
		var service = TagsService.CreateNull(existingTags);
		return await service.Find(new FindTagInput(slug));
	}

	private Task RunDelete(string slug = "test-slug")
	{
		var service = CreateServiceWithTracker();
		return service.Delete(new FindTagInput(slug));
	}

	private Task RunUpdate(string slug, string newTitle = "Test Title")
	{
		var service = CreateServiceWithTracker();
		return service.Update(new UpdateTagInput(slug, newTitle));
	}

	private TagsService CreateServiceWithTracker()
	{
		var slugGen = SlugGenerator.CreateNull();

		var repo = new StubTagsRepository(existingTags);
		tracker = repo.GetTracker();

		var uow = new StubUnitOfWork(tracker);

		return new TagsService(slugGen, repo, uow);
	}

	private List<Tag> GetCreatedTags()
	{
		return GetTrackedTagsByEventType(TagsTrackerEvent.EventType.Created);
	}

	private List<Tag> GetDeletedTags()
	{
		return GetTrackedTagsByEventType(TagsTrackerEvent.EventType.Deleted);
	}

	private List<Tag> GetUpdatedTags()
	{
		return GetTrackedTagsByEventType(TagsTrackerEvent.EventType.Updated);
	}

	private List<Tag> GetTrackedTagsByEventType(TagsTrackerEvent.EventType type)
	{
		if (tracker is null)
		{
			throw new InvalidOperationException(
				"Tracker is not initialized. Generate tracker before accessing tracked events."
			);
		}

		return tracker
			.GetOutput()
			.Where(e => e.Type == type)
			.Select(e => e.Payload)
			.ToList();
	}
}
