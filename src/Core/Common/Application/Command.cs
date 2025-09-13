using MediatR;

namespace Core.Common.Application;

public interface Command<out TOutput> : IRequest<TOutput> { }

public interface Command : IRequest { }
