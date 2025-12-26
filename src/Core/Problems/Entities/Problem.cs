namespace Core.Problems.Entities;

public class Problem
{
	public Guid Id { get; init; } = Guid.NewGuid();
	public required string Title { get; set; }
	public required string Slug { get; set; }
	public List<Tag> Tags { get; set; } = [];
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}
