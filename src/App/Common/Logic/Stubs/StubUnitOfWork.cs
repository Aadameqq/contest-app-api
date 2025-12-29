using App.Common.Logic.Ports;
using App.Features.Common.Logic.Ports;
using App.Features.Tags.Logic;

namespace App.Common.Logic.Stubs;

public class StubUnitOfWork : UnitOfWork
{
	private readonly Commitable? commitable;

	public StubUnitOfWork(Commitable commitable)
	{
		this.commitable = commitable;
	}

	public StubUnitOfWork() { }

	public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		commitable?.Commit();
		return Task.FromResult(1);
	}
}
