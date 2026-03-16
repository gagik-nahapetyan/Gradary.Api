using Microsoft.AspNetCore.Mvc;
using OnlineLibrary;
using OnlineLibrary.Api.Dtos.User;
using OnlineLibrary.Application.Abstractions.Services;

namespace OnlineLibrary.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create(UserRequest input)
    {
        var model = input.ToModel();
        model = await userService.CreateAsync(model);

        return Ok(model.ToDto());
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<IActionResult> Update(int id, UserRequest input)
    {
        var model = input.ToModel(id);
        await userService.UpdateAsync(model);

        return Ok(model.ToDto());
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<IActionResult> Get(int id)
    {
        var model = await userService.GetByIdAsync(id);
        var dto = model.ToDto();

        return Ok(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var users = await userService.GetAsync();
        var dtos = users.Select(u => u.ToDto()).ToList();

        return Ok(dtos);
    }
}
