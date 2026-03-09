using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.Api.Dtos;
using OnlineLibrary.Application.Abstractions.Services;

namespace OnlineLibrary.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController(IReviewService reviewService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create(ReviewRequest input)
    {
        var model = input.ToModel();
        model = await reviewService.CreateAsync(model);

        return Ok(model.ToDto());
    }

    [HttpPost("update/{id:int:min(1)}")]
    public async Task<IActionResult> Update(int id, ReviewRequest input)
    {
        var model = input.ToModel(id);
        await reviewService.UpdateAsync(model);

        return Ok(model.ToDto());
    }
    
    [HttpGet("{id:int:min(1)}")]
    public async Task<IActionResult> Get(int id)
    {
        var model = await reviewService.GetByIdAsync(id);
        var dto = model.ToDto();

        return Ok(dto);
    }
    
    [HttpGet("book/{bookId:int:min(1)}")]
    public async Task<IActionResult> GetByBookId(int bookId)
    {
        var models = await reviewService.GetByBookIdAsync(bookId);
        var dtos = models.Select(m => m.ToDto()).ToList();

        return Ok(dtos);
    }
}
