using Core.Common.Application;
using Core.Common.Application.Exceptions;
using Core.Common.Application.Ports;
using Core.Problems.Application.Ports;
using Core.Problems.Application.Services.Tags.Inputs;
using Core.Problems.Entities;

namespace Core.Problems.Application.Services.Tags;

public class TagsService(
	SlugGenerator slugGenerator,
	TagsRepository tagsRepository,
	UnitOfWork uow
) : Service
{
	public async Task<Tag> Create(CreateTagInput input, CancellationToken ct = default)
	{
		var baseSlug = slugGenerator.Generate(input.Title);

		var index = 0;
		var slug = baseSlug;

		while (await tagsRepository.Exists(slug))
		{
			slug = $"{baseSlug}{++index}";
		}

		var tag = new Tag { Title = input.Title, Slug = slug };

		await tagsRepository.Create(tag);
		await uow.SaveChangesAsync(ct);
		return tag;
	}

	public async Task<Tag> Find(FindTagInput input, CancellationToken _ = default)
	{
		var found = await tagsRepository.Find(input.Slug);
		return found ?? throw new NoSuch("Tag not found");
	}

	public Task<List<Tag>> List(CancellationToken _ = default)
	{
		return tagsRepository.ListAll();
	}

	public async Task Update(UpdateTagInput input, CancellationToken ct = default)
	{
		var found = await tagsRepository.Find(input.Slug);
		if (found is null)
		{
			throw new NoSuch("Tag not found");
		}

		found.Title = input.Title;

		await tagsRepository.Update(found);
		await uow.SaveChangesAsync(ct);
	}

	public async Task Delete(FindTagInput input, CancellationToken ct = default)
	{
		var found = await tagsRepository.Find(input.Slug);
		if (found is null)
		{
			throw new NoSuch("Tag not found");
		}

		await tagsRepository.Delete(found);
		await uow.SaveChangesAsync(ct);
	}
}
