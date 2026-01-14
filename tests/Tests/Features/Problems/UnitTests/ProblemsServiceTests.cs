using App.Common.Infrastructure;
using App.Common.Logic;
using App.Common.Logic.Exceptions;
using App.Common.Logic.Stubs;
using App.Features.Problems.Domain;
using App.Features.Problems.Logic;
using App.Features.Problems.Logic.Inputs;
using App.Features.Problems.Logic.Stubs;
using App.Features.Tags.Domain;
using App.Features.Tags.Logic;
using App.Features.Tags.Logic.Stubs;
using Shouldly;

namespace Tests.Features.Problems.UnitTests;

public class ProblemsServiceTests
{
	private OutputTracker<ProblemsTrackerEvent>? tracker;

	private readonly List<Problem> existingProblems =
	[
		new() { Title = "Existing Problem", Slug = "existing-problem" },
		new() { Title = "Second Problem", Slug = "second-problem" },
		new() { Title = "Third Problem", Slug = "third-problem" },
		new() { Title = "Fourth Problem", Slug = "fourth-problem" },
		new() { Title = "Fifth Problem", Slug = "fifth-problem" },
	];

	private readonly List<Tag> existingTags =
	[
		new() { Title = "Algorithm", Slug = "algorithm" },
		new() { Title = "Data Structure", Slug = "data-structure" },
	];

	[Fact]
	public async Task TestCreateShouldGenerateSlugAndCreateProblem()
	{
		var algorithmTag = existingTags[0];
		var title = "New Problem";

		await Should.NotThrowAsync(async () =>
		{
			var problem = await RunCreate(title, [algorithmTag.Slug]);

			problem.Title.ShouldBe(title);
			problem.Slug.ShouldBe("new-problem");
			problem.Tags.Count.ShouldBe(1);
			problem.Tags.First().ShouldBeEquivalentTo(algorithmTag);

			var createEvents = GetCreatedProblems();
			createEvents.Count.ShouldBe(1);
			createEvents.First().ShouldBeEquivalentTo(problem);
		});
	}

	[Fact]
	public async Task TestCreateShouldWorkWithEmptyTagsList()
	{
		await Should.NotThrowAsync(async () =>
		{
			var problem = await RunCreate();

			problem.Tags.Count.ShouldBe(0);
		});
	}

	[Fact]
	public async Task TestCreateShouldFailWhenTagNotFound()
	{
		await Should.ThrowAsync<InvalidArgument>(async () =>
		{
			await RunCreate(tagSlugs: ["non-existent-tag"]);
		});
	}

	[Fact]
	public async Task TestFindShouldReturnExistingProblem()
	{
		var existingProblem = existingProblems[0];

		await Should.NotThrowAsync(async () =>
		{
			var found = await RunFind(existingProblem.Slug);

			found.ShouldNotBeNull();
			found.ShouldBeEquivalentTo(existingProblem);
		});
	}

	[Fact]
	public async Task TestFindShouldFailIfProblemDoesNotExist()
	{
		await Should.ThrowAsync<NoSuch>(() => RunFind("non-existent-problem"));
	}

	[Fact]
	public async Task TestUpdateShouldModifyExistingProblem()
	{
		var existingProblem = existingProblems[0];
		var dataStructureTag = existingTags[1];
		var slug = existingProblem.Slug;
		var newTitle = "Updated Problem Title";

		await Should.NotThrowAsync(async () =>
		{
			await RunUpdate(slug, newTitle, [dataStructureTag.Slug]);
			var updateEvents = GetUpdatedProblems();
			updateEvents.Count.ShouldBe(1);
			var updatedProblem = updateEvents.First();
			updatedProblem.Title.ShouldBe(newTitle);
			updatedProblem.Slug.ShouldBe(slug);
			updatedProblem.Tags.Count.ShouldBe(1);
			updatedProblem.Tags.First().ShouldBeEquivalentTo(dataStructureTag);
		});
	}

	[Fact]
	public async Task TestUpdateShouldFailIfProblemDoesNotExist()
	{
		await Should.ThrowAsync<NoSuch>(async () =>
		{
			await RunUpdate("non-existent-problem");
		});
	}

