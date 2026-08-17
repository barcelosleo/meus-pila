using System.ComponentModel.DataAnnotations;

namespace MeusPila.Application.UseCases.Houshold.GetHousehold;

public record GetHouseholdInputDto
{
    [Required(ErrorMessage = "O id do grupo é obrigatório")]
    public required string HouseholdId { get; init; }

    [Required(ErrorMessage = "O id de quem requisitou é obrigatório")]
    public required string RequesterId { get; init; }
}
