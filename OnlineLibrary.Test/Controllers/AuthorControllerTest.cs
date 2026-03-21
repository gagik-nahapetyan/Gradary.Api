using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineLibrary.Api.Controllers;
using OnlineLibrary.Api.Dtos.Author;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Test.Controllers;

public class AuthorControllerTests
{
    private readonly AuthorController _controller;
    private readonly Mock<IAuthorService> _mockAuthorService;

    public AuthorControllerTests()
    {
        _mockAuthorService = new Mock<IAuthorService>();
        _controller = new AuthorController(_mockAuthorService.Object);
    }

    [Fact]
    public async Task CreateAuthor_ShouldCreateAuthor_WhenInputIsValid()
    {
        // arrange
        var input = new AuthorRequest
        {
            FullName = "David Goggins",
            Biography = "Can't Heart Me"
        };

        var createdModel = new AuthorModel
        {
            Id = 123,
            FullName = input.FullName,
            Biography = input.Biography
        };

        _mockAuthorService
            .Setup(s => s.CreateAsync(It.Is<AuthorModel>(m => 
                m.FullName == input.FullName && 
                m.Biography == input.Biography)))
            .ReturnsAsync(createdModel);


        // act
        var result = await _controller.Create(input);


        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<AuthorDto>(okResponse.Value);
        
        Assert.Equal(createdModel.Id, dto.Id);
        Assert.Equal(input.FullName, dto.FullName);
        Assert.Equal(input.Biography, dto.Biography);

        _mockAuthorService.Verify(s => s.CreateAsync(It.IsAny<AuthorModel>()), Times.Once);
    }

    [Theory]
    [InlineData(123)]
    public async Task UpdateAuthor_ShouldUpdateAuthor_WhenInputIsValid(int id)
    {
        // arrange
        var input = new AuthorRequest
        {
            FullName = "David Goggins",
            Biography = "Can't Heart Me"
        };

        _mockAuthorService
            .Setup(s => s.UpdateAsync(It.Is<AuthorModel>(m => 
                m.Id == id &&
                m.FullName == input.FullName &&
                m.Biography == input.Biography)))
            .Returns(Task.CompletedTask);

        // act
        var result = await _controller.Update(id, input);

        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<AuthorDto>(okResponse.Value);
        
        Assert.Equal(id, dto.Id);
        Assert.Equal(input.FullName, dto.FullName);
        Assert.Equal(input.Biography, dto.Biography);

        _mockAuthorService.Verify(s => s.UpdateAsync(It.IsAny<AuthorModel>()), Times.Once);
    }

    [Theory]
    [InlineData(123)]
    public async Task UpdateAuthor_ShouldUpdateAuthor_WhenIdIsNotValid(int id)
    {
        // arrange
        var input = new AuthorRequest
        {
            FullName = "David Goggins",
            Biography = "Can't Heart Me"
        };

        var expectedMessage = $"Author with id {id} not found";
        _mockAuthorService
            .Setup(s => s.UpdateAsync(It.Is<AuthorModel>(m => m.Id == id)))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));

        // act & assert — global exception middleware maps this to 404 ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Update(id, input));
        Assert.Equal(expectedMessage, ex.Message);

        _mockAuthorService.Verify(s => s.UpdateAsync(It.IsAny<AuthorModel>()), Times.Once);
    }

    [Theory]
    [InlineData(123)]
    public async Task GetAuthorById_ShouldReturnAuthor_WhenAuthorExists(int id)
    {
        // arrange
        var author = new AuthorModel
        {
            Id = id,
            FullName = "David Goggins",
            Biography = "Can't Heart Me"
        };

        _mockAuthorService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(author);

        // act
        var result = await _controller.Get(id);

        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<AuthorDto>(okResponse.Value);

        Assert.Equal(id, dto.Id);

        _mockAuthorService.Verify(s => s.GetByIdAsync(id), Times.Once);
    }

    [Theory]
    [InlineData(123)]
    public async Task GetAuthorById_ShouldThrowKeyNotFound_WhenAuthorDoesNotExist(int id)
    {
        // arrange
        var expectedMessage = $"Author with id {id} not found";
        _mockAuthorService
            .Setup(s => s.GetByIdAsync(id))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));

        // act & assert — global exception middleware maps this to 404 ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Get(id));
        Assert.Equal(expectedMessage, ex.Message);

        _mockAuthorService.Verify(s => s.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetAuthors_ShouldReturnAuthors_WhenAuthorsExist()
    {
        // arrange
        var authors = new List<AuthorModel>
        {
            new()
            {
                Id = 1,
                FullName = "David Goggins",
                Biography = "Can't Heart Me"
            },
            new()
            {
                Id = 2,
                FullName = "Robert Greene",
                Biography = "The Laws of Human Nature"
            }
        };

        _mockAuthorService.Setup(s => s.GetAsync()).ReturnsAsync(authors);

        // act
        var result = await _controller.Get();

        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dtos = Assert.IsType<List<AuthorDto>>(okResponse.Value);

        Assert.Equal(2, dtos.Count);

        _mockAuthorService.Verify(s => s.GetAsync(), Times.Once);
    }
}
