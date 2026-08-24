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

    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;
    }

    [HttpPost("authenticate")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthenticateUserOutputDto>> AuthenticateUser(
        [FromBody] AuthenticateUserInputDto input,
        [FromServices] IAuthenticateUserUseCase authenticateUserUseCase
    )
    {
        try
        {
            var output = await authenticateUserUseCase.ExecuteAsync(input);
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
    public async Task<ActionResult<RefreshTokenOutputDto>> RefreshToken(
        [FromBody] RefreshTokenInputDto refreshToken,
        [FromServices] IRefreshTokenUseCase refreshTokenUseCase
    )
    {
        try
        {
            var output = await refreshTokenUseCase.ExecuteAsync(refreshToken);
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
    public async Task<ActionResult<GetUserOutputDto>> GetCurrentUser([FromServices] IGetUserUseCase getUserUseCase)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var output = await getUserUseCase.ExecuteAsync(new GetUserInputDto { Id = userId!, RequesterId = userId! });
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
    public async Task<ActionResult<CreateUserOutputDto>> CreateUser(
        [FromBody] CreateUserInputDto input,
        [FromServices] ICreateUserUseCase createUserUseCase
    )
    {
        try
        {
            var output = await createUserUseCase.ExecuteAsync(input);
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
    public async Task<ActionResult<ListUsersOutputDto>> ListUsers(
        [FromQuery] ListUsersInputDto input,
        [FromServices] IListUsersUseCase listUsersUseCase
    )
    {
        try
        {
            var output = await listUsersUseCase.ExecuteAsync(input);
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
    public async Task<ActionResult<GetUserOutputDto>> GetUserById(
        [FromRoute] string id,
        [FromServices] IGetUserUseCase getUserUseCase
    )
    {
        string? requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var output = await getUserUseCase.ExecuteAsync(new GetUserInputDto { Id = id, RequesterId = requesterId! });
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
    public async Task<ActionResult> RemoveUser(
        [FromRoute] string id,
        [FromServices] IRemoveUserUseCase removeUserUseCase
    )
    {
        string? requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            await removeUserUseCase.ExecuteAsync(new RemoveUserInputDto { Id = id, RequesterId = requesterId! });
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
        [FromBody] UpdateUserQueryDto input,
        [FromServices] IUpdateUserUseCase updateUserUseCase
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
            var output = await updateUserUseCase.ExecuteAsync(inputDto);
            return Ok(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user.");
            return BadRequest(new { message = ex.Message });
        }
    }
}
