using App.Common.Logic;
using App.Features.Tags.Domain;

namespace App.Features.Tags.Logic.Stubs;

public class TagsTrackerEvent : TrackerEvent
{
	public EventType Type { get; private set; }
	public Tag Payload { get; private set; }

	public TagsTrackerEvent(EventType type, Tag payload, bool isTransactional = true)
		: base(isTransactional)
	{
		Type = type;
		Payload = payload;
	}

	public static TagsTrackerEvent InitCreatedEvent(Tag tag)
	{
		return new TagsTrackerEvent(EventType.Created, tag);
	}

	public static TagsTrackerEvent InitDeletedEvent(Tag tag)
	{
		return new TagsTrackerEvent(EventType.Deleted, tag);
	}

	public static TagsTrackerEvent InitUpdatedEvent(Tag tag)
	{
		return new TagsTrackerEvent(EventType.Updated, tag);
	}

	public enum EventType
	{
		Created,
		Deleted,
		Updated,
	}
}
