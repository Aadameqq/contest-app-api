using MediatR;

namespace Core.Common.Application;

public interface QueryHandler<in TQuery, TOutput> : IRequestHandler<TQuery, TOutput>
	where TQuery : Query<TOutput>
{
	public new Task<TOutput> Handle(
		TQuery query,
		CancellationToken cancellationToken = default
	);
}
