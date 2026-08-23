namespace MeusPila.Domain.Interfaces;

public interface IPagedList<T>
    where T : class
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public IEnumerable<T> Items { get; init; }
}
