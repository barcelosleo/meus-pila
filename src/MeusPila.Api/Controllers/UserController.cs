using MeusPila.Application.Interfaces;
using MeusPila.Application.UseCases.User.AuthenticateUser;
using MeusPila.Application.UseCases.User.CreateUser;
using MeusPila.Application.UseCases.User.ListUsers;
using MeusPila.Application.UseCases.User.RefreshToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeusPila.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IAuthenticateUserUseCase _authenticateUserUseCase;
    private readonly ICreateUserUseCase _createUserUseCase;
    private readonly IRefreshTokenUseCase _refreshTokenUseCase;
    private readonly IListUsersUseCase _listUsersUseCase;

    public UserController(
        ILogger<UserController> logger,
        IAuthenticateUserUseCase authenticateUserUseCase,
        ICreateUserUseCase createUserUseCase,
        IRefreshTokenUseCase refreshTokenUseCase,
        IListUsersUseCase listUsersUseCase
    )
    {
        _logger = logger;
        _authenticateUserUseCase = authenticateUserUseCase;
        _createUserUseCase = createUserUseCase;
        _refreshTokenUseCase = refreshTokenUseCase;
        _listUsersUseCase = listUsersUseCase;
    }

    [HttpPost("authenticate")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthenticateUserOutputDto>> AuthenticateUser(
        [FromBody] AuthenticateUserInputDto input
    )
    {
        try
        {
            var output = await _authenticateUserUseCase.ExecuteAsync(input);
            return Ok(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating user.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("refresh-token")]
    [Authorize]
    public async Task<ActionResult<RefreshTokenOutputDto>> RefreshToken([FromBody] RefreshTokenInputDto refreshToken)
    {
        try
        {
            var output = await _refreshTokenUseCase.ExecuteAsync(refreshToken);
            return Ok(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CreateUserOutputDto>> CreateUser([FromBody] CreateUserInputDto input)
    {
        try
        {
            var output = await _createUserUseCase.ExecuteAsync(input);
            return Ok(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("list")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ListUsersOutputDto>> ListUsers([FromQuery] ListUsersInputDto input)
    {
        try
        {
            var output = await _listUsersUseCase.ExecuteAsync(input);
            return Ok(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing users.");
            return BadRequest(new { message = ex.Message });
        }
    }
}
