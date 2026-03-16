using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.Api.Dtos.Category;
using OnlineLibrary.Application.Abstractions.Services;

namespace OnlineLibrary.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create(CategoryRequest input)
    {
        var model = input.ToModel();
        model = await categoryService.CreateAsync(model);

        return Ok(model.ToDto());
    }

    [HttpPut("update/{id:int:min(1)}")]
    public async Task<IActionResult> Update(int id, CategoryRequest input)
    {
        var model = input.ToModel(id);
        await categoryService.UpdateAsync(model);

        return Ok(model.ToDto());
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<IActionResult> Get(int id)
    {
        var model = await categoryService.GetByIdAsync(id);
        var dto = model.ToDto();

        return Ok(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var categories = await categoryService.GetAsync();
        var dtos = categories.Select(c => c.ToDto()).ToList();

        return Ok(dtos);
    }
}

