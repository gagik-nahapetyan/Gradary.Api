using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.Api.Dtos.Author;
using OnlineLibrary.Application.Abstractions.Services;

namespace OnlineLibrary.Api.Controllers;

[Route("api/authors")]
[ApiController]
[Produces("application/json")]
public class AuthorController(IAuthorService authorService) : ControllerBase
{
    /// <summary>Creates a new author.</summary>
    /// <param name="input">The author details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the created author.</response>
    /// <response code="400">If the request body is invalid.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpPost]
    [ProducesResponseType(typeof(AuthorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] AuthorRequest input, CancellationToken cancellationToken)
    {
        var model = input.ToModel();
        model = await authorService.CreateAsync(model, cancellationToken);

        return Ok(model.ToDto());
    }

    /// <summary>Updates an existing author.</summary>
    /// <param name="id">The id of the author to update.</param>
    /// <param name="input">The updated author details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the updated author.</response>
    /// <response code="400">If the request body is invalid.</response>
    /// <response code="404">If the author was not found.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpPut("{id:int:min(1)}")]
    [ProducesResponseType(typeof(AuthorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] AuthorRequest input, CancellationToken cancellationToken)
    {
        var model = input.ToModel(id);
        await authorService.UpdateAsync(model, cancellationToken);

        return Ok(model.ToDto());
    }

    /// <summary>Gets an author by id.</summary>
    /// <param name="id">The id of the author.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the author.</response>
    /// <response code="404">If the author was not found.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpGet("{id:int:min(1)}")]
    [ProducesResponseType(typeof(AuthorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var model = await authorService.GetByIdAsync(id, cancellationToken);
        var dto = model.ToDto();

        return Ok(dto);
    }

    /// <summary>Gets all authors.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the list of authors.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<AuthorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var authors = await authorService.GetAsync(cancellationToken);
        var dtos = authors.Select(a => a.ToDto()).ToList();

        return Ok(dtos);
    }
}
