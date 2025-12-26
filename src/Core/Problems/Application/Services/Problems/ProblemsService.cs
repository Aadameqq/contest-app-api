using Core.Common.Application;
using Core.Common.Application.Exceptions;
using Core.Common.Application.Ports;
using Core.Problems.Application.Ports;
using Core.Problems.Application.Services.Problems.Inputs;
using Core.Problems.Entities;

namespace Core.Problems.Application.Services.Problems;

public class ProblemsService(
	SlugGenerator slugGenerator,
	TagsRepository tagsRepository,
	ProblemsRepository problemsRepository,
	UnitOfWork uow
) : Service
{
	public async Task<Problem> Create(CreateProblemInput input)
	{
		var baseSlug = slugGenerator.Generate(input.Title);

		var index = 0;
		var slug = baseSlug;

		while (await problemsRepository.Exists(slug))
		{
			slug = $"{baseSlug}{++index}";
		}

		var tags = new List<Tag>();
		foreach (var tagSlug in input.TagSlugs)
		{
			var tag = await tagsRepository.Find(tagSlug);
			if (tag is null)
			{
				throw new InvalidArgument($"Tag with slug '{tagSlug}' not found");
			}
			tags.Add(tag);
		}

		var problem = new Problem
		{
			Title = input.Title,
			Slug = slug,
			Tags = tags,
		};

		await problemsRepository.Create(problem);
		await uow.SaveChangesAsync();
		return problem;
	}

	public async Task<Problem> Find(FindProblemInput input)
	{
		var found = await problemsRepository.Find(input.Slug);
		return found ?? throw new NoSuch("Problem not found");
	}

	public async Task Update(UpdateProblemInput input)
	{
		var found = await problemsRepository.Find(input.Slug);
		if (found is null)
		{
			throw new NoSuch("Problem not found");
		}

		var tags = new List<Tag>();
		foreach (var tagSlug in input.TagSlugs)
		{
			var tag = await tagsRepository.Find(tagSlug);
			if (tag is null)
			{
				throw new InvalidArgument($"Tag with slug '{tagSlug}' not found");
			}
			tags.Add(tag);
		}

		found.Title = input.Title;
		found.Tags = tags;

		await problemsRepository.Update(found);
		await uow.SaveChangesAsync();
	}

	public async Task Delete(FindProblemInput input)
	{
		var found = await problemsRepository.Find(input.Slug);
		if (found is null)
		{
			throw new NoSuch("Problem not found");
		}

		await problemsRepository.Delete(found);
		await uow.SaveChangesAsync();
	}
}
