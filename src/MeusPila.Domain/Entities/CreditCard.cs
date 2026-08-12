using System.Linq.Expressions;
using MeusPila.Domain.Interfaces;

namespace MeusPila.Domain.Entities;

public class CreditCard : IEntity, IFinancialAccount
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly ClosingDate { get; set; }
    public DateOnly DueDate { get; set; }
    public decimal Limit { get; private set; }

    public Guid OwnerId { get; private set; }

    public List<IFinancialTransaction> Transactions => [];

    public void SetLimit(decimal limit)
    {
        if (limit < 0)
        {
            throw new ArgumentException("Limit cannot be negative.", nameof(limit));
        }
        Limit = limit;
    }

    public decimal GetBalance()
    {
        return Transactions.Sum(t => t.GetValue());
    }

    public decimal GetBalance(Expression<Func<IFinancialTransaction, bool>> filter)
    {
        var filteredTransactions = Transactions.AsQueryable().Where(filter);
        return filteredTransactions.Sum(t => t.GetValue(filter));
    }
}
