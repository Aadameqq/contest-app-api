using App.Common.Logic;

namespace App.Features.Tags.Logic;

public class OutputTracker<T>
	where T : class
{
	private List<TrackerEvent<T>> tracked = [];

	public void Track(TrackerEvent<T> trackerEvent)
	{
		tracked.Add(trackerEvent);
	}

	public void TrackBehaviour(string behavior)
	{
		tracked.Add(new TrackerEvent<T> { Behavior = behavior });
	}

	public List<TrackerEvent<T>> GetOutput()
	{
		return tracked.AsReadOnly().ToList();
	}
}
