using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.Api.Dtos.Author;
using OnlineLibrary.Application.Abstractions.Services;

namespace OnlineLibrary.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthorController(IAuthorService authorService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create(AuthorRequest input)
    {
        var model = input.ToModel();
        model = await authorService.CreateAsync(model);

        return Ok(model.ToDto());
    }

    [HttpPut("update/{id:int:min(1)}")]
    public async Task<IActionResult> Update(int id, AuthorRequest input)
    {
        var model = input.ToModel(id);
        await authorService.UpdateAsync(model);

        return Ok(model.ToDto());
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<IActionResult> Get(int id)
    {
        var model = await authorService.GetByIdAsync(id);
        var dto = model.ToDto();

        return Ok(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var authors = await authorService.GetAsync();
        var dtos = authors.Select(a => a.ToDto()).ToList();

        return Ok(dtos);
    }
}

