using App.Common.Logic;
using App.Features.Problems.Domain;

namespace App.Features.Problems.Logic.Stubs;

public class ProblemsTrackerEvent : TrackerEvent
{
	public EventType Type { get; private set; }
	public Problem Payload { get; private set; }

	public ProblemsTrackerEvent(
		EventType type,
		Problem payload,
		bool isTransactional = true
	)
		: base(isTransactional)
	{
		Type = type;
		Payload = payload;
	}

	public static ProblemsTrackerEvent InitCreatedEvent(Problem problem)
	{
		return new ProblemsTrackerEvent(EventType.Created, problem);
	}

	public static ProblemsTrackerEvent InitDeletedEvent(Problem problem)
	{
		return new ProblemsTrackerEvent(EventType.Deleted, problem);
	}

	public static ProblemsTrackerEvent InitUpdatedEvent(Problem problem)
	{
		return new ProblemsTrackerEvent(EventType.Updated, problem);
	}

	public enum EventType
	{
		Created,
		Deleted,
		Updated,
	}
}
