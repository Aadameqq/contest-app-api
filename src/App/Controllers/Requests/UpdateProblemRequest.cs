namespace App.Controllers.Requests;

public class UpdateProblemRequest
{
	public required string Title { get; init; }
	public List<string> TagSlugs { get; init; } = [];
}
