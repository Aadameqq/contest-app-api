namespace App.Features.Tags.Domain;

public class Tag
{
	public Guid Id { get; init; } = Guid.NewGuid();
	public required string Title { get; set; }
	public required string Slug { get; set; }

	public static Tag CreateTestInstance(string? title = null, string? slug = null)
	{
		return new Tag { Title = title ?? "Test Tag", Slug = slug ?? "test-tag" };
	}
}
