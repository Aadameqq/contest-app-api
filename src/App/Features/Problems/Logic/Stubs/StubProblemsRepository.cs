using App.Common.Logic;
using App.Features.Problems.Domain;
using App.Features.Problems.Logic.Ports;
using App.Features.Tags.Logic;

namespace App.Features.Problems.Logic.Stubs;

public class StubProblemsRepository : ProblemsRepository
{
	private readonly List<Problem> problems;
	private readonly OutputTracker<ProblemsTrackerEvent> tracker = new();

	public StubProblemsRepository(List<Problem>? existingProblems = null)
	{
		problems = existingProblems ?? [];
	}

	public OutputTracker<ProblemsTrackerEvent> GetTracker() => tracker;

	public Task<bool> Exists(string slug)
	{
		return Task.FromResult(problems.Any(p => p.Slug == slug));
	}

	public Task<Problem?> Find(string slug)
	{
		return Task.FromResult(problems.FirstOrDefault(p => p.Slug == slug));
	}

	public Task<List<Problem>> ListAll()
	{
		return Task.FromResult(problems.ToList());
	}

	public Task<Paginated<Problem>> Search(Pagination pagination)
	{
		var payload = problems.Skip(pagination.Offset).Take(pagination.Limit).ToList();
		var totalCount = problems.Count;
		return Task.FromResult(pagination.AsPaginated(totalCount, payload));
	}

	public Task Create(Problem problem)
	{
		tracker.Track(ProblemsTrackerEvent.InitCreatedEvent(problem));
		return Task.CompletedTask;
	}

	public Task Update(Problem problem)
	{
		tracker.Track(ProblemsTrackerEvent.InitUpdatedEvent(problem));
		return Task.CompletedTask;
	}

	public Task Delete(Problem problem)
	{
		tracker.Track(ProblemsTrackerEvent.InitDeletedEvent(problem));
		return Task.CompletedTask;
	}
}
