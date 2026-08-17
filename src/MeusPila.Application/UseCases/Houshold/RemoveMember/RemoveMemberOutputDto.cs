namespace MeusPila.Application.UseCases.Houshold.RemoveMember;

public record MemberDto
{
    public required string Id;
    public required string Name;
}

public record RemoveMemberOutputDto
{
    public required string Id = string.Empty;
    public required string Name = string.Empty;
    public required DateTime CreatedAt;
    public required string CreatedById;
    public required DateTime UpdatedAt;
    public required string UpdatedById;
    public required List<MemberDto> Members;
}
