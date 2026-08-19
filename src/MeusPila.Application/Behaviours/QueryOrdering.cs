using MeusPila.Application.Interfaces;

namespace MeusPila.Application.Behaviours;

public record QueryOrdering : IQueryOrdering
{
    public string OrderingPropertyName { get; init; } = "Id";
    public bool Descending { get; init; } = false;
}

public record QueryOrdering<T> : IQueryOrdering
{
    public string OrderingPropertyName { get; init; } = typeof(T).Name;
    public bool Descending { get; init; } = false;
}
