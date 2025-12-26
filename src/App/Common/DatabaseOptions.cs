using System.ComponentModel.DataAnnotations;

namespace App.Common;

public class DatabaseOptions
{
	[Required]
	public required string ConnectionString { get; init; }
}
