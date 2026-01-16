namespace App.Common.Logic;

public record Paginated<T>(
	List<T> Payload,
	int Page,
	int PerPage,
	int Pages,
	int? NextPage
);
