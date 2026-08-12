using System.Linq.Expressions;
using MeusPila.Domain.Interfaces;

namespace MeusPila.Domain.Entities;

public class BankAccount : IEntity, IFinancialAccount
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid OwnerId { get; private set; }
    public List<IFinancialTransaction> Transactions { get; } = [];
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public BankAccount(string name, DateTime? createdAt = null, DateTime? updatedAt = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        CreatedAt = createdAt ?? DateTime.UtcNow;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
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
