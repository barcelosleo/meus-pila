using MeusPila.Domain.Enums;
using MeusPila.Domain.Interfaces;

namespace MeusPila.Domain.Entities;

public class InstallmentTransaction : CompositeTransaction
{
    public InstallmentTransaction(
        DateOnly date,
        TransactionType type,
        FinancialCategory category,
        DateTime? createdAt = null,
        DateTime? updatedAt = null
    )
        : base(date, category, createdAt, updatedAt)
    {
        Type = type;
    }

    public InstallmentTransaction(
        Guid id,
        DateOnly date,
        TransactionType type,
        FinancialCategory category,
        List<IFinancialTransaction> childTransactions,
        DateTime? createdAt = null,
        DateTime? updatedAt = null
    )
        : base(id, date, category, childTransactions, createdAt, updatedAt)
    {
        Type = type;
        if (GetChildTransactions().Any(t => t.Type != Type))
        {
            throw new InvalidOperationException(
                "All child transactions of an installment transaction must have the same type."
            );
        }
    }

    public override void Validate()
    {
        base.Validate();
        if (GetChildTransactions().Count < 2)
        {
            throw new InvalidOperationException(
                "An installment transaction must have at least two child transactions."
            );
        }

        TransactionType firstChildType = GetChildTransactions().First().Type;

        if (GetChildTransactions().Any(t => t.Type != firstChildType))
        {
            throw new InvalidOperationException(
                "All child transactions of an installment transaction must have the same type."
            );
        }
    }

    public override void AddChildTransaction(IFinancialTransaction transaction)
    {
        if (GetChildTransactions().Any(t => t.Type != Type))
        {
            throw new InvalidOperationException(
                "All child transactions of an installment transaction must have the same type."
            );
        }
        base.AddChildTransaction(transaction);
    }

    public override void AddChildTransactions(IEnumerable<IFinancialTransaction> transactions)
    {
        if (transactions.Any(t => t.Type != Type))
        {
            throw new InvalidOperationException(
                "All child transactions of an installment transaction must have the same type."
            );
        }
        base.AddChildTransactions(transactions);
    }

    public override void MarkAsPerformed()
    {
        throw new InvalidOperationException(
            "Installment transactions cannot be marked as performed directly. Mark each child transaction individually."
        );
    }

    public override void MarkAsNotPerformed()
    {
        throw new InvalidOperationException(
            "Installment transactions cannot be marked as not performed directly. Mark each child transaction individually."
        );
    }
}
