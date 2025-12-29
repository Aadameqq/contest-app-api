using App.Common.Logic;
using App.Features.Common.Logic.Ports;

namespace App.Features.Tags.Logic;

public class OutputTracker<T> : Commitable
	where T : TrackerEvent
{
	private List<T> tracked = [];
	private List<T> transaction = [];

	public void Track(T trackerEvent)
	{
		if (trackerEvent.IsTransactional)
		{
			transaction.Add(trackerEvent);
			return;
		}
		tracked.Add(trackerEvent);
	}

	public void Commit()
	{
		tracked.AddRange(transaction);
		tracked.Sort((a, b) => a.CreatedAt.CompareTo(b.CreatedAt));
		transaction.Clear();
	}

	public List<T> GetOutput()
	{
		return tracked.AsReadOnly().ToList();
	}
}
