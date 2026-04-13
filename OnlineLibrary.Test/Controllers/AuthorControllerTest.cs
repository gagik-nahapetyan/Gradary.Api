using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineLibrary.Api.Controllers;
using OnlineLibrary.Api.Dtos;
using OnlineLibrary.Api.Dtos.Author;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Enums;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Test.Controllers;

public class AuthorControllerTests
{
    private readonly Mock<IAuthorService> _mockAuthorService;
    private readonly AuthorController _controller;

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
            .Setup(s => s.CreateAsync(
                It.Is<AuthorModel>(m =>
                    m.FullName == input.FullName &&
                    m.Biography == input.Biography), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdModel);


        // act
        var result = await _controller.Create(input, CancellationToken.None);


        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<AuthorDto>(okResponse.Value);

        Assert.Equal(createdModel.Id, dto.Id);
        Assert.Equal(input.FullName, dto.FullName);
        Assert.Equal(input.Biography, dto.Biography);

        _mockAuthorService.Verify(s => s.CreateAsync(It.IsAny<AuthorModel>(), It.IsAny<CancellationToken>()), Times.Once);
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
            .Setup(s => s.UpdateAsync(
                It.Is<AuthorModel>(m =>
                    m.Id == id &&
                    m.FullName == input.FullName &&
                    m.Biography == input.Biography), 
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // act
        var result = await _controller.Update(id, input, CancellationToken.None);

        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<AuthorDto>(okResponse.Value);

        Assert.Equal(id, dto.Id);
        Assert.Equal(input.FullName, dto.FullName);
        Assert.Equal(input.Biography, dto.Biography);

        _mockAuthorService.Verify(s => s.UpdateAsync(It.IsAny<AuthorModel>(), It.IsAny<CancellationToken>()), Times.Once);
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
            .Setup(s => s.UpdateAsync(It.Is<AuthorModel>(m => m.Id == id), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));

        // act & assert — global exception middleware maps this to 404 ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Update(id, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockAuthorService.Verify(s => s.UpdateAsync(It.IsAny<AuthorModel>(), It.IsAny<CancellationToken>()), Times.Once);
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

        _mockAuthorService.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(author);

        // act
        var result = await _controller.Get(id, CancellationToken.None);

        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<AuthorDto>(okResponse.Value);

        Assert.Equal(id, dto.Id);

        _mockAuthorService.Verify(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(123)]
    public async Task GetAuthorById_ShouldThrowKeyNotFound_WhenAuthorDoesNotExist(int id)
    {
        // arrange
        var expectedMessage = $"Author with id {id} not found";
        _mockAuthorService
            .Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));

        // act & assert — global exception middleware maps this to 404 ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Get(id, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockAuthorService.Verify(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAuthors_ShouldReturnPagedAuthors_WhenAuthorsExist()
    {
        // arrange
        const int page = 1;
        const int pageSize = 20;

        var pagedAuthors = new PagedList<AuthorModel>
        {
            Items =
            [
                new() { Id = 1, FullName = "David Goggins", Biography = "Can't Heart Me" },
                new() { Id = 2, FullName = "Robert Greene", Biography = "The Laws of Human Nature" }
            ],
            TotalCount = 2,
            CurrentPage = page,
            PageSize = pageSize
        };

        _mockAuthorService
            .Setup(s => s.GetAsync(page, pageSize, It.IsAny<string?>(), It.IsAny<OrderType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedAuthors);

        // act
        var result = await _controller.Get(new PagedRequest { Page = page, PageSize = pageSize }, CancellationToken.None);

        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<PagedList<AuthorDto>>(okResponse.Value);

        Assert.Equal(2, response.Items.Count);
        Assert.Equal(pagedAuthors.TotalCount, response.TotalCount);
        Assert.Equal(page, response.CurrentPage);
        Assert.Equal(pageSize, response.PageSize);
        Assert.Equal(pagedAuthors.Items[0].Id, response.Items[0].Id);
        Assert.Equal(pagedAuthors.Items[1].Id, response.Items[1].Id);

        _mockAuthorService.Verify(s => s.GetAsync(page, pageSize, It.IsAny<string?>(), It.IsAny<OrderType>(), It.IsAny<CancellationToken>()), Times.Once);
    }

[Theory]
    [InlineData(123)]
    public async Task DeleteAuthor_ShouldReturnNoContent_WhenAuthorExists(int id)
    {
        // arrange
        _mockAuthorService
            .Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // act
        var result = await _controller.Delete(id, CancellationToken.None);

        // assert
        Assert.IsType<NoContentResult>(result);

        _mockAuthorService.Verify(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(999)]
    public async Task DeleteAuthor_ShouldThrowKeyNotFoundException_WhenAuthorDoesNotExist(int id)
    {
        // arrange
        var expectedMessage = $"Author with id {id} not found";
        _mockAuthorService
            .Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));

        // act & assert — global exception middleware maps this to 404 ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Delete(id, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockAuthorService.Verify(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
