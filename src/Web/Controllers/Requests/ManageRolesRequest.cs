using System.Runtime.Serialization;

namespace Web.Controllers.Requests;

public class ManageRolesRequest
{
	public required List<ManageRoleSingleChange> RoleChanges { get; init; }

	public class ManageRoleSingleChange
	{
		public required ManageRoleOperation Op { get; set; }
		public required string RoleName { get; set; }
	}

	public enum ManageRoleOperation
	{
		[EnumMember(Value = "add")]
		Add,

		[EnumMember(Value = "remove")]
		Remove,
	}
}
