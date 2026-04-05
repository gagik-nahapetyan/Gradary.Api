using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.Api.Dtos.Category;
using OnlineLibrary.Application.Abstractions.Services;

namespace OnlineLibrary.Api.Controllers;

[Route("api/categories")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    /// <summary>Creates a new category.</summary>
    /// <param name="input">The category details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the created category.</response>
    /// <response code="400">If the request body is invalid, the parent does not exist, or a hierarchy cycle would be introduced.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpPost]
    [Authorize(Roles = "Admin,Librarian")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CategoryRequest input, CancellationToken cancellationToken)
    {
        var model = input.ToModel();
        model = await categoryService.CreateAsync(model, cancellationToken);

        return Ok(model.ToDto());
    }

    /// <summary>Updates an existing category.</summary>
    /// <param name="id">The id of the category to update.</param>
    /// <param name="input">The updated category details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the updated category.</response>
    /// <response code="400">If the request body is invalid, the parent does not exist, or a hierarchy cycle would be introduced.</response>
    /// <response code="404">If the category was not found.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpPut("{id:int:min(1)}")]
    [Authorize(Roles = "Admin,Librarian")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryRequest input, CancellationToken cancellationToken)
    {
        var model = input.ToModel(id);
        await categoryService.UpdateAsync(model, cancellationToken);

        return Ok(model.ToDto());
    }

    /// <summary>Gets a category by id.</summary>
    /// <param name="id">The id of the category.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the category.</response>
    /// <response code="404">If the category was not found.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpGet("{id:int:min(1)}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var model = await categoryService.GetByIdAsync(id, cancellationToken);
        var dto = model.ToDto();

        return Ok(dto);
    }

    /// <summary>Gets all categories.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the list of categories.</response>
    /// <response code="500">If an unexpected error occurred.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var categories = await categoryService.GetAsync(cancellationToken);
        var dtos = categories.Select(c => c.ToDto()).ToList();

        return Ok(dtos);
    }
}
