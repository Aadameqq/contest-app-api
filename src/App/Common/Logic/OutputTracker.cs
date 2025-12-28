namespace App.Features.Tags.Logic;

public class OutputTracker<T>
	where T : class
{
	private List<T> tracked = [];

	public void Track(T item)
	{
		tracked.Add(item);
	}

	public List<T> GetOutput()
	{
		return tracked.AsReadOnly().ToList();
	}
}
