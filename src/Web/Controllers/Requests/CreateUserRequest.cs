namespace Web.Controllers.Requests;

public class CreateUserRequest
{
	public required string Email { get; init; }
	public required string Password { get; init; }
}
