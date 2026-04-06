using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using OnlineLibrary.Api.Controllers;
using OnlineLibrary.Api.Dtos.Book;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Models;
using OnlineLibrary.Domain.Settings;

namespace OnlineLibrary.Test.Controllers;

public class BookControllerTests
{
    private readonly Mock<IBookService> _mockBookService;
    private readonly BookController _controller;
    private readonly FileUploadSettings _fileUploadSettings = new() { MaxFileSizeBytes = 26214400 };

    public BookControllerTests()
    {
        _mockBookService = new Mock<IBookService>();
        _controller = new BookController(_mockBookService.Object, Options.Create(_fileUploadSettings));
    }

    [Fact]
    public async Task CreateBook_ShouldCreateBook_WhenInputIsValid()
    {
        // arrange
        var input = new BookRequest
        {
            Title = "Never Finished",
            Subtitle = "Master Your Mind and Defy the Odds",
            Description = "The book about power of will.",
            AuthorId = 1,
            CategoryId = 1
        };

        var createdModel = new BookModel
        {
            Id = 10,
            Title = input.Title,
            Subtitle = input.Subtitle,
            Description = input.Description,
            AuthorId = input.AuthorId,
            CategoryId = input.CategoryId
        };

        _mockBookService
            .Setup(s => s.CreateAsync(It.Is<BookModel>(m =>
                m.Title == input.Title &&
                m.Subtitle == input.Subtitle &&
                m.Description == input.Description &&
                m.AuthorId == input.AuthorId &&
                m.CategoryId == input.CategoryId
            ), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdModel);


        // act
        var result = await _controller.Create(input, CancellationToken.None);


        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<BookDto>(okResponse.Value);

        Assert.Equal(createdModel.Id, dto.Id);
        Assert.Equal(input.AuthorId, dto.AuthorId);
        Assert.Equal(input.Title, dto.Title);
        Assert.Equal(input.Subtitle, dto.Subtitle);
        Assert.Equal(input.CategoryId, dto.CategoryId);
        Assert.Equal(input.Description, dto.Description);

        _mockBookService.Verify(s => s.CreateAsync(It.IsAny<BookModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(5)]
    public async Task UpdateBook_ShouldUpdateBook_WhenInputIsValid(int id)
    {
        // arrange
        var input = new BookRequest
        {
            Title = "Never Finished",
            Subtitle = "Master Your Mind and Defy the Odds",
            Description = "The book about power of will.",
            AuthorId = 1,
            CategoryId = 2
        };

        _mockBookService
            .Setup(s => s.UpdateAsync(It.Is<BookModel>(m =>
                m.Id == id &&
                m.Title == input.Title &&
                m.Subtitle == input.Subtitle &&
                m.Description == input.Description &&
                m.AuthorId == input.AuthorId &&
                m.CategoryId == input.CategoryId
            ), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);


        // act
        var result = await _controller.Update(id, input, CancellationToken.None);


        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<BookDto>(okResponse.Value);

        Assert.Equal(id, dto.Id);
        Assert.Equal(input.Title, dto.Title);
        Assert.Equal(input.Subtitle, dto.Subtitle);
        Assert.Equal(input.AuthorId, dto.AuthorId);
        Assert.Equal(input.CategoryId, dto.CategoryId);
        Assert.Equal(input.Description, dto.Description);

        _mockBookService.Verify(s => s.UpdateAsync(It.IsAny<BookModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(999)]
    public async Task UpdateBook_ShouldThrowKeyNotFoundException_WhenBookDoesNotExist(int id)
    {
        // arrange
        var input = new BookRequest
        {
            Title = "Never Finished",
            AuthorId = 1,
            CategoryId = 1
        };

        var expectedMessage = $"Book with id {id} not found";
        _mockBookService
            .Setup(s => s.UpdateAsync(It.Is<BookModel>(m => m.Id == id), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));


        // act & assert — global exception middleware maps this to 404 ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Update(id, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockBookService.Verify(s => s.UpdateAsync(It.IsAny<BookModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(5)]
    public async Task UploadFile_ShouldReturnNoContent_WhenFileIsValid(int id)
    {
        // arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(1024);
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

        _mockBookService
            .Setup(s => s.UploadFileAsync(id, It.IsAny<Func<Stream>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);


        // act
        var result = await _controller.UploadFile(id, mockFile.Object, CancellationToken.None);


        // assert
        Assert.IsType<NoContentResult>(result);

        _mockBookService.Verify(s => s.UploadFileAsync(id, It.IsAny<Func<Stream>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UploadFile_ShouldReturnBadRequest_WhenFileIsEmpty()
    {
        // arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(0);


        // act
        var result = await _controller.UploadFile(1, mockFile.Object, CancellationToken.None);


        // assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("File must not be empty.", badRequest.Value);

        _mockBookService.Verify(s => s.UploadFileAsync(It.IsAny<int>(), It.IsAny<Func<Stream>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UploadFile_ShouldReturnBadRequest_WhenFileExceedsMaxSize()
    {
        // arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(_fileUploadSettings.MaxFileSizeBytes + 1);


        // act
        var result = await _controller.UploadFile(1, mockFile.Object, CancellationToken.None);


        // assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal($"File size must not exceed {_fileUploadSettings.MaxFileSizeBytes / (1024 * 1024)} MB.", badRequest.Value);

        _mockBookService.Verify(s => s.UploadFileAsync(It.IsAny<int>(), It.IsAny<Func<Stream>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(999)]
    public async Task UploadFile_ShouldThrowKeyNotFoundException_WhenBookDoesNotExist(int id)
    {
        // arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(1024);
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

        var expectedMessage = $"Book with id {id} not found";
        _mockBookService
            .Setup(s => s.UploadFileAsync(id, It.IsAny<Func<Stream>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));


        // act & assert — global exception middleware maps this to 404 ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.UploadFile(id, mockFile.Object, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockBookService.Verify(s => s.UploadFileAsync(id, It.IsAny<Func<Stream>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(5)]
    public async Task GetBookById_ShouldReturnBook_WhenBookExists(int id)
    {
        // arrange
        var book = new BookModel
        {
            Id = id,
            Title = "Never Finished",
            Subtitle = "Master Your Mind and Defy the Odds",
            Description = "The book about power of will.",
            AuthorId = 1,
            CategoryId = 2
        };

        _mockBookService
            .Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);


        // act
        var result = await _controller.Get(id, CancellationToken.None);


        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<BookDto>(okResponse.Value);

        Assert.Equal(book.Id, dto.Id);
        Assert.Equal(book.Title, dto.Title);
        Assert.Equal(book.Subtitle, dto.Subtitle);
        Assert.Equal(book.AuthorId, dto.AuthorId);
        Assert.Equal(book.CategoryId, dto.CategoryId);
        Assert.Equal(book.Description, dto.Description);

        _mockBookService.Verify(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(999)]
    public async Task GetBookById_ShouldThrowKeyNotFoundException_WhenBookDoesNotExist(int id)
    {
        // arrange
        var expectedMessage = $"Book with id {id} not found";
        _mockBookService
            .Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));


        // act & assert — global exception middleware maps this to 404 ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Get(id, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockBookService.Verify(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBooks_ShouldReturnBooks_WhenBooksExist()
    {
        // arrange
        var books = new List<BookModel>
        {
            new() { Id = 1, Title = "Never Finished", AuthorId = 1, CategoryId = 1 },
            new() { Id = 2, Title = "Can't Hurt Me", AuthorId = 1, CategoryId = 1 }
        };

        _mockBookService
            .Setup(s => s.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(books);


        // act
        var result = await _controller.Get(CancellationToken.None);


        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dtos = Assert.IsAssignableFrom<IEnumerable<BookDto>>(okResponse.Value).ToList();

        Assert.Equal(2, dtos.Count);
        Assert.Equal(books[0].Id, dtos[0].Id);
        Assert.Equal(books[0].Title, dtos[0].Title);
        Assert.Equal(books[1].Id, dtos[1].Id);
        Assert.Equal(books[1].Title, dtos[1].Title);

        _mockBookService.Verify(s => s.GetAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBooks_ShouldReturnEmptyList_WhenNoBooksExist()
    {
        // arrange
        _mockBookService
            .Setup(s => s.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);


        // act
        var result = await _controller.Get(CancellationToken.None);


        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dtos = Assert.IsAssignableFrom<IEnumerable<BookDto>>(okResponse.Value);

        Assert.Empty(dtos);

        _mockBookService.Verify(s => s.GetAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(3)]
    public async Task GetBooksByCategoryId_ShouldReturnBooks_WhenCategoryExists(int categoryId)
    {
        // arrange
        var books = new List<BookModel>
        {
            new() { Id = 1, Title = "Never Finished", AuthorId = 1, CategoryId = categoryId },
            new() { Id = 2, Title = "Can't Hurt Me", AuthorId = 1, CategoryId = categoryId }
        };

        _mockBookService
            .Setup(s => s.GetByCategoryIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(books);


        // act
        var result = await _controller.GetByCategoryId(categoryId, CancellationToken.None);


        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dtos = Assert.IsAssignableFrom<IEnumerable<BookDto>>(okResponse.Value).ToList();

        Assert.Equal(books.Count, dtos.Count);
        Assert.All(dtos, dto => Assert.Equal(categoryId, dto.CategoryId));

        _mockBookService.Verify(s => s.GetByCategoryIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(3)]
    public async Task GetBooksByCategoryId_ShouldReturnEmptyList_WhenCategoryHasNoBooks(int categoryId)
    {
        // arrange
        _mockBookService
            .Setup(s => s.GetByCategoryIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);


        // act
        var result = await _controller.GetByCategoryId(categoryId, CancellationToken.None);


        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dtos = Assert.IsAssignableFrom<IEnumerable<BookDto>>(okResponse.Value);

        Assert.Empty(dtos);

        _mockBookService.Verify(s => s.GetByCategoryIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(999)]
    public async Task GetBooksByCategoryId_ShouldThrowKeyNotFoundException_WhenCategoryDoesNotExist(int categoryId)
    {
        // arrange
        var expectedMessage = $"Category with id {categoryId} not found";
        _mockBookService
            .Setup(s => s.GetByCategoryIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));


        // act & assert — global exception middleware maps this to 404 ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.GetByCategoryId(categoryId, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockBookService.Verify(s => s.GetByCategoryIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
    }
}