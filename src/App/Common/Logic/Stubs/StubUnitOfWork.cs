using App.Common.Logic.Ports;
using App.Features.Tags.Logic;

namespace App.Common.Logic.Stubs;

public class StubUnitOfWork : UnitOfWork
{
	private Action<string> trackBehaviour;
	private readonly OutputTracker<TrackerEvent<object>>? tracker = null;

	public StubUnitOfWork()
	{
		tracker = new();
		trackBehaviour = tracker.TrackBehaviour;
	}

	public StubUnitOfWork(Action<string> func)
	{
		trackBehaviour = func;
	}

	public void SetCustomTrackerFunc(Action<string> func)
	{
		trackBehaviour = func;
	}

	public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		trackBehaviour("save-changes");
		return Task.FromResult(1);
	}

	public OutputTracker<TrackerEvent<object>>? GetTracker()
	{
		return tracker;
	}
}
