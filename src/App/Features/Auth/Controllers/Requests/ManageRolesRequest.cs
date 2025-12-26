namespace App.Features.Auth.Controllers.Requests;

public class ManageRolesRequest
{
	public required List<string> Roles { get; init; }
}
