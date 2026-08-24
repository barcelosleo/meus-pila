namespace MeusPila.Application.UseCases.Bank.CreateBank;

using System.ComponentModel.DataAnnotations;

public record CreateBankQueryDto
{
    [Required(ErrorMessage = "O nome do banco é obrigatório.")]
    [MaxLength(100, ErrorMessage = "O nome do banco não pode ter mais de 100 caracteres.")]
    public required string Name { get; init; }

    [Required(ErrorMessage = "O código do banco é obrigatório.")]
    [MaxLength(10, ErrorMessage = "O código do banco não pode ter mais de 10 caracteres.")]
    public required string Code { get; init; }
}

public record CreateBankInputDto : CreateBankQueryDto
{
    [Required(ErrorMessage = "O ID do usuário que está criando o banco é obrigatório.")]
    public required string CreatedById { get; init; }
}
