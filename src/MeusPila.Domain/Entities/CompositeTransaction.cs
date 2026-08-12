using System.Linq.Expressions;
using MeusPila.Domain.Enums;
using MeusPila.Domain.Interfaces;

namespace MeusPila.Domain.Entities;

public class CompositeTransaction : IEntity, IFinancialTransaction
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public bool Performed { get; set; }
    public TransactionType Type { get; protected set; }
    public FinancialCategory Category { get; set; }
    private readonly List<IFinancialTransaction> _childTransactions = [];
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public CompositeTransaction(
        DateOnly date,
        FinancialCategory category,
        DateTime? createdAt = null,
        DateTime? updatedAt = null
    )
    {
        Id = Guid.NewGuid();
        Date = date;
        Performed = false;
        Type = GetValue() < 0 ? TransactionType.Outflow : TransactionType.Inflow;
        Category = category;
        CreatedAt = createdAt ?? DateTime.UtcNow;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
    }

    public CompositeTransaction(
        Guid id,
        DateOnly date,
        FinancialCategory category,
        List<IFinancialTransaction> childTransactions,
        DateTime? createdAt = null,
        DateTime? updatedAt = null
    )
    {
        Id = id;
        Date = date;
        Performed = false;
        Category = category;
        _childTransactions = childTransactions;
        Type = GetValue() < 0 ? TransactionType.Outflow : TransactionType.Inflow;
        CreatedAt = createdAt ?? DateTime.UtcNow;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
    }

    public virtual void Validate()
    {
        if (_childTransactions.Any(t => t.Category != Category))
        {
            throw new InvalidOperationException(
                "All child transactions must have the same category as the composite transaction."
            );
        }
    }

    public List<IFinancialTransaction> GetChildTransactions()
    {
        return _childTransactions;
    }

    private static decimal CalculateNetValue(IEnumerable<IFinancialTransaction> transactions)
    {
        return transactions.Sum(t =>
            t.Type switch
            {
                TransactionType.Inflow => t.GetValue(),
                TransactionType.Outflow => -t.GetValue(),
                _ => 0m,
            }
        );
    }

    public decimal GetValue()
    {
        return CalculateNetValue(_childTransactions);
    }

    public decimal GetValue(Expression<Func<IFinancialTransaction, bool>> filter)
    {
        var filteredTransactions = _childTransactions.AsQueryable().Where(filter);
        return CalculateNetValue(filteredTransactions);
    }

    public virtual void AddChildTransaction(IFinancialTransaction transaction)
    {
        if (transaction.Category != Category)
        {
            throw new InvalidOperationException(
                "Child transaction category must match the composite transaction category."
            );
        }

        if (_childTransactions.Contains(transaction))
        {
            throw new InvalidOperationException(
                "The specified transaction is already a child of this composite transaction."
            );
        }

        _childTransactions.Add(transaction);

        Type = GetValue() < 0 ? TransactionType.Outflow : TransactionType.Inflow;
    }

    public virtual void AddChildTransactions(IEnumerable<IFinancialTransaction> transactions)
    {
        if (transactions.Any(t => t.Category != Category))
        {
            throw new InvalidOperationException(
                "All child transaction categories must match the composite transaction category."
            );
        }

        if (transactions.Any(_childTransactions.Contains))
        {
            throw new InvalidOperationException(
                "One or more specified transactions are already children of this composite transaction."
            );
        }

        foreach (var transaction in transactions)
        {
            _childTransactions.Add(transaction);
        }

        Type = GetValue() < 0 ? TransactionType.Outflow : TransactionType.Inflow;
    }

    public void RemoveChildTransaction(IFinancialTransaction transaction)
    {
        if (!_childTransactions.Contains(transaction))
        {
            throw new InvalidOperationException(
                "The specified transaction is not a child of this composite transaction."
            );
        }
        _childTransactions.Remove(transaction);
        Type = GetValue() < 0 ? TransactionType.Outflow : TransactionType.Inflow;
    }

    public void RemoveChildTransactions(IEnumerable<IFinancialTransaction> transactions)
    {
        if (transactions.Any(t => !_childTransactions.Contains(t)))
        {
            throw new InvalidOperationException(
                "One or more specified transactions are not children of this composite transaction."
            );
        }

        foreach (var transaction in transactions)
        {
            _childTransactions.Remove(transaction);
        }

        Type = GetValue() < 0 ? TransactionType.Outflow : TransactionType.Inflow;
    }

    public virtual void MarkAsPerformed()
    {
        Performed = true;
        foreach (var transaction in _childTransactions)
        {
            transaction.Performed = true;
        }
    }

    public virtual void MarkAsNotPerformed()
    {
        Performed = false;
        foreach (var transaction in _childTransactions)
        {
            transaction.Performed = false;
        }
    }
}
