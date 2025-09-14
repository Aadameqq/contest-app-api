namespace Web.Controllers.Requests;

public class LogInRequest
{
	public string Email { get; set; }
	public string Password { get; set; }
	public bool RememberMe { get; set; }
}
