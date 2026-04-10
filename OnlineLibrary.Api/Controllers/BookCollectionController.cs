using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.Api.Dtos.BookCollection;
using OnlineLibrary.Application.Abstractions.Services;

namespace OnlineLibrary.Api.Controllers;

[Route("api/collections")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class BookCollectionController(IBookCollectionService bookCollectionService) : ControllerBase
{
    private int CallerId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Creates a new book collection for the current user.</summary>
    /// <param name="input">The collection details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the created collection.</response>
    /// <response code="400">If the name is taken or the active collection limit is reached.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpPost]
    [ProducesResponseType(typeof(BookCollectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] BookCollectionRequest input, CancellationToken cancellationToken)
    {
        var model = input.ToModel(CallerId);
        model = await bookCollectionService.CreateAsync(model, cancellationToken);

        return Ok(model.ToDto());
    }

    /// <summary>Updates an existing collection.</summary>
    /// <param name="id">The id of the collection.</param>
    /// <param name="input">The updated collection details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the updated collection.</response>
    /// <response code="400">If validation fails.</response>
    /// <response code="403">If the collection does not belong to the current user.</response>
    /// <response code="404">If the collection was not found.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpPut("{id:int:min(1)}")]
    [ProducesResponseType(typeof(BookCollectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] BookCollectionRequest input, CancellationToken cancellationToken)
    {
        var model = input.ToModel(CallerId, id);
        model = await bookCollectionService.UpdateAsync(model, CallerId, cancellationToken);

        return Ok(model.ToDto());
    }

    /// <summary>Gets a collection by id.</summary>
    /// <param name="id">The id of the collection.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the collection with its items.</response>
    /// <response code="403">If the collection does not belong to the current user.</response>
    /// <response code="404">If the collection was not found.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpGet("{id:int:min(1)}")]
    [ProducesResponseType(typeof(BookCollectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var model = await bookCollectionService.GetByIdAsync(id, CallerId, cancellationToken);

        return Ok(model.ToDto());
    }

    /// <summary>Gets all collections of the current user.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the list of collections.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<BookCollectionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var models = await bookCollectionService.GetByUserIdAsync(CallerId, cancellationToken);

        return Ok(models.Select(m => m.ToDto()));
    }

    /// <summary>Adds a book to a collection.</summary>
    /// <param name="id">The id of the collection.</param>
    /// <param name="input">The book details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the added item.</response>
    /// <response code="400">If the book is already in the collection or the active book limit is reached.</response>
    /// <response code="403">If the collection does not belong to the current user.</response>
    /// <response code="404">If the collection or book was not found.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpPost("{id:int:min(1)}/books")]
    [ProducesResponseType(typeof(BookCollectionItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddBook(int id, [FromBody] BookCollectionItemRequest input, CancellationToken cancellationToken)
    {
        var model = input.ToModel();
        model = await bookCollectionService.AddBookAsync(id, model, CallerId, cancellationToken);

        return Ok(model.ToDto());
    }

    /// <summary>Updates the status or order of a book in a collection.</summary>
    /// <param name="id">The id of the collection.</param>
    /// <param name="bookId">The id of the book.</param>
    /// <param name="input">The updated item details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the updated item.</response>
    /// <response code="400">If the active book limit is reached.</response>
    /// <response code="403">If the collection does not belong to the current user.</response>
    /// <response code="404">If the collection or book was not found.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpPut("{id:int:min(1)}/books/{bookId:int:min(1)}")]
    [ProducesResponseType(typeof(BookCollectionItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateBook(int id, int bookId, [FromBody] BookCollectionItemRequest input, CancellationToken cancellationToken)
    {
        var model = input.ToModel();
        model.BookId = bookId;
        model = await bookCollectionService.UpdateBookAsync(id, model, CallerId, cancellationToken);

        return Ok(model.ToDto());
    }

    /// <summary>Removes a book from a collection.</summary>
    /// <param name="id">The id of the collection.</param>
    /// <param name="bookId">The id of the book to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Book removed successfully.</response>
    /// <response code="403">If the collection does not belong to the current user.</response>
    /// <response code="404">If the collection or book was not found.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpDelete("{id:int:min(1)}/books/{bookId:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveBook(int id, int bookId, CancellationToken cancellationToken)
    {
        await bookCollectionService.RemoveBookAsync(id, bookId, CallerId, cancellationToken);

        return NoContent();
    }
}
