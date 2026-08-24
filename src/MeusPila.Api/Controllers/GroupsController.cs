using System.Security.Claims;
using MeusPila.Application.UseCases.Group.AddMember;
using MeusPila.Application.UseCases.Group.CreateGroup;
using MeusPila.Application.UseCases.Group.GetGroup;
using MeusPila.Application.UseCases.Group.ListGroups;
using MeusPila.Application.UseCases.Group.RemoveGroup;
using MeusPila.Application.UseCases.Group.RemoveMember;
using MeusPila.Application.UseCases.Group.UpdateGroup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeusPila.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly ILogger<GroupsController> _logger;

    public GroupsController(ILogger<GroupsController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<GetGroupOutputDto>> GetGroup(
        string id,
        [FromServices] IGetGroupUseCase getGroupUseCase
    )
    {
        try
        {
            string? requesterUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var input = new GetGroupInputDto { RequesterId = requesterUserId!, GroupId = id };

            var result = await getGroupUseCase.ExecuteAsync(input);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting group.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ListGroupsOutputDto>> ListGroups(
        [FromQuery] ListGroupsQueryDto query,
        [FromServices] IListGroupsUseCase listGroupsUseCase
    )
    {
        try
        {
            string? requesterUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var input = new ListGroupsInputDto
            {
                RequesterUserId = requesterUserId!,
                SearchTerm = query.SearchTerm,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                Ordering = query.Ordering,
            };

            var result = await listGroupsUseCase.ExecuteAsync(input);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing groups.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CreateGroupOutputDto>> CreateGroup(
        [FromBody] CreateGroupQueryDto query,
        [FromServices] ICreateGroupUseCase createGroupUseCase
    )
    {
        try
        {
            string? requesterUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var input = new CreateGroupInputDto
            {
                Name = query.Name,
                OwnerId = query.OwnerId,
                CreatedById = requesterUserId!,
            };
            var result = await createGroupUseCase.ExecuteAsync(input);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating group.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<UpdateGroupOutputDto>> UpdateGroup(
        string id,
        [FromBody] UpdateGroupQueryDto input,
        [FromServices] IUpdateGroupUseCase updateGroupUseCase
    )
    {
        try
        {
            string? updatedById = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var updateInput = new UpdateGroupInputDto
            {
                Id = id,
                Name = input.Name,
                OwnerId = input.OwnerId,
                UpdatedById = updatedById!,
            };
            var result = await updateGroupUseCase.ExecuteAsync(updateInput);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating group.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/add-member/{userId}")]
    [Authorize]
    public async Task<ActionResult<AddMemberOutputDto>> AddMember(
        string id,
        string userId,
        [FromServices] IAddMemberUseCase addMemberUseCase
    )
    {
        try
        {
            string? requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var input = new AddMemberInputDto
            {
                GroupId = id,
                MemberId = userId,
                UpdatedById = requesterId!,
            };
            var result = await addMemberUseCase.ExecuteAsync(input);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding member to group.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}/remove-member/{userId}")]
    [Authorize]
    public async Task<ActionResult<RemoveMemberOutputDto>> RemoveMember(
        string id,
        string userId,
        [FromServices] IRemoveMemberUseCase removeMemberUseCase
    )
    {
        try
        {
            string? requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var input = new RemoveMemberInputDto
            {
                GroupId = id,
                MemberId = userId,
                UpdatedById = requesterId!,
            };
            var result = await removeMemberUseCase.ExecuteAsync(input);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing member from group.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> RemoveGroup(string id, [FromServices] IRemoveGroupUseCase removeGroupUseCase)
    {
        try
        {
            string? requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var input = new RemoveGroupInputDto { GroupId = id, RequesterId = requesterId! };
            await removeGroupUseCase.ExecuteAsync(input);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing group.");
            return BadRequest(new { message = ex.Message });
        }
    }
}
