using System.Linq.Expressions;
using MeusPila.Domain.Interfaces;

namespace MeusPila.Domain.Entities;

public class IndividualTransaction : FinancialTransaction
{
    public override void SetValue(decimal value)
    {
        if (value < 0)
        {
            throw new ArgumentException("Value cannot be negative.", nameof(value));
        }
        _value = value;
    }

    public override decimal GetValue()
    {
        return _value;
    }

    public override decimal GetValue(Expression<Func<FinancialTransaction, bool>> filter)
    {
        return _value;
    }
}
