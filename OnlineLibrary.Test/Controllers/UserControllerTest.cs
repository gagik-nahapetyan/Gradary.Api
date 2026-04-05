using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineLibrary.Api.Controllers;
using OnlineLibrary.Api.Dtos.User;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Enums;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Test.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _controller = new UserController(_mockUserService.Object);
    }

    private static ClaimsPrincipal CreateUser(int id, string role = "Member")
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(ClaimTypes.Role, role)
        };
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
    }

    private void SetCaller(int id, string role = "Member")
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateUser(id, role) }
        };
    }

    [Fact]
    public async Task CreateUser_ShouldCreateUser_WhenInputIsValid()
    {
        // arrange
        var input = new UserCreateRequest
        {
            FullName = "David Goggins",
            Email = "david@goggins.com",
            Password = "StrongPass123!",
            Role = UserRole.Member
        };

        var createdModel = new UserModel
        {
            Id = 1,
            FullName = input.FullName,
            Email = input.Email,
            PasswordHash = "hashed",
            Role = input.Role
        };

        _mockUserService
            .Setup(s => s.CreateAsync(
                It.Is<UserModel>(m =>
                    m.FullName == input.FullName &&
                    m.Email == input.Email &&
                    m.Role == input.Role),
                input.Password,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdModel);

        // act
        var result = await _controller.Create(input, CancellationToken.None);

        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<UserDto>(okResponse.Value);

        Assert.Equal(createdModel.Id, dto.Id);
        Assert.Equal(input.FullName, dto.FullName);
        Assert.Equal(input.Email, dto.Email);
        Assert.Equal(input.Role, dto.Role);

        _mockUserService.Verify(s => s.CreateAsync(It.IsAny<UserModel>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    public async Task UpdateUser_ShouldUpdateUser_WhenCallerIsOwner(int id)
    {
        // arrange
        SetCaller(id);

        var input = new UserUpdateRequest
        {
            FullName = "David Goggins",
            Email = "david@goggins.com",
            Role = UserRole.Member
        };

        _mockUserService
            .Setup(s => s.UpdateAsync(
                It.Is<UserModel>(m =>
                    m.Id == id &&
                    m.FullName == input.FullName &&
                    m.Email == input.Email &&
                    m.Role == input.Role),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // act
        var result = await _controller.Update(id, input, CancellationToken.None);

        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<UserDto>(okResponse.Value);

        Assert.Equal(id, dto.Id);
        Assert.Equal(input.FullName, dto.FullName);
        Assert.Equal(input.Email, dto.Email);
        Assert.Equal(input.Role, dto.Role);

        _mockUserService.Verify(s => s.UpdateAsync(It.IsAny<UserModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_ShouldUpdateUser_WhenCallerIsAdmin()
    {
        // arrange
        const int targetId = 1;
        SetCaller(id: 99, role: "Admin");

        var input = new UserUpdateRequest
        {
            FullName = "David Goggins",
            Email = "david@goggins.com",
            Role = UserRole.Member
        };

        _mockUserService
            .Setup(s => s.UpdateAsync(It.IsAny<UserModel>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // act
        var result = await _controller.Update(targetId, input, CancellationToken.None);

        // assert
        Assert.IsType<OkObjectResult>(result);
        _mockUserService.Verify(s => s.UpdateAsync(It.IsAny<UserModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnForbid_WhenCallerIsNotOwnerAndNotAdmin()
    {
        // arrange
        SetCaller(id: 2, role: "Member");

        var input = new UserUpdateRequest
        {
            FullName = "David Goggins",
            Email = "david@goggins.com",
            Role = UserRole.Member
        };

        // act
        var result = await _controller.Update(1, input, CancellationToken.None);

        // assert
        Assert.IsType<ForbidResult>(result);
        _mockUserService.Verify(s => s.UpdateAsync(It.IsAny<UserModel>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(999)]
    public async Task UpdateUser_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist(int id)
    {
        // arrange
        SetCaller(id);

        var input = new UserUpdateRequest
        {
            FullName = "David Goggins",
            Email = "david@goggins.com",
            Role = UserRole.Member
        };

        var expectedMessage = $"User with id {id} not found";
        _mockUserService
            .Setup(s => s.UpdateAsync(It.Is<UserModel>(m => m.Id == id), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));

        // act & assert — global exception middleware maps this to 404 ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Update(id, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockUserService.Verify(s => s.UpdateAsync(It.IsAny<UserModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    public async Task UpdatePassword_ShouldReturnNoContent_WhenCallerIsOwner(int id)
    {
        // arrange
        SetCaller(id);

        var input = new UpdatePasswordRequest { NewPassword = "NewStrongPass456!" };

        _mockUserService
            .Setup(s => s.UpdatePasswordAsync(id, input.NewPassword, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // act
        var result = await _controller.UpdatePassword(id, input, CancellationToken.None);

        // assert
        Assert.IsType<NoContentResult>(result);

        _mockUserService.Verify(s => s.UpdatePasswordAsync(id, input.NewPassword, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePassword_ShouldReturnForbid_WhenCallerIsNotOwner()
    {
        // arrange
        SetCaller(id: 2);

        var input = new UpdatePasswordRequest { NewPassword = "NewStrongPass456!" };

        // act
        var result = await _controller.UpdatePassword(1, input, CancellationToken.None);

        // assert
        Assert.IsType<ForbidResult>(result);
        _mockUserService.Verify(s => s.UpdatePasswordAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(999)]
    public async Task UpdatePassword_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist(int id)
    {
        // arrange
        SetCaller(id);

        var input = new UpdatePasswordRequest { NewPassword = "NewStrongPass456!" };

        var expectedMessage = $"User with id {id} not found";
        _mockUserService
            .Setup(s => s.UpdatePasswordAsync(id, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));

        // act & assert — global exception middleware maps this to 404 ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.UpdatePassword(id, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockUserService.Verify(s => s.UpdatePasswordAsync(id, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    public async Task GetUserById_ShouldReturnUser_WhenUserExists(int id)
    {
        // arrange
        var user = new UserModel
        {
            Id = id,
            FullName = "David Goggins",
            Email = "david@goggins.com",
            PasswordHash = "hashed",
            Role = UserRole.Member
        };

        _mockUserService
            .Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // act
        var result = await _controller.Get(id, CancellationToken.None);

        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<UserDto>(okResponse.Value);

        Assert.Equal(user.Id, dto.Id);
        Assert.Equal(user.FullName, dto.FullName);
        Assert.Equal(user.Email, dto.Email);
        Assert.Equal(user.Role, dto.Role);

        _mockUserService.Verify(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(999)]
    public async Task GetUserById_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist(int id)
    {
        // arrange
        var expectedMessage = $"User with id {id} not found";
        _mockUserService
            .Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));

        // act & assert — global exception middleware maps this to 404 ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Get(id, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockUserService.Verify(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUsers_ShouldReturnUsers_WhenUsersExist()
    {
        // arrange
        var users = new List<UserModel>
        {
            new() { Id = 1, FullName = "David Goggins", Email = "david@goggins.com", PasswordHash = "hashed", Role = UserRole.Member },
            new() { Id = 2, FullName = "Robert Greene", Email = "robert@greene.com", PasswordHash = "hashed", Role = UserRole.Librarian }
        };

        _mockUserService
            .Setup(s => s.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // act
        var result = await _controller.Get(CancellationToken.None);

        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dtos = Assert.IsType<List<UserDto>>(okResponse.Value);

        Assert.Equal(2, dtos.Count);
        Assert.Equal(users[0].Id, dtos[0].Id);
        Assert.Equal(users[1].Id, dtos[1].Id);

        _mockUserService.Verify(s => s.GetAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUsers_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        // arrange
        _mockUserService
            .Setup(s => s.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // act
        var result = await _controller.Get(CancellationToken.None);

        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dtos = Assert.IsType<List<UserDto>>(okResponse.Value);

        Assert.Empty(dtos);

        _mockUserService.Verify(s => s.GetAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
