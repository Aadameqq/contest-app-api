using Core.Common.Infrastructure.Persistence;
using Core.Problems.Application.Ports;
using Core.Problems.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Problems.Infrastructure.Persistence;

public class EfProblemsRepository(AppDbContext ctx) : ProblemsRepository
{
	public Task<bool> Exists(string slug)
	{
		return ctx.Problems.AnyAsync(p => p.Slug == slug);
	}

	public Task<Problem?> Find(string slug)
	{
		return ctx.Problems.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Slug == slug);
	}

	public Task<List<Problem>> ListAll()
	{
		return ctx.Problems.Include(p => p.Tags).ToListAsync();
	}

	public Task Create(Problem problem)
	{
		return ctx.Problems.AddAsync(problem).AsTask();
	}

	public Task Update(Problem problem)
	{
		ctx.Problems.Update(problem);
		return Task.CompletedTask;
	}

	public Task Delete(Problem problem)
	{
		ctx.Problems.Remove(problem);
		return Task.CompletedTask;
	}
}
