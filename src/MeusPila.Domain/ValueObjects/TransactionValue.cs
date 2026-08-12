using MeusPila.Domain.Interfaces;

namespace MeusPila.Domain.ValueObjects;

public class TransactionValue : IValueObject<decimal>
{
    public decimal Value { get; }

    public TransactionValue(decimal value)
    {
        Value = value;
        Validate();
    }

    public bool Equals(IValueObject<decimal> other)
    {
        return other != null && Value == other.Value;
    }

    public void Validate()
    {
        if (Value <= 0)
        {
            throw new ArgumentException("Transaction value must be greater than zero.");
        }
    }
}
