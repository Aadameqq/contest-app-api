using App.Features.Auth;
using App.Features.Problems;
using App.Features.Tags;
using Microsoft.Extensions.DependencyInjection;

namespace App.Features;

public static class Dependencies
{
	public static void SetUpFeatures(this IServiceCollection services)
	{
		services.SetUpAuth();
		services.SetUpProblems();
		services.SetUpTags();
	}
}
