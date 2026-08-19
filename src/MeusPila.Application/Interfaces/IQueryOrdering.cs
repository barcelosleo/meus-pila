namespace MeusPila.Application.Interfaces;

public interface IQueryOrdering
{
    public string OrderingPropertyName { get; }
    public bool Descending { get; }
}
