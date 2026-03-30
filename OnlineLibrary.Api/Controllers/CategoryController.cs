using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.Api.Dtos.Category;
using OnlineLibrary.Application.Abstractions.Services;

namespace OnlineLibrary.Api.Controllers;

[Route("api/categories")]
[ApiController]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CategoryRequest input, CancellationToken cancellationToken)
    {
        var model = input.ToModel();
        model = await categoryService.CreateAsync(model, cancellationToken);

        return Ok(model.ToDto());
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<IActionResult> Update(int id, CategoryRequest input, CancellationToken cancellationToken)
    {
        var model = input.ToModel(id);
        await categoryService.UpdateAsync(model, cancellationToken);

        return Ok(model.ToDto());
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var model = await categoryService.GetByIdAsync(id, cancellationToken);
        var dto = model.ToDto();

        return Ok(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var categories = await categoryService.GetAsync(cancellationToken);
        var dtos = categories.Select(c => c.ToDto()).ToList();

        return Ok(dtos);
    }
}