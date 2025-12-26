using App.Common.Infrastructure.Persistence;
using App.Features.Tags.Domain;
using App.Features.Tags.Logic.Ports;
using Microsoft.EntityFrameworkCore;

namespace App.Features.Tags.Infrastructure;

public class EfTagsRepository(AppDbContext ctx) : TagsRepository
{
	public Task<bool> Exists(string slug)
	{
		return ctx.Tags.AnyAsync(t => t.Slug == slug);
	}

	public Task<Tag?> Find(string slug)
	{
		return ctx.Tags.FirstOrDefaultAsync(t => t.Slug == slug);
	}

	public Task<List<Tag>> ListAll()
	{
		return ctx.Tags.ToListAsync();
	}

	public Task Create(Tag tag)
	{
		return ctx.Tags.AddAsync(tag).AsTask();
	}

	public Task Update(Tag tag)
	{
		ctx.Tags.Update(tag);
		return Task.CompletedTask;
	}

	public Task Delete(Tag tag)
	{
		ctx.Tags.Remove(tag);
		return Task.CompletedTask;
	}
}
