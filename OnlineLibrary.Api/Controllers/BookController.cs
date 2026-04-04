using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OnlineLibrary.Api.Dtos.Book;
using OnlineLibrary.Domain.Settings;
using OnlineLibrary.Application.Abstractions.Services;

namespace OnlineLibrary.Api.Controllers;

[Route("api/books")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class BookController(IBookService bookService, IOptions<FileUploadSettings> fileUploadSettingsOptions) : ControllerBase
{
    private readonly FileUploadSettings fileUploadSettings = fileUploadSettingsOptions.Value;

    /// <summary>Creates a new book.</summary>
    /// <param name="input">The book details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the created book.</response>
    /// <response code="400">If the request body is invalid or the title already exists.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpPost]
    [Authorize(Roles = "Admin,Librarian")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] BookRequest input, CancellationToken cancellationToken)
    {
        var model = input.ToModel();
        model = await bookService.CreateAsync(model, cancellationToken);

        return Ok(model.ToDto());
    }

    /// <summary>Updates an existing book.</summary>
    /// <param name="id">The id of the book to update.</param>
    /// <param name="input">The updated book details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the updated book.</response>
    /// <response code="400">If the request body is invalid or the title already exists.</response>
    /// <response code="404">If the book was not found.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpPut("{id:int:min(1)}")]
    [Authorize(Roles = "Admin,Librarian")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] BookRequest input, CancellationToken cancellationToken)
    {
        var model = input.ToModel(id);
        await bookService.UpdateAsync(model, cancellationToken);

        return Ok(model.ToDto());
    }

    /// <summary>Uploads or replaces the file for a book.</summary>
    /// <param name="id">The id of the book.</param>
    /// <param name="file">The file to upload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">File uploaded successfully.</response>
    /// <response code="400">If no file was provided.</response>
    /// <response code="404">If the book was not found.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpPost("{id:int:min(1)}/file")]
    [Authorize(Roles = "Admin,Librarian")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadFile(int id, [Required] IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            return BadRequest("File must not be empty.");

        if (file.Length > fileUploadSettings.MaxFileSizeBytes)
            return BadRequest($"File size must not exceed {fileUploadSettings.MaxFileSizeBytes / (1024 * 1024)} MB.");

        await bookService.UploadFileAsync(id, file.OpenReadStream, cancellationToken);

        return NoContent();
    }

    /// <summary>Gets a book by id.</summary>
    /// <param name="id">The id of the book.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the book.</response>
    /// <response code="404">If the book was not found.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpGet("{id:int:min(1)}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var model = await bookService.GetByIdAsync(id, cancellationToken);
        var dto = model.ToDto();

        return Ok(dto);
    }

    /// <summary>Gets all books.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the list of books.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<BookDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var books = await bookService.GetAsync(cancellationToken);

        return Ok(books.Select(b => b.ToDto()));
    }
}
