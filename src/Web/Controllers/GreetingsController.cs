using Core.Auth.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GreetingsController(IMediator mediator) : ControllerBase
{
	[HttpGet]
	public ActionResult<string> Get()
	{
		return NotFound();
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] GreetingsBody body)
	{
		var cmd = new CreateGreetingCommand(body.Content);
		await mediator.Send(cmd);
		return Created();
	}
}
