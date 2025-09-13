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
		return "test";
	}

	[HttpPost]
	public async Task<IActionResult> Create()
	{
		var cmd = new CreateGreetingCommand("Hello from ASP.NET Core!");
		await mediator.Send(cmd);
		return Created();
	}
}
