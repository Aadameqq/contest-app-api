using App.Common.Logic.Ports;
using App.Features.Tags.Logic;

namespace App.Common.Logic.Stubs;

public class StubUnitOfWork : UnitOfWork
{
	private readonly OutputTracker<TrackerEvent> tracker = new();

	public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		tracker.Track(new TrackerEvent { Behavior = "save-changes" });
		// Return 1 to simulate successful save
		return Task.FromResult(1);
	}

	public OutputTracker<TrackerEvent> GetTracker()
	{
		return tracker;
	}

	public class TrackerEvent
	{
		public required string Behavior { get; set; }
	}
}
