using System.Linq.Expressions;
using MeusPila.Domain.Entities;

namespace MeusPila.Domain.Interfaces;

public interface IFinancialAccount
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid OwnerId { get; }
    public IEnumerable<FinancialTransaction> Transactions { get; }
    public decimal GetBalance();
    public decimal GetBalance(Expression<Func<FinancialTransaction, bool>> filter);
}
