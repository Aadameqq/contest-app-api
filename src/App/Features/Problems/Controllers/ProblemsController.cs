using App.Common.Domain;
using App.Common.Web;
using App.Common.Web.Identity;
using App.Features.Problems.Controllers.Requests;
using App.Features.Problems.Domain;
using App.Features.Problems.Logic;
using App.Features.Problems.Logic.Inputs;
using Microsoft.AspNetCore.Mvc;

namespace App.Features.Problems.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProblemsController(ProblemsService service) : ControllerBase
{
	[HttpPost]
	[CheckAuth(Role.Admin, Role.Moderator, Role.Editor)]
	public async Task<IActionResult> Create([FromBody] CreateProblemRequest req)
	{
		var input = new CreateProblemInput(req.Title, req.TagSlugs);
		var problem = await service.Create(input);
		return CreatedAtAction(nameof(Get), new { slug = problem.Slug }, null);
	}

	[HttpGet]
	public async Task<PaginatedResponse<Problem>> Search(
		[FromQuery] int? perPage,
		[FromQuery] int page = 1
	)
	{
		var input = new SearchProblemsInput(page, perPage);
		var paginated = await service.Search(input);
		return PaginatedResponse<Problem>.OfPaginated(paginated);
	}

	[HttpGet("{slug}")]
	[CheckAuth(Role.Editor, Role.Admin, Role.Moderator)]
	public Task<Problem> Get([FromRoute] string slug)
	{
		var input = new FindProblemInput(slug);
		return service.Find(input);
	}

	[HttpPut("{slug}")]
	[CheckAuth(Role.Admin, Role.Moderator, Role.Editor)]
	public Task Put([FromRoute] string slug, [FromBody] UpdateProblemRequest req)
	{
		var input = new UpdateProblemInput(slug, req.Title, req.TagSlugs);
		return service.Update(input);
	}

	[HttpDelete("{slug}")]
	[CheckAuth(Role.Admin, Role.Moderator, Role.Editor)]
	public Task Delete([FromRoute] string slug)
	{
		var input = new FindProblemInput(slug);
		return service.Delete(input);
	}
}
