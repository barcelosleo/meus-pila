using System.ComponentModel.DataAnnotations;

namespace MeusPila.Application.UseCases.Houshold.UpdateHousehold;

public record UpdateHouseholdInputDto
{
    public required string Id;

    public string? Name = string.Empty;

    public string? UpdatedById = string.Empty;
}

public record UpdateHouseholdQueryDto
{
    [StringLength(100, ErrorMessage = "O nome do grupo deve ter no máximo 100 caracteres")]
    public string? Name = string.Empty;
}
