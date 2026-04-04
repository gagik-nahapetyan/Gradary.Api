using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLibrary;
using OnlineLibrary.Api.Dtos.User;
using OnlineLibrary.Application.Abstractions.Services;

namespace OnlineLibrary.Api.Controllers;

[Route("api/users")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class UserController(IUserService userService) : ControllerBase
{
    /// <summary>Creates a new user.</summary>
    /// <param name="input">The user details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the created user.</response>
    /// <response code="400">If the request body is invalid or the password is missing.</response>
    /// <response code="401">If the request is not authenticated.</response>
    /// <response code="403">If the user does not have the required role.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] UserCreateRequest input, CancellationToken cancellationToken)
    {
        var model = input.ToModel();
        model = await userService.CreateAsync(model, input.Password, cancellationToken);

        return Ok(model.ToDto());
    }

    /// <summary>Updates an existing user.</summary>
    /// <param name="id">The id of the user to update.</param>
    /// <param name="input">The updated user details. Omit password to keep the existing one.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the updated user.</response>
    /// <response code="400">If the request body is invalid.</response>
    /// <response code="404">If the user was not found.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpPut("{id:int:min(1)}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] UserUpdateRequest input, CancellationToken cancellationToken)
    {
        var model = input.ToModel(id);
        await userService.UpdateAsync(model, cancellationToken);

        return Ok(model.ToDto());
    }

    /// <summary>Updates the password of an existing user.</summary>
    /// <param name="id">The id of the user.</param>
    /// <param name="input">The new password details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Password updated successfully.</response>
    /// <response code="400">If the request body is invalid or the password is empty.</response>
    /// <response code="404">If the user was not found.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpPut("{id:int:min(1)}/password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdatePassword(int id, [FromBody] UpdatePasswordRequest input, CancellationToken cancellationToken)
    {
        await userService.UpdatePasswordAsync(id, input.NewPassword, cancellationToken);

        return NoContent();
    }

    /// <summary>Gets a user by id.</summary>
    /// <param name="id">The id of the user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the user.</response>
    /// <response code="404">If the user was not found.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpGet("{id:int:min(1)}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var model = await userService.GetByIdAsync(id, cancellationToken);
        var dto = model.ToDto();

        return Ok(dto);
    }

    /// <summary>Gets all users.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the list of users.</response>
    /// <response code="401">If the request is not authenticated.</response>
    /// <response code="403">If the user does not have the required role.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Librarian")]
    [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var users = await userService.GetAsync(cancellationToken);
        var dtos = users.Select(u => u.ToDto()).ToList();

        return Ok(dtos);
    }
}
