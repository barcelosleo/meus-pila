using System.Linq.Expressions;
using MeusPila.Domain.Enums;

namespace MeusPila.Domain.Entities;

public class CompositeTransaction : FinancialTransaction
{
    public virtual ICollection<FinancialTransaction> ChildTransactions { get; set; } = [];

    public override void SetValue(decimal value)
    {
        throw new InvalidOperationException(
            "Cannot set value directly on a composite transaction. The value is derived from its child transactions."
        );
    }

    public virtual void Validate()
    {
        if (ChildTransactions.Any(t => t.Category != Category))
        {
            throw new InvalidOperationException(
                "All child transactions must have the same category as the composite transaction."
            );
        }
    }

    private static decimal CalculateNetValue(IEnumerable<FinancialTransaction> transactions)
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

    public override decimal GetValue()
    {
        return CalculateNetValue(ChildTransactions);
    }

    public override decimal GetValue(Expression<Func<FinancialTransaction, bool>> filter)
    {
        var filteredTransactions = ChildTransactions.AsQueryable().Where(filter);
        return CalculateNetValue(filteredTransactions);
    }

    public virtual void AddChildTransaction(FinancialTransaction transaction)
    {
        if (transaction.Category != Category)
        {
            throw new InvalidOperationException(
                "Child transaction category must match the composite transaction category."
            );
        }

        if (ChildTransactions.Contains(transaction))
        {
            throw new InvalidOperationException(
                "The specified transaction is already a child of this composite transaction."
            );
        }

        ChildTransactions.Add(transaction);

        Type = GetValue() < 0 ? TransactionType.Outflow : TransactionType.Inflow;
    }

    public virtual void AddChildTransactions(IEnumerable<FinancialTransaction> transactions)
    {
        if (transactions.Any(t => t.Category != Category))
        {
            throw new InvalidOperationException(
                "All child transaction categories must match the composite transaction category."
            );
        }

        if (transactions.Any(ChildTransactions.Contains))
        {
            throw new InvalidOperationException(
                "One or more specified transactions are already children of this composite transaction."
            );
        }

        foreach (var transaction in transactions)
        {
            ChildTransactions.Add(transaction);
        }

        Type = GetValue() < 0 ? TransactionType.Outflow : TransactionType.Inflow;
    }

    public void RemoveChildTransaction(FinancialTransaction transaction)
    {
        if (!ChildTransactions.Contains(transaction))
        {
            throw new InvalidOperationException(
                "The specified transaction is not a child of this composite transaction."
            );
        }
        ChildTransactions.Remove(transaction);
        Type = GetValue() < 0 ? TransactionType.Outflow : TransactionType.Inflow;
    }

    public void RemoveChildTransactions(IEnumerable<FinancialTransaction> transactions)
    {
        if (transactions.Any(t => !ChildTransactions.Contains(t)))
        {
            throw new InvalidOperationException(
                "One or more specified transactions are not children of this composite transaction."
            );
        }

        foreach (var transaction in transactions)
        {
            ChildTransactions.Remove(transaction);
        }

        Type = GetValue() < 0 ? TransactionType.Outflow : TransactionType.Inflow;
    }

    public virtual void MarkAsPerformed()
    {
        Performed = true;
        foreach (var transaction in ChildTransactions)
        {
            transaction.Performed = true;
        }
    }

    public virtual void MarkAsNotPerformed()
    {
        Performed = false;
        foreach (var transaction in ChildTransactions)
        {
            transaction.Performed = false;
        }
    }
}
