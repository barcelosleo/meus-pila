using System.Security.Claims;
using MeusPila.Application.UseCases.Group.AddMember;
using MeusPila.Application.UseCases.Group.CreateGroup;
using MeusPila.Application.UseCases.Group.GetGroup;
using MeusPila.Application.UseCases.Group.ListGroups;
using MeusPila.Application.UseCases.Group.RemoveMember;
using MeusPila.Application.UseCases.Group.UpdateGroup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeusPila.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupController : ControllerBase
{
    private readonly ILogger<GroupController> _logger;
    private readonly ICreateGroupUseCase _createGroupUseCase;
    private readonly IListGroupsUseCase _listGroupsUseCase;
    private readonly IGetGroupUseCase _getGroupUseCase;
    private readonly IUpdateGroupUseCase _updateGroupUseCase;
    private readonly IAddMemberUseCase _addMemberUseCase;
    private readonly IRemoveMemberUseCase _removeMemberUseCase;

    public GroupController(
        ILogger<GroupController> logger,
        ICreateGroupUseCase createGroupUseCase,
        IListGroupsUseCase listGroupsUseCase,
        IGetGroupUseCase getGroupUseCase,
        IUpdateGroupUseCase updateGroupUseCase,
        IAddMemberUseCase addMemberUseCase,
        IRemoveMemberUseCase removeMemberUseCase
    )
    {
        _logger = logger;
        _createGroupUseCase = createGroupUseCase;
        _listGroupsUseCase = listGroupsUseCase;
        _getGroupUseCase = getGroupUseCase;
        _updateGroupUseCase = updateGroupUseCase;
        _addMemberUseCase = addMemberUseCase;
        _removeMemberUseCase = removeMemberUseCase;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<GetGroupOutputDto>> GetGroup(string id)
    {
        try
        {
            string? requesterUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var input = new GetGroupInputDto { RequesterId = requesterUserId!, GroupId = id };

            var result = await _getGroupUseCase.ExecuteAsync(input);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting household.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ListGroupsOutputDto>> ListGroups([FromQuery] ListGroupsQueryDto query)
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
            };

            var result = await _listGroupsUseCase.ExecuteAsync(input);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing households.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CreateGroupOutputDto>> CreateGroup([FromBody] CreateGroupQueryDto query)
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
            var result = await _createGroupUseCase.ExecuteAsync(input);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating household.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<UpdateGroupOutputDto>> UpdateGroup(string id, [FromBody] UpdateGroupQueryDto input)
    {
        try
        {
            string? updatedById = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var updateInput = new UpdateGroupInputDto
            {
                Id = id,
                Name = input.Name,
                UpdatedById = updatedById!,
            };
            var result = await _updateGroupUseCase.ExecuteAsync(updateInput);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating household.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/add-member/{userId}")]
    [Authorize]
    public async Task<ActionResult<AddMemberOutputDto>> AddMember(string id, string userId)
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
            var result = await _addMemberUseCase.ExecuteAsync(input);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding member to household.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}/remove-member/{userId}")]
    [Authorize]
    public async Task<ActionResult<RemoveMemberOutputDto>> RemoveMember(string id, string userId)
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
            var result = await _removeMemberUseCase.ExecuteAsync(input);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing member from household.");
            return BadRequest(new { message = ex.Message });
        }
    }
}
