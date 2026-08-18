using System.Security.Claims;
using MeusPila.Application.Interfaces;
using MeusPila.Application.UseCases.User.AuthenticateUser;
using MeusPila.Application.UseCases.User.CreateUser;
using MeusPila.Application.UseCases.User.GetUser;
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
    private readonly IGetUserUseCase _getUserUseCase;

    public UserController(
        ILogger<UserController> logger,
        IAuthenticateUserUseCase authenticateUserUseCase,
        ICreateUserUseCase createUserUseCase,
        IRefreshTokenUseCase refreshTokenUseCase,
        IListUsersUseCase listUsersUseCase,
        IGetUserUseCase getUserUseCase
    )
    {
        _logger = logger;
        _authenticateUserUseCase = authenticateUserUseCase;
        _createUserUseCase = createUserUseCase;
        _refreshTokenUseCase = refreshTokenUseCase;
        _listUsersUseCase = listUsersUseCase;
        _getUserUseCase = getUserUseCase;
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
    [AllowAnonymous]
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

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<GetUserOutputDto>> GetCurrentUser()
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var output = await _getUserUseCase.ExecuteAsync(
                new GetUserInputDto { Id = userId!, RequesterId = userId! }
            );
            return Ok(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user.");
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

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<GetUserOutputDto>> GetUserById([FromRoute] string id)
    {
        string? requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var output = await _getUserUseCase.ExecuteAsync(
                new GetUserInputDto { Id = id, RequesterId = requesterId! }
            );
            return Ok(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by id.");
            return BadRequest(new { message = ex.Message });
        }
    }
}
