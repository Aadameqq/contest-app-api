using Core.Auth.Entities;
using Core.Problems.Application.Services.Problems;
using Core.Problems.Application.Services.Problems.Inputs;
using Core.Problems.Entities;
using Microsoft.AspNetCore.Mvc;
using Web.Controllers.Requests;
using Web.Identity;

namespace Web.Controllers;

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
