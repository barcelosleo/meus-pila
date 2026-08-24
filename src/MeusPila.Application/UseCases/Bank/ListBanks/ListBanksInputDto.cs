using System.Text.Json.Serialization;
using MeusPila.Application.Behaviours;

namespace MeusPila.Application.UseCases.Bank.ListBanks;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BanksOrdering
{
    IdAsc,
    IdDesc,
    NameAsc,
    NameDesc,
    CreatedAtAsc,
    CreatedAtDesc,
    UpdatedAtAsc,
    UpdatedAtDesc,
    CreatedByAsc,
    CreatedByDesc,
}

public record ListBanksInputDto : AbstractInputDtoListUseCase<BanksOrdering> { }
