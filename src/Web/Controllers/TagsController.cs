using Core.Auth.Entities;
using Core.Problems.Application.Services.Tags;
using Core.Problems.Application.Services.Tags.Inputs;
using Core.Problems.Entities;
using Microsoft.AspNetCore.Mvc;
using Web.Controllers.Requests;
using Web.Identity;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController(TagsService service) : ControllerBase
{
	[HttpPost]
	[CheckAuth(Role.Admin, Role.Moderator)]
	public async Task<IActionResult> Create([FromBody] CreateTagRequest req)
	{
		var input = new CreateTagInput(req.Title);
		var tag = await service.Create(input);
		return CreatedAtAction(nameof(Get), new { slug = tag.Slug }, null);
	}

	[HttpGet("{slug}")]
	[CheckAuth(Role.Editor, Role.Admin, Role.Moderator)]
	public Task<Tag> Get([FromRoute] string slug)
	{
		var input = new FindTagInput(slug);
		return service.Find(input);
	}

	[HttpGet]
	public Task<List<Tag>> GetAll()
	{
		return service.List();
	}

	[HttpPut("{slug}")]
	[CheckAuth(Role.Admin, Role.Moderator)]
	public Task Put([FromRoute] string slug, [FromBody] CreateTagRequest req)
	{
		var input = new UpdateTagInput(slug, req.Title);
		return service.Update(input);
	}

	[HttpDelete("{slug}")]
	[CheckAuth(Role.Admin, Role.Moderator)]
	public Task Delete([FromRoute] string slug)
	{
		var input = new FindTagInput(slug);
		return service.Delete(input);
	}
}
