using MeusPila.Domain.Enums;
using MeusPila.Domain.Interfaces;

namespace MeusPila.Domain.Entities;

public class InstallmentTransaction : CompositeTransaction
{
    public required int TotalInstallments { get; set; }
    public required bool IsRecurring { get; set; }

    public override void Validate()
    {
        base.Validate();
        if (ChildTransactions.Count < 2)
        {
            throw new InvalidOperationException(
                "An installment transaction must have at least two child transactions."
            );
        }

        TransactionType firstChildType = ChildTransactions.First().Type;

        if (ChildTransactions.Any(t => t.Type != firstChildType))
        {
            throw new InvalidOperationException(
                "All child transactions of an installment transaction must have the same type."
            );
        }
    }

    public override void AddChildTransaction(FinancialTransaction transaction)
    {
        if (ChildTransactions.Any(t => t.Type != Type))
        {
            throw new InvalidOperationException(
                "All child transactions of an installment transaction must have the same type."
            );
        }
        base.AddChildTransaction(transaction);
    }

    public override void AddChildTransactions(IEnumerable<FinancialTransaction> transactions)
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
