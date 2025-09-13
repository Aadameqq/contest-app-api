using System.ComponentModel.DataAnnotations;

namespace Web.Controllers;

public class GreetingsBody
{
	[MinLength(10)]
	public string Content { get; set; }
}
