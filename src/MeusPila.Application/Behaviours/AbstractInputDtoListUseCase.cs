using System.ComponentModel.DataAnnotations;
using MeusPila.Domain.Common;

namespace MeusPila.Application.Behaviours;

public abstract record AbstractInputDtoListUseCase<TOrderingEnum>
    where TOrderingEnum : Enum
{
    public string? SearchTerm { get; init; }
    public IEnumerable<TOrderingEnum>? Ordering { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Número da página deve ser maior ou igual a 1.")]
    public int PageNumber { get; init; } = 1;

    [Range(5, PagedListProperties.MaxPageSize, ErrorMessage = "Tamanho da página deve ser entre {0} e {1}.")]
    public int PageSize { get; init; } = PagedListProperties.DefaultPageSize;
}
