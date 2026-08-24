using System.Linq.Expressions;
using MeusPila.Application.Interfaces;
using MeusPila.Application.Services;
using BankEntity = MeusPila.Domain.Entities.Bank;

namespace MeusPila.Application.UseCases.Bank.ListBanks;

public interface IListBanksUseCase : IUseCase<ListBanksInputDto, ListBanksOutputDto> { }

public class ListBanksUseCase : IListBanksUseCase
{
    private readonly IBankRepository _bankRepository;

    public ListBanksUseCase(IBankRepository bankRepository)
    {
        _bankRepository = bankRepository;
    }

    public async Task<ListBanksOutputDto> ExecuteAsync(ListBanksInputDto input)
    {
        string? searchTerm = input.SearchTerm?.Trim();
        string? normalizedSearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.ToLower();

        IEnumerable<BanksOrdering> inputOrdering = input.Ordering ?? [BanksOrdering.NameAsc];

        var ordering = QueryOrderingMapper<BankEntity, BanksOrdering>.MapQueryOrderings(inputOrdering);

        Expression<Func<BankEntity, bool>> predicate = bank =>
            normalizedSearchTerm == null
            || bank.Name.ToLower().Contains(normalizedSearchTerm)
            || bank.Code.ToLower().Contains(normalizedSearchTerm);

        IEnumerable<BankEntity> banks = await _bankRepository.GetAllAsync(
            predicate,
            input.PageNumber,
            input.PageSize,
            ordering
        );
        int totalCount = await _bankRepository.CountAsync(predicate);

        return new ListBanksOutputDto
        {
            PageNumber = input.PageNumber,
            PageSize = input.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)input.PageSize),
            Items = banks.Select(bank => new BankDto
            {
                Id = bank.Id,
                Name = bank.Name,
                Code = bank.Code,
                CreatedAt = bank.CreatedAt,
                CreatedById = bank.CreatedById?.ToString(),
                CreatedByName = bank.CreatedBy?.Name,
                UpdatedAt = bank.UpdatedAt,
                UpdatedById = bank.UpdatedById?.ToString(),
                UpdatedByName = bank.UpdatedBy?.Name,
            }),
        };
    }
}