	[Fact]
	public async Task TestUpdateShouldFailWhenTagNotFound()
	{
		var existingProblem = existingProblems[0];

		await Should.ThrowAsync<InvalidArgument>(async () =>
		{
			await RunUpdate(existingProblem.Slug, newTagSlugs: ["non-existent-tag"]);
		});
	}

	[Fact]
	public async Task TestDeleteShouldRemoveExistingProblem()
	{
		var existingProblem = existingProblems[0];

		await Should.NotThrowAsync(async () =>
		{
			await RunDelete(existingProblem.Slug);
			var deleteEvents = GetDeletedProblems();
			deleteEvents.Count.ShouldBe(1);
			deleteEvents.First().ShouldBeEquivalentTo(existingProblem);
		});
	}

	[Fact]
	public async Task TestDeleteShouldFailIfProblemDoesNotExist()
	{
		await Should.ThrowAsync<NoSuch>(async () =>
		{
			await RunDelete("non-existent-problem");
		});
	}

	[Fact]
	public async Task TestSearchShouldReturnPaginatedResults()
	{
		var result = await RunSearch(page: 1, perPage: 2);

		result.Page.ShouldBe(1);
		result.PerPage.ShouldBe(2);
		result.Pages.ShouldBe(3);
		result.NextPage.ShouldBe(2);
	}

	[Fact]
	public async Task TestSearchShouldReturnCorrectPageOfResults()
	{
		var result = await RunSearch(page: 2, perPage: 2);

		result.Payload.Count.ShouldBe(2);
		result.Payload[0].ShouldBeEquivalentTo(existingProblems[2]);
		result.Payload[1].ShouldBeEquivalentTo(existingProblems[3]);
	}

	[Fact]
	public async Task TestSearchShouldHandleLastPartialPage()
	{
		var result = await RunSearch(page: 3, perPage: 2);

		result.Payload.Count.ShouldBe(1);
		result.NextPage.ShouldBe(null);
		result.Payload[0].ShouldBeEquivalentTo(existingProblems[4]);
	}

	[Fact]
	public async Task TestSearchShouldReturnEmptyResultsForPageBeyondTotal()
	{
		var result = await RunSearch(page: 10, perPage: 2);

		result.Payload.Count.ShouldBe(0);
		result.NextPage.ShouldBe(null);
	}

	private Task<Problem> RunCreate(
		string title = "Test Problem",
		List<string>? tagSlugs = null
	)
	{
		var service = CreateServiceWithTracker();
		return service.Create(new CreateProblemInput(title, tagSlugs ?? []));
	}

	private async Task<Problem> RunFind(string slug)
	{
		var service = ProblemsService.CreateNull(existingProblems, existingTags);
		return await service.Find(new FindProblemInput(slug));
	}

	private Task RunUpdate(
		string slug,
		string newTitle = "Test Title",
		List<string>? newTagSlugs = null
	)
	{
		var service = CreateServiceWithTracker();
		return service.Update(new UpdateProblemInput(slug, newTitle, newTagSlugs ?? []));
	}

	private Task RunDelete(string slug)
	{
		var service = CreateServiceWithTracker();
		return service.Delete(new FindProblemInput(slug));
	}

	private Task<Paginated<Problem>> RunSearch(int page, int? perPage = null)
	{
		var service = ProblemsService.CreateNull(existingProblems, existingTags);
		return service.Search(new SearchProblemsInput(page, perPage));
	}

	private ProblemsService CreateServiceWithTracker()
	{
		var slugifier = new BasicSlugifier();
		var tagsRepo = new StubTagsRepository(existingTags);
		var problemsRepo = new StubProblemsRepository(existingProblems);
		tracker = problemsRepo.GetTracker();
		var uow = new StubUnitOfWork(tracker);
		return new ProblemsService(slugifier, tagsRepo, problemsRepo, uow);
	}

	private List<Problem> GetCreatedProblems()
	{
		return GetTrackedProblemsByEventType(ProblemsTrackerEvent.EventType.Created);
	}

	private List<Problem> GetUpdatedProblems()
	{
		return GetTrackedProblemsByEventType(ProblemsTrackerEvent.EventType.Updated);
	}

	private List<Problem> GetDeletedProblems()
	{
		return GetTrackedProblemsByEventType(ProblemsTrackerEvent.EventType.Deleted);
	}

	private List<Problem> GetTrackedProblemsByEventType(
		ProblemsTrackerEvent.EventType type
	)
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
