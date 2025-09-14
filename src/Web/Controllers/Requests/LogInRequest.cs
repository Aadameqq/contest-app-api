namespace Web.Controllers.Requests;

public class LogInRequest
{
	public required string Email { get; init; }
	public required string Password { get; init; }
	public required bool RememberMe { get; init; }
}
