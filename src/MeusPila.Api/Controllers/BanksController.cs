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

    public BanksController(ILogger<BanksController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CreateBankOutputDto>> CreateBank(
        [FromBody] CreateBankQueryDto input,
        [FromServices] ICreateBankUseCase createBankUseCase
    )
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
            var output = await createBankUseCase.ExecuteAsync(inputDto);
            return CreatedAtAction(nameof(CreateBank), new { id = output.Id }, output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating bank.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<ListBanksOutputDto>> ListBanks(
        [FromQuery] ListBanksInputDto input,
        [FromServices] IListBanksUseCase listBanksUseCase
    )
    {
        try
        {
            var output = await listBanksUseCase.ExecuteAsync(input);
            return Ok(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing banks.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetBankOutputDto>> GetBank(
        [FromRoute] string id,
        [FromServices] IGetBankUseCase getBankUseCase
    )
    {
        try
        {
            var input = new GetBankInputDto { Id = id };
            var output = await getBankUseCase.ExecuteAsync(input);
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
        [FromBody] UpdateBankQueryDto input,
        [FromServices] IUpdateBankUseCase updateBankUseCase
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
            var output = await updateBankUseCase.ExecuteAsync(inputDto);
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
    public async Task<ActionResult> RemoveBank(
        [FromRoute] string id,
        [FromServices] IRemoveBankUseCase removeBankUseCase
    )
    {
        try
        {
            string? requesterUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var input = new RemoveBankInputDto { Id = id, RequesterId = requesterUserId! };
            await removeBankUseCase.ExecuteAsync(input);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing bank.");
            return BadRequest(new { message = ex.Message });
        }
    }
}
