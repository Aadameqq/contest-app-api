using Core.Auth.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GreetingsController(IMediator mediator) : ControllerBase
{
	[Authorize]
	[HttpGet]
	public ActionResult<string> Get()
	{
		return NotFound();
	}

	[HttpPost]
	public async Task<IActionResult> Create()
	{
		var cmd = new CreateGreetingCommand("body.Content");
		await mediator.Send(cmd);
		return Created();
	}
}
