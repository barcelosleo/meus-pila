using System.ComponentModel.DataAnnotations.Schema;

namespace MeusPila.Domain.Entities;

public class AccountTransaction
{
    public required Guid FinancialAccountId { get; set; }

    [ForeignKey("FinancialAccountId")]
    public required FinancialAccount FinancialAccount { get; set; }
    public required Guid FinancialTransactionId { get; set; }

    [ForeignKey("FinancialTransactionId")]
    public required FinancialTransaction FinancialTransaction { get; set; }
}
