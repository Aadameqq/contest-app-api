namespace App.Common.Logic;

public abstract class TrackerEvent
{
	public bool IsTransactional { get; private set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	protected TrackerEvent(bool isTransactional)
	{
		IsTransactional = isTransactional;
	}
}
