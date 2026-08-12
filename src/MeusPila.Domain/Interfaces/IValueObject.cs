namespace MeusPila.Domain.Interfaces;

public interface IValueObject<T>
{
    public T Value { get; }
    public bool Equals(IValueObject<T> other);
    public void Validate();
}
