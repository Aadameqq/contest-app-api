using System.ComponentModel.DataAnnotations;

namespace Core.Options;

public class DatabaseOptions
{
	[Required]
	public required string ConnectionString { get; init; }
}
