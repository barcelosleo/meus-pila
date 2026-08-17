using System.Linq.Expressions;
using MeusPila.Domain.Entities;
using MeusPila.Domain.Enums;

namespace MeusPila.Domain.Interfaces;

public interface IFinancialTransaction
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public bool Performed { get; set; }
    public TransactionType Type { get; }
    public Guid CategoryId { get; set; }
    public FinancialCategory Category { get; set; }
    public Guid? ParentTransactionId { get; set; }
    public IFinancialTransaction? ParentTransaction { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public decimal GetValue();
    public decimal GetValue(Expression<Func<IFinancialTransaction, bool>> filter);
}
