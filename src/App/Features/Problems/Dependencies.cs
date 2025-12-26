using App.Features.Problems.Infrastructure;
using App.Features.Problems.Logic;
using App.Features.Problems.Logic.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace App.Features.Problems;

public static class Dependencies
{
	public static void SetUpProblems(this IServiceCollection services)
	{
		services.AddScoped<ProblemsRepository, EfProblemsRepository>();

		services.AddScoped<ProblemsService>();
	}
}
