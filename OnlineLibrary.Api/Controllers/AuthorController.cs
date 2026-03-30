using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.Api.Dtos.Author;
using OnlineLibrary.Application.Abstractions.Services;

namespace OnlineLibrary.Api.Controllers;

[Route("api/authors")]
[ApiController]
public class AuthorController(IAuthorService authorService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(AuthorRequest input, CancellationToken cancellationToken)
    {
        var model = input.ToModel();
        model = await authorService.CreateAsync(model, cancellationToken);

        return Ok(model.ToDto());
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<IActionResult> Update(int id, AuthorRequest input, CancellationToken cancellationToken)
    {
        var model = input.ToModel(id);
        await authorService.UpdateAsync(model, cancellationToken);

        return Ok(model.ToDto());
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var model = await authorService.GetByIdAsync(id, cancellationToken);
        var dto = model.ToDto();

        return Ok(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var authors = await authorService.GetAsync(cancellationToken);
        var dtos = authors.Select(a => a.ToDto()).ToList();

        return Ok(dtos);
    }
}
