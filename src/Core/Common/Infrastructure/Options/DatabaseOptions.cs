using System.ComponentModel.DataAnnotations;

namespace Core.Common.Infrastructure.Options;

public class DatabaseOptions
{
	[Required]
	public required string ConnectionString { get; init; }
}
