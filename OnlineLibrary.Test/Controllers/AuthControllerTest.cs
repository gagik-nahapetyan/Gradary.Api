using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using OnlineLibrary.Api.Controllers;
using OnlineLibrary.Api.Dtos.Auth;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Models;
using OnlineLibrary.Domain.Enums;
using OnlineLibrary.Domain.Settings;

namespace OnlineLibrary.Test.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();

        var jwtOptions = Options.Create(new JwtSettings
        {
            Issuer = "test",
            Audience = "test",
            SecretKey = "test-secret-key-min-32-characters!!",
            AccessTokenExpiryMinutes = 60
        });

        _controller = new AuthController(_mockAuthService.Object, jwtOptions);
    }

    [Fact]
    public async Task Register_ShouldReturnLoginResponse_WhenInputIsValid()
    {
        // arrange
        var input = new RegisterRequest
        {
            FullName = "David Goggins",
            Email = "david@goggins.com",
            Password = "StrongPass123!"
        };

        var user = new UserModel { Id = 1, FullName = input.FullName, Email = input.Email, Role = UserRole.Member };
        var authResult = new AuthResult("access-token", user);

        _mockAuthService
            .Setup(s => s.RegisterAsync(input.FullName, input.Email, input.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(authResult);

        // act
        var result = await _controller.Register(input, CancellationToken.None);

        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(okResponse.Value);

        Assert.Equal(authResult.AccessToken, response.AccessToken);
        Assert.Equal(user.Id, response.User.Id);
        Assert.Equal(user.Email, response.User.Email);

        _mockAuthService.Verify(s => s.RegisterAsync(input.FullName, input.Email, input.Password, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Register_ShouldThrowArgumentException_WhenEmailIsAlreadyTaken()
    {
        // arrange
        var input = new RegisterRequest
        {
            FullName = "David Goggins",
            Email = "david@goggins.com",
            Password = "StrongPass123!"
        };

        var expectedMessage = $"Email '{input.Email}' is already registered.";
        _mockAuthService
            .Setup(s => s.RegisterAsync(input.FullName, input.Email, input.Password, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException(expectedMessage));

        // act & assert — global exception middleware maps this to 400 ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _controller.Register(input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);
    }

    [Fact]
    public async Task Login_ShouldReturnLoginResponse_WhenCredentialsAreValid()
    {
        // arrange
        var input = new LoginRequest { Email = "david@goggins.com", Password = "StrongPass123!" };

        var user = new UserModel { Id = 1, FullName = "David Goggins", Email = input.Email, Role = UserRole.Member };
        var authResult = new AuthResult("access-token", user);

        _mockAuthService
            .Setup(s => s.LoginAsync(input.Email, input.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(authResult);

        // act
        var result = await _controller.Login(input, CancellationToken.None);

        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(okResponse.Value);

        Assert.Equal(authResult.AccessToken, response.AccessToken);
        Assert.Equal(user.Id, response.User.Id);

        _mockAuthService.Verify(s => s.LoginAsync(input.Email, input.Password, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Login_ShouldThrowUnauthorizedAccessException_WhenCredentialsAreInvalid()
    {
        // arrange
        var input = new LoginRequest { Email = "david@goggins.com", Password = "WrongPassword!" };

        _mockAuthService
            .Setup(s => s.LoginAsync(input.Email, input.Password, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid email or password."));

        // act & assert — global exception middleware maps this to 401 ProblemDetails when the API runs end-to-end
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _controller.Login(input, CancellationToken.None));

        _mockAuthService.Verify(s => s.LoginAsync(input.Email, input.Password, It.IsAny<CancellationToken>()), Times.Once);
    }
}
