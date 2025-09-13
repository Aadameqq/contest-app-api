using Core.Auth.Entities;
using Core.Common.Application;
using Core.Common.Infrastructure.Persistence;

namespace Core.Auth.Application.Commands;

public class CreateGreetingCommandHandler(CommonContext ctx)
	: CommandHandler<CreateGreetingCommand>
{
	public async Task Handle(
		CreateGreetingCommand command,
		CancellationToken cancellationToken = default
	)
	{
		await ctx.Greetings.AddAsync(
			new Greeting { Message = command.Content },
			cancellationToken
		);
	}
}
