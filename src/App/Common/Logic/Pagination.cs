namespace App.Common.Logic;

public record Pagination(int Page, int PerPage)
{
	public int Offset => (Page - 1) * PerPage;
	public int Limit => PerPage;

	public Paginated<T> AsPaginated<T>(int totalCount, List<T> payload)
	{
		var pages = (int)Math.Ceiling((double)totalCount / PerPage);
		int? nextPage = Page < pages ? Page + 1 : null;
		return new Paginated<T>(payload, Page, PerPage, pages, nextPage);
	}
}
