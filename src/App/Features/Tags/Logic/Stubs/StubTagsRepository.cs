using App.Features.Tags.Domain;
using App.Features.Tags.Logic;
using App.Features.Tags.Logic.Ports;

namespace App.Features.Tags.Infrastructure;

public class StubTagsRepository : TagsRepository
{
	private List<Tag> existing = [];

	private readonly OutputTracker<TrackerEvent> tracker = new();

	public StubTagsRepository(List<Tag>? existingTags)
	{
		if (existingTags is not null)
		{
			existing = existingTags;
		}
	}

	public Task Create(Tag tag)
	{
		tracker.Track(new TrackerEvent { Behavior = "create", Subject = tag });
		return Task.CompletedTask;
	}

	public Task Delete(Tag tag)
	{
		tracker.Track(new TrackerEvent { Behavior = "delete", Subject = tag });
		return Task.CompletedTask;
	}

	public Task<bool> Exists(string slug)
	{
		return Task.FromResult(existing.Any(t => t.Slug == slug));
	}

	public Task<Tag?> Find(string slug)
	{
		return Task.FromResult(existing.FirstOrDefault(t => t.Slug == slug));
	}

	public Task<List<Tag>> ListAll()
	{
		return Task.FromResult(existing.ToList());
	}

	public Task Update(Tag tag)
	{
		tracker.Track(new TrackerEvent { Behavior = "update", Subject = tag });
		return Task.CompletedTask;
	}

	public OutputTracker<TrackerEvent> GetTracker()
	{
		return tracker;
	}

	public class TrackerEvent
	{
		public required string Behavior { get; set; }
		public required Tag Subject { get; set; }
	}
}
