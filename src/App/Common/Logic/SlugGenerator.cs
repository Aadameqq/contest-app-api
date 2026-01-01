using App.Common.Infrastructure;
using App.Common.Logic.Ports;

namespace App.Common.Logic;

public class SlugGenerator(Slugifier slugifier)
{
	public static SlugGenerator CreateNull()
	{
		Slugifier slugifier = new BasicSlugifier();
		return new SlugGenerator(slugifier);
	}

	public async Task<string> Generate(string title, Func<string, Task<bool>> exists)
	{
		var baseSlug = slugifier.Slugify(title);
		var index = 0;
		var slug = baseSlug;

		while (await exists(slug))
		{
			index++;
			slug = $"{baseSlug}-{index}";
		}

		return slug;
	}
}
