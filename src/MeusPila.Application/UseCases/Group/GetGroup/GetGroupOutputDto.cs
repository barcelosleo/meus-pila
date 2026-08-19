namespace MeusPila.Application.UseCases.Group.GetGroup;

public record MemberDto
{
    public required string Id;
    public required string Name;
}

public record GetGroupOutputDto
{
    public required string Id = string.Empty;
    public required string Name = string.Empty;
    public required DateTime CreatedAt;
    public string? CreatedById;
    public DateTime? UpdatedAt;
    public string? UpdatedById;
    public required List<MemberDto> Members;
}
