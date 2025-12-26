namespace App.Controllers.Requests;

public class CreateProblemRequest
{
	public required string Title { get; init; }
	public List<string> TagSlugs { get; init; } = [];
}
