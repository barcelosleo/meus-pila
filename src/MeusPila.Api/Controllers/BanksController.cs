using System.Security.Claims;
using MeusPila.Application.UseCases.Bank.CreateBank;
using MeusPila.Application.UseCases.Bank.GetBank;
using MeusPila.Application.UseCases.Bank.ListBanks;
using MeusPila.Application.UseCases.Bank.RemoveBank;
using MeusPila.Application.UseCases.Bank.UpdateBank;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeusPila.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BanksController : ControllerBase
{
    private readonly ILogger<BanksController> _logger;
    private readonly IListBanksUseCase _listBanksUseCase;
    private readonly ICreateBankUseCase _createBankUseCase;
    private readonly IGetBankUseCase _getBankUseCase;
    private readonly IUpdateBankUseCase _updateBankUseCase;
    private readonly IRemoveBankUseCase _removeBankUseCase;

    public BanksController(
        ILogger<BanksController> logger,
        IListBanksUseCase listBanksUseCase,
        ICreateBankUseCase createBankUseCase,
        IGetBankUseCase getBankUseCase,
        IUpdateBankUseCase updateBankUseCase,
        IRemoveBankUseCase removeBankUseCase
    )
    {
        _logger = logger;
        _listBanksUseCase = listBanksUseCase;
        _createBankUseCase = createBankUseCase;
        _getBankUseCase = getBankUseCase;
        _updateBankUseCase = updateBankUseCase;
        _removeBankUseCase = removeBankUseCase;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CreateBankOutputDto>> CreateBank([FromBody] CreateBankQueryDto input)
    {
        try
        {
            string? requesterUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var inputDto = new CreateBankInputDto
            {
                Name = input.Name,
                Code = input.Code,
                CreatedById = requesterUserId!,
            };
            var output = await _createBankUseCase.ExecuteAsync(inputDto);
            return CreatedAtAction(nameof(CreateBank), new { id = output.Id }, output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating bank.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<ListBanksOutputDto>> ListBanks([FromQuery] ListBanksInputDto input)
    {
        try
        {
            var output = await _listBanksUseCase.ExecuteAsync(input);
            return Ok(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing banks.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetBankOutputDto>> GetBank([FromRoute] string id)
    {
        try
        {
            var input = new GetBankInputDto { Id = id };
            var output = await _getBankUseCase.ExecuteAsync(input);
            return Ok(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bank.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UpdateBankOutputDto>> UpdateBank(
        [FromRoute] string id,
        [FromBody] UpdateBankQueryDto input
    )
    {
        try
        {
            string? requesterUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var inputDto = new UpdateBankInputDto
            {
                Id = id,
                Name = input.Name,
                Code = input.Code,
                UpdatedById = requesterUserId!,
            };
            var output = await _updateBankUseCase.ExecuteAsync(inputDto);
            return Ok(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating bank.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> RemoveBank([FromRoute] string id)
    {
        try
        {
            string? requesterUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var input = new RemoveBankInputDto { Id = id, RequesterId = requesterUserId! };
            await _removeBankUseCase.ExecuteAsync(input);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing bank.");
            return BadRequest(new { message = ex.Message });
        }
    }
}
