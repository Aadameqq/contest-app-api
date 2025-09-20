namespace Web.Controllers.Requests;

public class ManageRolesRequest
{
	public required List<ManageRoleRequest> RoleChanges { get; init; }

	public class ManageRoleRequest
	{
		public required string Op { get; set; }
		public required string RoleName { get; set; }
	}

	public static class Operation
	{
		public const string Add = "add";
		public const string Remove = "remove";
	}
}
