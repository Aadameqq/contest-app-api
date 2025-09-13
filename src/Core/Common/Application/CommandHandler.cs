using MediatR;

namespace Core.Common.Application;

public interface CommandHandler<in TCommand, TOutput> : IRequestHandler<TCommand, TOutput>
	where TCommand : Command<TOutput>
	where TOutput : class
{
	public new Task<TOutput> Handle(
		TCommand cmd,
		CancellationToken cancellationToken = default
	);
}

public interface CommandHandler<in TCommand> : IRequestHandler<TCommand>
	where TCommand : Command
{
	public new Task Handle(
		TCommand command,
		CancellationToken cancellationToken = default
	);
}
