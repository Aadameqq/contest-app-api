namespace Core.Common.Application.Ports;

public interface UnitOfWork
{
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
