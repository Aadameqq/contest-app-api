using App.Common.Logic;

namespace App.Common.Web;

public record PaginatedResponse<T>(
	List<T> Payload,
	PaginatedResponse<T>.PageInfo Pagination
)
{
	public record PageInfo(int Page, int PerPage, int Pages, int? NextPage);

	public static PaginatedResponse<T> OfPaginated(Paginated<T> paginated)
	{
		var pageInfo = new PageInfo(
			paginated.Page,
			paginated.PerPage,
			paginated.Pages,
			paginated.NextPage
		);
		return new PaginatedResponse<T>(paginated.Payload, pageInfo);
	}
}
