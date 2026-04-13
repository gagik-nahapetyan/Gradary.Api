using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.Api.Dtos;
using OnlineLibrary.Api.Dtos.Review;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Api.Controllers;

[Route("api/reviews")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class ReviewController(IReviewService reviewService) : ControllerBase
{
    /// <summary>Creates a new review for a book.</summary>
    /// <param name="input">The review details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the created review.</response>
    /// <response code="400">If the request body is invalid.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] ReviewRequest input, CancellationToken cancellationToken)
    {
        var model = input.ToModel();
        model = await reviewService.CreateAsync(model, cancellationToken);

        return Ok(model.ToDto());
    }

    /// <summary>Updates an existing review.</summary>
    /// <param name="id">The id of the review to update.</param>
    /// <param name="input">The updated review details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the updated review.</response>
    /// <response code="400">If the request body is invalid.</response>
    /// <response code="404">If the review was not found.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpPut("{id:int:min(1)}")]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(
        int id, 
        [FromBody] ReviewRequest input, 
        CancellationToken cancellationToken)
    {
        var model = input.ToModel(id);
        await reviewService.UpdateAsync(model, cancellationToken);

        return Ok(model.ToDto());
    }

    /// <summary>Gets a review by id.</summary>
    /// <param name="id">The id of the review.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the review.</response>
    /// <response code="404">If the review was not found.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpGet("{id:int:min(1)}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var model = await reviewService.GetByIdAsync(id, cancellationToken);
        var dto = model.ToDto();

        return Ok(dto);
    }

    /// <summary>Gets a paginated list of reviews for a book.</summary>
    /// <param name="bookId">The id of the book.</param>
    /// <param name="pagination">The pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the paginated list of reviews for the book.</response>
    /// <response code="400">If the pagination parameters are invalid.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpGet("book/{bookId:int:min(1)}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedList<ReviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByBookId(
        int bookId,
        [FromQuery] PagedRequest pagination,
        CancellationToken cancellationToken = default)
    {
        var paged = await reviewService.GetByBookIdAsync(bookId, pagination.Page, pagination.PageSize, pagination.OrderBy, pagination.OrderType, cancellationToken);

        return Ok(new PagedList<ReviewDto>
        {
            Items = [.. paged.Items.Select(m => m.ToDto())],
            TotalCount = paged.TotalCount,
            CurrentPage = paged.CurrentPage,
            PageSize = paged.PageSize
        });
    }

    /// <summary>Soft-deletes a review by id. Only the owner may delete their review.</summary>
    /// <param name="id">The id of the review to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Review deleted successfully.</response>
    /// <response code="403">If the review does not belong to the current user.</response>
    /// <response code="404">If the review was not found.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpDelete("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var callerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await reviewService.DeleteAsync(id, callerId, cancellationToken);

        return NoContent();
    }
}
