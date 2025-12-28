namespace App.Common.Logic;

public class TrackerEvent<T>
	where T : class
{
	public required string Behavior { get; set; }
	public T? Subject { get; set; } = null;
}
