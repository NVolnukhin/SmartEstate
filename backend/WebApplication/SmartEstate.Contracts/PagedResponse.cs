namespace Contracts;

public class PagedResponse<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public List<T> Items { get; set; }

    public PagedResponse(List<T> items, int count, int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
        TotalCount = count;
        Items = items;
    }
}