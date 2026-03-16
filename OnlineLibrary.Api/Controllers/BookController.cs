using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.Api.Dtos.Book;
using OnlineLibrary.Application.Abstractions.Services;

namespace OnlineLibrary.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookController(IBookService bookService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Upload(BookRequest input)
    {
        var model = input.ToModel();
        model = await bookService.CreateAsync(model);

        return Ok(model.ToDto());
    }

    [HttpPut("update/{id:int:min(1)}")]
    public async Task<IActionResult> Update(int id, BookRequest input)
    {
        var model = input.ToModel(id);
        await bookService.UpdateAsync(model);

        return Ok(model.ToDto());
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<IActionResult> Get(int id)
    {
        var model = await bookService.GetByIdAsync(id);
        var dto = model.ToDto();

        return Ok(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var books = await bookService.GetAsync();
        
        return Ok(books);
    }
}
