using MeusPila.Domain.Entities;

namespace MeusPila.Application.Interfaces;

/// <summary>
/// Represents a repository interface for performing CRUD operations on FinancialTransaction entities.
/// </summary>
public interface IFinancialTransactionRepository : IRepository<FinancialTransaction> { }
