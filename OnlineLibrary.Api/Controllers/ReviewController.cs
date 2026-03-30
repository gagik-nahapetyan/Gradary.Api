using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.Api.Dtos.Review;
using OnlineLibrary.Application.Abstractions.Services;

namespace OnlineLibrary.Api.Controllers;

[Route("api/reviews")]
[ApiController]
public class ReviewController(IReviewService reviewService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(ReviewRequest input, CancellationToken cancellationToken)
    {
        var model = input.ToModel();
        model = await reviewService.CreateAsync(model, cancellationToken);

        return Ok(model.ToDto());
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<IActionResult> Update(int id, ReviewRequest input, CancellationToken cancellationToken)
    {
        var model = input.ToModel(id);
        await reviewService.UpdateAsync(model, cancellationToken);

        return Ok(model.ToDto());
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var model = await reviewService.GetByIdAsync(id, cancellationToken);
        var dto = model.ToDto();

        return Ok(dto);
    }

    [HttpGet("book/{bookId:int:min(1)}")]
    public async Task<IActionResult> GetByBookId(int bookId, CancellationToken cancellationToken)
    {
        var models = await reviewService.GetByBookIdAsync(bookId, cancellationToken);
        var dtos = models.Select(m => m.ToDto()).ToList();

        return Ok(dtos);
    }
}