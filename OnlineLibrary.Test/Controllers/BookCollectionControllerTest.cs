using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineLibrary.Api.Controllers;
using OnlineLibrary.Api.Dtos.BookCollection;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Enums;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Test.Controllers;

public class BookCollectionControllerTests
{
    private const int CallerId = 1;
    private const int MaxActiveCollections = 10;
    private const int MaxActiveBooksPerCollection = 50;

    private readonly Mock<IBookCollectionService> _mockService;
    private readonly BookCollectionController _controller;

    public BookCollectionControllerTests()
    {
        _mockService = new Mock<IBookCollectionService>();
        _controller = new BookCollectionController(_mockService.Object);
        SetCaller(CallerId);
    }

    private void SetCaller(int id)
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, id.ToString()) };
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"))
            }
        };
    }

    // -------------------------------------------------------------------------
    // Create
    // -------------------------------------------------------------------------

    [Fact]
    public async Task Create_ShouldReturnCollection_WhenInputIsValid()
    {
        // arrange
        var input = new BookCollectionRequest
        {
            Name = "My Reading List",
            Description = "Books I want to read",
            Status = BookCollectionStatus.NotStarted
        };

        var createdModel = new BookCollectionModel
        {
            Id = 10,
            UserId = CallerId,
            Name = input.Name,
            Description = input.Description,
            Status = input.Status
        };

        _mockService
            .Setup(s => s.CreateAsync(
                It.Is<BookCollectionModel>(m =>
                    m.UserId == CallerId &&
                    m.Name == input.Name &&
                    m.Description == input.Description &&
                    m.Status == input.Status),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdModel);


        // act
        var result = await _controller.Create(input, CancellationToken.None);


        // assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<BookCollectionDto>(okResult.Value);

        Assert.Equal(createdModel.Id, dto.Id);
        Assert.Equal(createdModel.UserId, dto.UserId);
        Assert.Equal(input.Name, dto.Name);
        Assert.Equal(input.Description, dto.Description);
        Assert.Equal(input.Status, dto.Status);

        _mockService.Verify(s => s.CreateAsync(It.IsAny<BookCollectionModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_ShouldThrowArgumentException_WhenNameAlreadyExists()
    {
        // arrange
        var input = new BookCollectionRequest { Name = "Duplicate", Status = BookCollectionStatus.NotStarted };
        var expectedMessage = "A collection named 'Duplicate' already exists.";

        _mockService
            .Setup(s => s.CreateAsync(It.IsAny<BookCollectionModel>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException(expectedMessage));


        // act & assert — global exception middleware maps this to 4xx ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _controller.Create(input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockService.Verify(s => s.CreateAsync(It.IsAny<BookCollectionModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_ShouldThrowInvalidOperationException_WhenActiveCollectionLimitReached()
    {
        // arrange
        var input = new BookCollectionRequest { Name = "New List", Status = BookCollectionStatus.NotStarted };
        var expectedMessage = $"Cannot have more than {MaxActiveCollections} active collections.";

        _mockService
            .Setup(s => s.CreateAsync(It.IsAny<BookCollectionModel>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException(expectedMessage));


        // act & assert — global exception middleware maps this to 4xx ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.Create(input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockService.Verify(s => s.CreateAsync(It.IsAny<BookCollectionModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    // -------------------------------------------------------------------------
    // Update
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData(5)]
    public async Task Update_ShouldReturnUpdatedCollection_WhenInputIsValid(int id)
    {
        // arrange
        var input = new BookCollectionRequest
        {
            Name = "Updated List",
            Description = "Updated description",
            Status = BookCollectionStatus.InProgress
        };

        var updatedModel = new BookCollectionModel
        {
            Id = id,
            UserId = CallerId,
            Name = input.Name,
            Description = input.Description,
            Status = input.Status
        };

        _mockService
            .Setup(s => s.UpdateAsync(
                It.Is<BookCollectionModel>(m => m.Id == id && m.UserId == CallerId && m.Name == input.Name),
                CallerId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedModel);


        // act
        var result = await _controller.Update(id, input, CancellationToken.None);


        // assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<BookCollectionDto>(okResult.Value);

        Assert.Equal(id, dto.Id);
        Assert.Equal(input.Name, dto.Name);
        Assert.Equal(input.Status, dto.Status);

        _mockService.Verify(s => s.UpdateAsync(It.IsAny<BookCollectionModel>(), CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(999)]
    public async Task Update_ShouldThrowKeyNotFoundException_WhenCollectionDoesNotExist(int id)
    {
        // arrange
        var input = new BookCollectionRequest { Name = "List", Status = BookCollectionStatus.NotStarted };
        var expectedMessage = "Collection not found.";

        _mockService
            .Setup(s => s.UpdateAsync(It.Is<BookCollectionModel>(m => m.Id == id), CallerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));


        // act & assert — global exception middleware maps this to 4xx ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Update(id, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockService.Verify(s => s.UpdateAsync(It.IsAny<BookCollectionModel>(), CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(5)]
    public async Task Update_ShouldThrowUnauthorizedAccessException_WhenCallerDoesNotOwnCollection(int id)
    {
        // arrange
        var input = new BookCollectionRequest { Name = "List", Status = BookCollectionStatus.NotStarted };
        var expectedMessage = "You do not own this collection.";

        _mockService
            .Setup(s => s.UpdateAsync(It.IsAny<BookCollectionModel>(), CallerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException(expectedMessage));


        // act & assert — global exception middleware maps this to 4xx ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _controller.Update(id, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockService.Verify(s => s.UpdateAsync(It.IsAny<BookCollectionModel>(), CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(5)]
    public async Task Update_ShouldThrowInvalidOperationException_WhenReactivatingCollectionExceedsLimit(int id)
    {
        // arrange
        var input = new BookCollectionRequest { Name = "List", Status = BookCollectionStatus.NotStarted };
        var expectedMessage = $"Cannot have more than {MaxActiveCollections} active collections.";

        _mockService
            .Setup(s => s.UpdateAsync(It.IsAny<BookCollectionModel>(), CallerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException(expectedMessage));


        // act & assert — global exception middleware maps this to 4xx ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.Update(id, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockService.Verify(s => s.UpdateAsync(It.IsAny<BookCollectionModel>(), CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    // -------------------------------------------------------------------------
    // GetById
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData(5)]
    public async Task GetById_ShouldReturnCollection_WhenCollectionExists(int id)
    {
        // arrange
        var model = new BookCollectionModel
        {
            Id = id,
            UserId = CallerId,
            Name = "My List",
            Status = BookCollectionStatus.InProgress,
            Items =
            [
                new BookCollectionItemModel { Id = 1, BookId = 10, Status = BookCollectionItemStatus.Reading, Position = 1 }
            ]
        };

        _mockService
            .Setup(s => s.GetByIdAsync(id, CallerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(model);


        // act
        var result = await _controller.Get(id, CancellationToken.None);


        // assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<BookCollectionDto>(okResult.Value);

        Assert.Equal(id, dto.Id);
        Assert.Equal(CallerId, dto.UserId);
        Assert.Equal(model.Name, dto.Name);
        Assert.Single(dto.Items);

        _mockService.Verify(s => s.GetByIdAsync(id, CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(999)]
    public async Task GetById_ShouldThrowKeyNotFoundException_WhenCollectionDoesNotExist(int id)
    {
        // arrange
        var expectedMessage = "Collection not found.";

        _mockService
            .Setup(s => s.GetByIdAsync(id, CallerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));


        // act & assert — global exception middleware maps this to 4xx ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Get(id, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockService.Verify(s => s.GetByIdAsync(id, CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(5)]
    public async Task GetById_ShouldThrowUnauthorizedAccessException_WhenCallerDoesNotOwnCollection(int id)
    {
        // arrange
        var expectedMessage = "You do not own this collection.";

        _mockService
            .Setup(s => s.GetByIdAsync(id, CallerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException(expectedMessage));


        // act & assert — global exception middleware maps this to 4xx ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _controller.Get(id, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockService.Verify(s => s.GetByIdAsync(id, CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    // -------------------------------------------------------------------------
    // GetByUserId
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetByUserId_ShouldReturnCollections_WhenCollectionsExist()
    {
        // arrange
        var models = new List<BookCollectionModel>
        {
            new() { Id = 1, UserId = CallerId, Name = "List A", Status = BookCollectionStatus.NotStarted },
            new() { Id = 2, UserId = CallerId, Name = "List B", Status = BookCollectionStatus.InProgress }
        };

        _mockService
            .Setup(s => s.GetByUserIdAsync(CallerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(models);


        // act
        var result = await _controller.Get(CancellationToken.None);


        // assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var dtos = Assert.IsAssignableFrom<IEnumerable<BookCollectionDto>>(okResult.Value).ToList();

        Assert.Equal(2, dtos.Count);
        Assert.Equal(models[0].Id, dtos[0].Id);
        Assert.Equal(models[1].Id, dtos[1].Id);

        _mockService.Verify(s => s.GetByUserIdAsync(CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByUserId_ShouldReturnEmptyList_WhenNoCollectionsExist()
    {
        // arrange
        _mockService
            .Setup(s => s.GetByUserIdAsync(CallerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);


        // act
        var result = await _controller.Get(CancellationToken.None);


        // assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var dtos = Assert.IsAssignableFrom<IEnumerable<BookCollectionDto>>(okResult.Value);

        Assert.Empty(dtos);

        _mockService.Verify(s => s.GetByUserIdAsync(CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    // -------------------------------------------------------------------------
    // AddBook
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData(5)]
    public async Task AddBook_ShouldReturnItem_WhenInputIsValid(int collectionId)
    {
        // arrange
        var input = new BookCollectionItemRequest
        {
            BookId = 42,
            Status = BookCollectionItemStatus.WantToRead,
            Position = 1
        };

        var createdItem = new BookCollectionItemModel
        {
            Id = 100,
            BookCollectionId = collectionId,
            BookId = input.BookId,
            Status = input.Status,
            Position = input.Position
        };

        _mockService
            .Setup(s => s.AddBookAsync(
                collectionId,
                It.Is<BookCollectionItemModel>(m => m.BookId == input.BookId && m.Status == input.Status),
                CallerId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdItem);


        // act
        var result = await _controller.AddBook(collectionId, input, CancellationToken.None);


        // assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<BookCollectionItemDto>(okResult.Value);

        Assert.Equal(createdItem.Id, dto.Id);
        Assert.Equal(input.BookId, dto.BookId);
        Assert.Equal(input.Status, dto.Status);
        Assert.Equal(input.Position, dto.Position);

        _mockService.Verify(s => s.AddBookAsync(collectionId, It.IsAny<BookCollectionItemModel>(), CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(999)]
    public async Task AddBook_ShouldThrowKeyNotFoundException_WhenCollectionDoesNotExist(int collectionId)
    {
        // arrange
        var input = new BookCollectionItemRequest { BookId = 1, Status = BookCollectionItemStatus.WantToRead };
        var expectedMessage = "Collection not found.";

        _mockService
            .Setup(s => s.AddBookAsync(collectionId, It.IsAny<BookCollectionItemModel>(), CallerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));


        // act & assert — global exception middleware maps this to 4xx ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.AddBook(collectionId, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockService.Verify(s => s.AddBookAsync(collectionId, It.IsAny<BookCollectionItemModel>(), CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(5)]
    public async Task AddBook_ShouldThrowUnauthorizedAccessException_WhenCallerDoesNotOwnCollection(int collectionId)
    {
        // arrange
        var input = new BookCollectionItemRequest { BookId = 1, Status = BookCollectionItemStatus.WantToRead };
        var expectedMessage = "You do not own this collection.";

        _mockService
            .Setup(s => s.AddBookAsync(collectionId, It.IsAny<BookCollectionItemModel>(), CallerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException(expectedMessage));


        // act & assert — global exception middleware maps this to 4xx ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _controller.AddBook(collectionId, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockService.Verify(s => s.AddBookAsync(collectionId, It.IsAny<BookCollectionItemModel>(), CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(5, 999)]
    public async Task AddBook_ShouldThrowKeyNotFoundException_WhenBookDoesNotExist(int collectionId, int bookId)
    {
        // arrange
        var input = new BookCollectionItemRequest { BookId = bookId, Status = BookCollectionItemStatus.WantToRead };
        var expectedMessage = $"Book with id {bookId} not found.";

        _mockService
            .Setup(s => s.AddBookAsync(collectionId, It.Is<BookCollectionItemModel>(m => m.BookId == bookId), CallerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));


        // act & assert — global exception middleware maps this to 4xx ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.AddBook(collectionId, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockService.Verify(s => s.AddBookAsync(collectionId, It.IsAny<BookCollectionItemModel>(), CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(5, 42)]
    public async Task AddBook_ShouldThrowArgumentException_WhenBookAlreadyInCollection(int collectionId, int bookId)
    {
        // arrange
        var input = new BookCollectionItemRequest { BookId = bookId, Status = BookCollectionItemStatus.WantToRead };
        var expectedMessage = $"Book with id {bookId} is already in this collection.";

        _mockService
            .Setup(s => s.AddBookAsync(collectionId, It.Is<BookCollectionItemModel>(m => m.BookId == bookId), CallerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException(expectedMessage));


        // act & assert — global exception middleware maps this to 4xx ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _controller.AddBook(collectionId, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockService.Verify(s => s.AddBookAsync(collectionId, It.IsAny<BookCollectionItemModel>(), CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(5)]
    public async Task AddBook_ShouldThrowInvalidOperationException_WhenActiveBookLimitReached(int collectionId)
    {
        // arrange
        var input = new BookCollectionItemRequest { BookId = 1, Status = BookCollectionItemStatus.WantToRead };
        var expectedMessage = $"Cannot have more than {MaxActiveBooksPerCollection} active books in a collection.";

        _mockService
            .Setup(s => s.AddBookAsync(collectionId, It.IsAny<BookCollectionItemModel>(), CallerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException(expectedMessage));


        // act & assert — global exception middleware maps this to 4xx ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.AddBook(collectionId, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockService.Verify(s => s.AddBookAsync(collectionId, It.IsAny<BookCollectionItemModel>(), CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(5, 3)]
    public async Task AddBook_ShouldThrowInvalidOperationException_WhenPositionIsAlreadyOccupied(int collectionId, int position)
    {
        // arrange
        var input = new BookCollectionItemRequest { BookId = 1, Status = BookCollectionItemStatus.WantToRead, Position = position };
        var expectedMessage = $"Position {position} is already occupied in this collection.";

        _mockService
            .Setup(s => s.AddBookAsync(collectionId, It.IsAny<BookCollectionItemModel>(), CallerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException(expectedMessage));


        // act & assert — global exception middleware maps this to 4xx ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.AddBook(collectionId, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockService.Verify(s => s.AddBookAsync(collectionId, It.IsAny<BookCollectionItemModel>(), CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    // -------------------------------------------------------------------------
    // UpdateBook
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData(5, 42)]
    public async Task UpdateBook_ShouldReturnUpdatedItem_WhenInputIsValid(int collectionId, int bookId)
    {
        // arrange
        var input = new BookCollectionItemRequest
        {
            BookId = bookId,
            Status = BookCollectionItemStatus.Reading,
            Position = 2
        };

        var updatedItem = new BookCollectionItemModel
        {
            Id = 100,
            BookCollectionId = collectionId,
            BookId = bookId,
            Status = input.Status,
            Position = input.Position
        };

        _mockService
            .Setup(s => s.UpdateBookAsync(
                collectionId,
                It.Is<BookCollectionItemModel>(m => m.BookId == bookId && m.Status == input.Status),
                CallerId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedItem);


        // act
        var result = await _controller.UpdateBook(collectionId, bookId, input, CancellationToken.None);


        // assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<BookCollectionItemDto>(okResult.Value);

        Assert.Equal(updatedItem.Id, dto.Id);
        Assert.Equal(bookId, dto.BookId);
        Assert.Equal(input.Status, dto.Status);
        Assert.Equal(input.Position, dto.Position);

        _mockService.Verify(s => s.UpdateBookAsync(collectionId, It.IsAny<BookCollectionItemModel>(), CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(999, 1)]
    public async Task UpdateBook_ShouldThrowKeyNotFoundException_WhenCollectionDoesNotExist(int collectionId, int bookId)
    {
        // arrange
        var input = new BookCollectionItemRequest { BookId = bookId, Status = BookCollectionItemStatus.Reading };
        var expectedMessage = "Collection not found.";

        _mockService
            .Setup(s => s.UpdateBookAsync(collectionId, It.IsAny<BookCollectionItemModel>(), CallerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));


        // act & assert — global exception middleware maps this to 4xx ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.UpdateBook(collectionId, bookId, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockService.Verify(s => s.UpdateBookAsync(collectionId, It.IsAny<BookCollectionItemModel>(), CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(5, 1)]
    public async Task UpdateBook_ShouldThrowUnauthorizedAccessException_WhenCallerDoesNotOwnCollection(int collectionId, int bookId)
    {
        // arrange
        var input = new BookCollectionItemRequest { BookId = bookId, Status = BookCollectionItemStatus.Reading };
        var expectedMessage = "You do not own this collection.";

        _mockService
            .Setup(s => s.UpdateBookAsync(collectionId, It.IsAny<BookCollectionItemModel>(), CallerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException(expectedMessage));


        // act & assert — global exception middleware maps this to 4xx ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _controller.UpdateBook(collectionId, bookId, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockService.Verify(s => s.UpdateBookAsync(collectionId, It.IsAny<BookCollectionItemModel>(), CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(5, 999)]
    public async Task UpdateBook_ShouldThrowKeyNotFoundException_WhenBookNotInCollection(int collectionId, int bookId)
    {
        // arrange
        var input = new BookCollectionItemRequest { BookId = bookId, Status = BookCollectionItemStatus.Reading };
        var expectedMessage = $"Book with id {bookId} not found in this collection.";

        _mockService
            .Setup(s => s.UpdateBookAsync(collectionId, It.Is<BookCollectionItemModel>(m => m.BookId == bookId), CallerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));


        // act & assert — global exception middleware maps this to 4xx ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.UpdateBook(collectionId, bookId, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockService.Verify(s => s.UpdateBookAsync(collectionId, It.IsAny<BookCollectionItemModel>(), CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(5, 42)]
    public async Task UpdateBook_ShouldThrowInvalidOperationException_WhenTransitioningToActiveExceedsLimit(int collectionId, int bookId)
    {
        // arrange
        var input = new BookCollectionItemRequest { BookId = bookId, Status = BookCollectionItemStatus.WantToRead };
        var expectedMessage = $"Cannot have more than {MaxActiveBooksPerCollection} active books in a collection.";

        _mockService
            .Setup(s => s.UpdateBookAsync(collectionId, It.IsAny<BookCollectionItemModel>(), CallerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException(expectedMessage));


        // act & assert — global exception middleware maps this to 4xx ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.UpdateBook(collectionId, bookId, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockService.Verify(s => s.UpdateBookAsync(collectionId, It.IsAny<BookCollectionItemModel>(), CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(5, 42, 3)]
    public async Task UpdateBook_ShouldThrowInvalidOperationException_WhenPositionIsAlreadyOccupied(int collectionId, int bookId, int position)
    {
        // arrange
        var input = new BookCollectionItemRequest { BookId = bookId, Status = BookCollectionItemStatus.Reading, Position = position };
        var expectedMessage = $"Position {position} is already occupied in this collection.";

        _mockService
            .Setup(s => s.UpdateBookAsync(collectionId, It.IsAny<BookCollectionItemModel>(), CallerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException(expectedMessage));


        // act & assert — global exception middleware maps this to 4xx ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.UpdateBook(collectionId, bookId, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockService.Verify(s => s.UpdateBookAsync(collectionId, It.IsAny<BookCollectionItemModel>(), CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    // -------------------------------------------------------------------------
    // RemoveBook
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData(5, 42)]
    public async Task RemoveBook_ShouldReturnNoContent_WhenBookIsInCollection(int collectionId, int bookId)
    {
        // arrange
        _mockService
            .Setup(s => s.RemoveBookAsync(collectionId, bookId, CallerId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);


        // act
        var result = await _controller.RemoveBook(collectionId, bookId, CancellationToken.None);


        // assert
        Assert.IsType<NoContentResult>(result);

        _mockService.Verify(s => s.RemoveBookAsync(collectionId, bookId, CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(999, 1)]
    public async Task RemoveBook_ShouldThrowKeyNotFoundException_WhenCollectionDoesNotExist(int collectionId, int bookId)
    {
        // arrange
        var expectedMessage = "Collection not found.";

        _mockService
            .Setup(s => s.RemoveBookAsync(collectionId, bookId, CallerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));


        // act & assert — global exception middleware maps this to 4xx ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.RemoveBook(collectionId, bookId, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockService.Verify(s => s.RemoveBookAsync(collectionId, bookId, CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(5, 1)]
    public async Task RemoveBook_ShouldThrowUnauthorizedAccessException_WhenCallerDoesNotOwnCollection(int collectionId, int bookId)
    {
        // arrange
        var expectedMessage = "You do not own this collection.";

        _mockService
            .Setup(s => s.RemoveBookAsync(collectionId, bookId, CallerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException(expectedMessage));


        // act & assert — global exception middleware maps this to 4xx ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _controller.RemoveBook(collectionId, bookId, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockService.Verify(s => s.RemoveBookAsync(collectionId, bookId, CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(5, 999)]
    public async Task RemoveBook_ShouldThrowKeyNotFoundException_WhenBookNotInCollection(int collectionId, int bookId)
    {
        // arrange
        var expectedMessage = $"Book with id {bookId} not found in this collection.";

        _mockService
            .Setup(s => s.RemoveBookAsync(collectionId, bookId, CallerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));


        // act & assert — global exception middleware maps this to 4xx ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.RemoveBook(collectionId, bookId, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockService.Verify(s => s.RemoveBookAsync(collectionId, bookId, CallerId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
