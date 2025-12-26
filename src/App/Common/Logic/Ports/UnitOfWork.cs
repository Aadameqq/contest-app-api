namespace App.Common.Logic.Ports;

public interface UnitOfWork
{
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
