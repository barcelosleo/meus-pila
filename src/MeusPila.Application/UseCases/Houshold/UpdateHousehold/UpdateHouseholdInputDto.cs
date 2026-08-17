using System.ComponentModel.DataAnnotations;

namespace MeusPila.Application.UseCases.Houshold.UpdateHousehold;

public record UpdateHouseholdInputDto
{
    [Required(ErrorMessage = "O id do grupo é obrigatório")]
    public required string Id;

    [StringLength(100, ErrorMessage = "O nome do grupo deve ter no máximo 100 caracteres")]
    public string? Name = string.Empty;

    public string? UpdatedById = string.Empty;
}
