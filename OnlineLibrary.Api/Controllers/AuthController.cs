using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OnlineLibrary.Api.Dtos.Auth;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Settings;

namespace OnlineLibrary.Api.Controllers;

[Route("api/auth")]
[ApiController]
[AllowAnonymous]
[Produces("application/json")]
public class AuthController(IAuthService authService, IOptions<JwtSettings> jwtOptions) : ControllerBase
{
    /// <summary>Registers a new user and returns an access token.</summary>
    /// <response code="200">Returns the access token and user info.</response>
    /// <response code="400">If the email is already taken or input is invalid.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest input, CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(input.FullName, input.Email, input.Password, cancellationToken);
        return Ok(result.ToResponse(ExpiresAt));
    }

    /// <summary>Authenticates a user and returns an access token.</summary>
    /// <response code="200">Returns the access token and user info.</response>
    /// <response code="401">If credentials are invalid.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest input, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(input.Email, input.Password, cancellationToken);
        return Ok(result.ToResponse(ExpiresAt));
    }

    private DateTime ExpiresAt => DateTime.UtcNow.AddMinutes(jwtOptions.Value.AccessTokenExpiryMinutes);
}
