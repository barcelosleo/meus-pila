namespace MeusPila.Application.UseCases.Group.RemoveMember;

public record MemberDto
{
    public required string Id;
    public required string Name;
}

public record RemoveMemberOutputDto
{
    public required string Id;
    public required string Name;
    public required DateTime CreatedAt;
    public string? CreatedById;
    public DateTime? UpdatedAt;
    public string? UpdatedById;
    public required List<MemberDto> Members;
}
