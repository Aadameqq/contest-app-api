namespace App.Common;

public interface UnitOfWork
{
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
