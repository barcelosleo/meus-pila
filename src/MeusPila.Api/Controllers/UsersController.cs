using System.Security.Claims;
using MeusPila.Application.UseCases.Admin.CreateUser;
using MeusPila.Application.UseCases.Admin.ListUsers;
using MeusPila.Application.UseCases.User.AuthenticateUser;
using MeusPila.Application.UseCases.User.GetUser;
using MeusPila.Application.UseCases.User.RefreshToken;
using MeusPila.Application.UseCases.User.RemoveUser;
using MeusPila.Application.UseCases.User.UpdateUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeusPila.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IAuthenticateUserUseCase _authenticateUserUseCase;
    private readonly ICreateUserUseCase _createUserUseCase;
    private readonly IRefreshTokenUseCase _refreshTokenUseCase;
    private readonly IListUsersUseCase _listUsersUseCase;
    private readonly IGetUserUseCase _getUserUseCase;
    private readonly IRemoveUserUseCase _removeUserUseCase;

    private readonly IUpdateUserUseCase _updateUserUseCase;

    public UsersController(
        ILogger<UsersController> logger,
        IAuthenticateUserUseCase authenticateUserUseCase,
        ICreateUserUseCase createUserUseCase,
        IRefreshTokenUseCase refreshTokenUseCase,
        IListUsersUseCase listUsersUseCase,
        IGetUserUseCase getUserUseCase,
        IRemoveUserUseCase removeUserUseCase,
        IUpdateUserUseCase updateUserUseCase
    )
    {
        _logger = logger;
        _authenticateUserUseCase = authenticateUserUseCase;
        _createUserUseCase = createUserUseCase;
        _refreshTokenUseCase = refreshTokenUseCase;
        _listUsersUseCase = listUsersUseCase;
        _getUserUseCase = getUserUseCase;
        _removeUserUseCase = removeUserUseCase;
        _updateUserUseCase = updateUserUseCase;
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

    [HttpPost]
    [Authorize(Roles = "Admin,GroupManager")]
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

    [HttpGet]
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

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> RemoveUser([FromRoute] string id)
    {
        string? requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            await _removeUserUseCase.ExecuteAsync(new RemoveUserInputDto { Id = id, RequesterId = requesterId! });
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [HttpPatch("{id}")]
    [Authorize]
    public async Task<ActionResult<UpdateUserOutputDto>> UpdateUser(
        [FromRoute] string id,
        [FromBody] UpdateUserQueryDto input
    )
    {
        string? requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var inputDto = new UpdateUserInputDto
            {
                Id = id,
                Name = input.Name,
                Email = input.Email,
                CurrentPassword = input.CurrentPassword,
                NewPassword = input.NewPassword,
                IsActive = input.IsActive,
                UpdatedById = requesterId!,
            };
            var output = await _updateUserUseCase.ExecuteAsync(inputDto);
            return Ok(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user.");
            return BadRequest(new { message = ex.Message });
        }
    }
}
