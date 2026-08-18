using System.Security.Claims;
using MeusPila.Application.UseCases.Houshold.AddMember;
using MeusPila.Application.UseCases.Houshold.CreateHousehold;
using MeusPila.Application.UseCases.Houshold.GetHousehold;
using MeusPila.Application.UseCases.Houshold.ListHouseholds;
using MeusPila.Application.UseCases.Houshold.RemoveMember;
using MeusPila.Application.UseCases.Houshold.UpdateHousehold;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeusPila.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HouseholdController : ControllerBase
{
    private readonly ILogger<HouseholdController> _logger;
    private readonly ICreateHouseholdUseCase _createHouseholdUseCase;
    private readonly IListHouseholdsUseCase _listHouseholdsUseCase;
    private readonly IGetHouseholdUseCase _getHouseholdUseCase;
    private readonly IUpdateHouseholdUseCase _updateHouseholdUseCase;
    private readonly IAddMemberUseCase _addMemberUseCase;
    private readonly IRemoveMemberUseCase _removeMemberUseCase;

    public HouseholdController(
        ILogger<HouseholdController> logger,
        ICreateHouseholdUseCase createHouseholdUseCase,
        IListHouseholdsUseCase listHouseholdsUseCase,
        IGetHouseholdUseCase getHouseholdUseCase,
        IUpdateHouseholdUseCase updateHouseholdUseCase,
        IAddMemberUseCase addMemberUseCase,
        IRemoveMemberUseCase removeMemberUseCase
    )
    {
        _logger = logger;
        _createHouseholdUseCase = createHouseholdUseCase;
        _listHouseholdsUseCase = listHouseholdsUseCase;
        _getHouseholdUseCase = getHouseholdUseCase;
        _updateHouseholdUseCase = updateHouseholdUseCase;
        _addMemberUseCase = addMemberUseCase;
        _removeMemberUseCase = removeMemberUseCase;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<GetHouseholdOutputDto>> GetHousehold(string id)
    {
        try
        {
            string? requesterUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var input = new GetHouseholdInputDto { RequesterId = requesterUserId!, HouseholdId = id };

            var result = await _getHouseholdUseCase.ExecuteAsync(input);
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
    public async Task<ActionResult<ListHouseholdsOutputDto>> ListHouseholds([FromQuery] ListHouseholdsQueryDto query)
    {
        try
        {
            string? requesterUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var input = new ListHouseholdsInputDto
            {
                RequesterUserId = requesterUserId!,
                SearchTerm = query.SearchTerm,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
            };

            var result = await _listHouseholdsUseCase.ExecuteAsync(input);
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
    public async Task<ActionResult<CreateHouseholdOutputDto>> CreateHousehold([FromBody] CreateHouseholdQueryDto query)
    {
        try
        {
            string? requesterUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var input = new CreateHouseholdInputDto
            {
                Name = query.Name,
                OwnerId = query.OwnerId,
                CreatedById = requesterUserId!,
            };
            var result = await _createHouseholdUseCase.ExecuteAsync(input);
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
    public async Task<ActionResult<UpdateHouseholdOutputDto>> UpdateHousehold(
        string id,
        [FromBody] UpdateHouseholdQueryDto input
    )
    {
        try
        {
            string? updatedById = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var updateInput = new UpdateHouseholdInputDto
            {
                Id = id,
                Name = input.Name,
                UpdatedById = updatedById!,
            };
            var result = await _updateHouseholdUseCase.ExecuteAsync(updateInput);
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
                HouseholdId = id,
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
                HouseholdId = id,
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
