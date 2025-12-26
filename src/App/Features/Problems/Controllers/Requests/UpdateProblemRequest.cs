namespace App.Features.Problems.Controllers.Requests;

public class UpdateProblemRequest
{
	public required string Title { get; init; }
	public List<string> TagSlugs { get; init; } = [];
}
