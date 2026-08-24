using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MeusPila.Application.Behaviours;
using MeusPila.Application.Interfaces;
using MeusPila.Domain.Common;

namespace MeusPila.Application.UseCases.Admin.ListUsers;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UsersOrdering
{
    IdAsc,
    IdDesc,
    NameAsc,
    NameDesc,
    EmailAsc,
    EmailDesc,
    RoleAsc,
    RoleDesc,
    CreatedAtAsc,
    CreatedAtDesc,
    UpdatedAtAsc,
    UpdatedAtDesc,
    CreatedByAsc,
    CreatedByDesc,
}

public record ListUsersInputDto : AbstractInputDtoListUseCase<UsersOrdering> { }
