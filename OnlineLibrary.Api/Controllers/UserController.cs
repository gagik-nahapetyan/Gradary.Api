using Microsoft.AspNetCore.Mvc;
using OnlineLibrary;
using OnlineLibrary.Api.Dtos.User;
using OnlineLibrary.Application.Abstractions.Services;

namespace OnlineLibrary.Api.Controllers;

[Route("api/users")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(UserRequest input, CancellationToken cancellationToken)
    {
        var model = input.ToModel();
        model = await userService.CreateAsync(model, cancellationToken);

        return Ok(model.ToDto());
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<IActionResult> Update(int id, UserRequest input, CancellationToken cancellationToken)
    {
        var model = input.ToModel(id);
        await userService.UpdateAsync(model, cancellationToken);

        return Ok(model.ToDto());
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var model = await userService.GetByIdAsync(id, cancellationToken);
        var dto = model.ToDto();

        return Ok(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var users = await userService.GetAsync(cancellationToken);
        var dtos = users.Select(u => u.ToDto()).ToList();

        return Ok(dtos);
    }
}