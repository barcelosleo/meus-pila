using System.ComponentModel.DataAnnotations;

namespace MeusPila.Application.UseCases.Houshold.CreateHousehold;

public record CreateHouseholdInputDto
{
    [Required(ErrorMessage = "O nome do grupo é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome do grupo deve ter no máximo 100 caracteres")]
    public required string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "O ID do proprietário é obrigatório")]
    public required string OwnerId { get; init; }

    public string? CreatedById { get; init; }
}
