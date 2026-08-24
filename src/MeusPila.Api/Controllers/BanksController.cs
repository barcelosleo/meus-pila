using MeusPila.Application.UseCases.Bank.ListBanks;
using Microsoft.AspNetCore.Mvc;

namespace MeusPila.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BanksController : ControllerBase
{
    private readonly ILogger<BanksController> _logger;
    private readonly IListBanksUseCase _listBanksUseCase;

    public BanksController(ILogger<BanksController> logger, IListBanksUseCase listBanksUseCase)
    {
        _logger = logger;
        _listBanksUseCase = listBanksUseCase;
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
