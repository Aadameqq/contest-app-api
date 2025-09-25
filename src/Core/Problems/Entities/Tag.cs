namespace Core.Problems.Entities;

public class Tag
{
	public Guid Id { get; set; }
	public required string Title { get; set; }
	public required string Slug { get; set; }
}
