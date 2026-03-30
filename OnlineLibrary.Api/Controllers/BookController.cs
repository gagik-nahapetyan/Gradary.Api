using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.Api.Dtos.Book;
using OnlineLibrary.Application.Abstractions.Services;

namespace OnlineLibrary.Api.Controllers;

[Route("api/books")]
[ApiController]
public class BookController(IBookService bookService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(BookRequest input, CancellationToken cancellationToken)
    {
        using var stream = input.File?.OpenReadStream();
        var model = input.ToModel(stream: stream);

        model = await bookService.CreateAsync(model, cancellationToken);

        return Ok(model.ToDto());
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<IActionResult> Update(int id, BookRequest input, CancellationToken cancellationToken)
    {
        using var stream = input.File?.OpenReadStream();
        var model = input.ToModel(id, stream);

        await bookService.UpdateAsync(model, cancellationToken);

        return Ok(model.ToDto());
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var model = await bookService.GetByIdAsync(id, cancellationToken);
        var dto = model.ToDto();

        return Ok(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var books = await bookService.GetAsync(cancellationToken);
        
        return Ok(books.Select(b => b.ToDto()));
    }
}
