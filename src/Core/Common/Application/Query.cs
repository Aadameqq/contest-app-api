using MediatR;

namespace Core.Common.Application;

public interface Query<out TOutput> : IRequest<TOutput> { }
