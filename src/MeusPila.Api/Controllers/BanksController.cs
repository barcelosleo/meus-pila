using System.Security.Claims;
using MeusPila.Application.UseCases.Bank.CreateBank;
using MeusPila.Application.UseCases.Bank.ListBanks;
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

    public BanksController(
        ILogger<BanksController> logger,
        IListBanksUseCase listBanksUseCase,
        ICreateBankUseCase createBankUseCase
    )
    {
        _logger = logger;
        _listBanksUseCase = listBanksUseCase;
        _createBankUseCase = createBankUseCase;
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
}
