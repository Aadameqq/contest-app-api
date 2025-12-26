using System.ComponentModel.DataAnnotations;

namespace App.Common.Infrastructure.Options;

public class DatabaseOptions
{
	[Required]
	public required string ConnectionString { get; init; }
}
