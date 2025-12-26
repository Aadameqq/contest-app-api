using App.Common.Infrastructure.Persistence;
using App.Features.Problems.Domain;
using App.Features.Problems.Logic.Ports;
using Microsoft.EntityFrameworkCore;

namespace App.Features.Problems.Infrastructure;

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
