using System.Linq.Expressions;
using MeusPila.Domain.Enums;
using MeusPila.Domain.Interfaces;
using MeusPila.Domain.ValueObjects;

namespace MeusPila.Domain.Entities;

public class FinancialTransaction : IEntity, IFinancialTransaction
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public bool Performed { get; set; }
    public TransactionType Type { get; set; }
    public FinancialCategory Category { get; set; }
    private TransactionValue Value { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public FinancialTransaction(
        decimal value,
        DateOnly date,
        TransactionType type,
        FinancialCategory category,
        bool performed = false,
        DateTime? createdAt = null,
        DateTime? updatedAt = null
    )
    {
        Id = Guid.NewGuid();
        Date = date;
        Performed = performed;
        Type = type;
        Category = category;
        Value = new TransactionValue(value);
        CreatedAt = createdAt ?? DateTime.UtcNow;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
    }

    public decimal GetValue()
    {
        return Value.Value;
    }

    public decimal GetValue(Expression<Func<IFinancialTransaction, bool>> filter)
    {
        return GetValue();
    }
}
