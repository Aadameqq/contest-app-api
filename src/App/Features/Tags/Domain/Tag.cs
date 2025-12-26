namespace App.Features.Tags.Domain;

public class Tag
{
	public Guid Id { get; init; } = Guid.NewGuid();
	public required string Title { get; set; }
	public required string Slug { get; set; }
}
