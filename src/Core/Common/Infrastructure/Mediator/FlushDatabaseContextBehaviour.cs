using Core.Common.Application;
using Core.Common.Infrastructure.Persistence;
using MediatR;

namespace Core.Common.Infrastructure.Mediator;

public sealed class FlushDatabaseContextBehaviour<TRequest, TResponse>(CommonContext ctx)
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : notnull
{
	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken ct
	)
	{
		var response = await next(ct);

		if (request is Command<TResponse> || request is Command)
		{
			await ctx.SaveChangesAsync(ct);
		}

		return response;
	}
}
