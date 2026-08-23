using MeusPila.Domain.Interfaces;

namespace MeusPila.Domain.Common;

public static class PagedListProperties
{
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 50;
}

public class PagedList<T> : IPagedList<T>
    where T : class
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public IEnumerable<T> Items { get; init; } = [];

    public PagedList(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}
