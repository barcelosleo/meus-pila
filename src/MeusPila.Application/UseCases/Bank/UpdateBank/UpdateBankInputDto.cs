namespace MeusPila.Application.UseCases.Bank.UpdateBank;

using System.ComponentModel.DataAnnotations;

public record UpdateBankQueryDto
{
    [MaxLength(100, ErrorMessage = "O campo 'Name' deve ter no máximo {1} caracteres.")]
    public string Name { get; init; } = string.Empty;

    [MaxLength(10, ErrorMessage = "O campo 'Code' deve ter no máximo {1} caracteres.")]
    public string Code { get; init; } = string.Empty;
}

public record UpdateBankInputDto : UpdateBankQueryDto
{
    [Required(ErrorMessage = "O campo 'Id' é obrigatório.")]
    public required string Id { get; init; }

    [Required(ErrorMessage = "O campo 'UpdatedById' é obrigatório.")]
    public required string UpdatedById { get; init; }
}
