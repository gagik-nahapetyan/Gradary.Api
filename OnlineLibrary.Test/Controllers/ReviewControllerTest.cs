using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineLibrary.Api.Controllers;
using OnlineLibrary.Api.Dtos.Review;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Enums;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Test.Controllers;

public class ReviewControllerTests
{
    private readonly Mock<IReviewService> _mockReviewService;
    private readonly ReviewController _controller;

    public ReviewControllerTests()
    {
        _mockReviewService = new Mock<IReviewService>();
        _controller = new ReviewController(_mockReviewService.Object);
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

    [Fact]
    public async Task CreateReview_ShouldCreateReview_WhenInputIsValid()
    {
        // arrange
        var input = new ReviewRequest
        {
            UserId = 1,
            BookId = 5,
            Rating = BookRating.FourStars,
            Comment = "Great book"
        };

        var createdModel = new ReviewModel
        {
            Id = 10,
            UserId = input.UserId,
            BookId = input.BookId,
            Rating = input.Rating,
            Comment = input.Comment
        };

        _mockReviewService
            .Setup(s => s.CreateAsync(
                It.Is<ReviewModel>(m =>
                    m.UserId == input.UserId &&
                    m.BookId == input.BookId &&
                    m.Rating == input.Rating &&
                    m.Comment == input.Comment),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdModel);


        // act
        var result = await _controller.Create(input, CancellationToken.None);


        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<ReviewDto>(okResponse.Value);

        Assert.Equal(createdModel.Id, dto.Id);
        Assert.Equal(input.UserId, dto.UserId);
        Assert.Equal(input.BookId, dto.BookId);
        Assert.Equal(input.Rating, dto.Rating);
        Assert.Equal(input.Comment, dto.Comment);

        _mockReviewService.Verify(s => s.CreateAsync(It.IsAny<ReviewModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(10)]
    public async Task UpdateReview_ShouldUpdateReview_WhenInputIsValid(int id)
    {
        // arrange
        var input = new ReviewRequest
        {
            UserId = 1,
            BookId = 5,
            Rating = BookRating.FiveStars,
            Comment = "Excellent!"
        };

        _mockReviewService
            .Setup(s => s.UpdateAsync(
                It.Is<ReviewModel>(m =>
                    m.Id == id &&
                    m.UserId == input.UserId &&
                    m.BookId == input.BookId &&
                    m.Rating == input.Rating &&
                    m.Comment == input.Comment),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);


        // act
        var result = await _controller.Update(id, input, CancellationToken.None);


        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<ReviewDto>(okResponse.Value);

        Assert.Equal(id, dto.Id);
        Assert.Equal(input.UserId, dto.UserId);
        Assert.Equal(input.BookId, dto.BookId);
        Assert.Equal(input.Rating, dto.Rating);
        Assert.Equal(input.Comment, dto.Comment);

        _mockReviewService.Verify(s => s.UpdateAsync(It.IsAny<ReviewModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateReview_ShouldThrowKeyNotFoundException_WhenBookDoesNotExist()
    {
        // arrange
        var input = new ReviewRequest
        {
            UserId = 1,
            BookId = 999,
            Rating = BookRating.FourStars,
            Comment = "Great book"
        };

        var expectedMessage = $"Book with id {input.BookId} not found";
        _mockReviewService
            .Setup(s => s.CreateAsync(It.Is<ReviewModel>(m => m.BookId == input.BookId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));

        // act & assert — global exception middleware maps this to 404 ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Create(input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockReviewService.Verify(s => s.CreateAsync(It.IsAny<ReviewModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateReview_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
    {
        // arrange
        var input = new ReviewRequest
        {
            UserId = 999,
            BookId = 5,
            Rating = BookRating.FourStars,
            Comment = "Great book"
        };

        var expectedMessage = $"User with id {input.UserId} not found";
        _mockReviewService
            .Setup(s => s.CreateAsync(It.Is<ReviewModel>(m => m.UserId == input.UserId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));

        // act & assert — global exception middleware maps this to 404 ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Create(input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockReviewService.Verify(s => s.CreateAsync(It.IsAny<ReviewModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(999)]
    public async Task UpdateReview_ShouldThrowKeyNotFoundException_WhenReviewDoesNotExist(int id)
    {
        // arrange
        var input = new ReviewRequest
        {
            UserId = 1,
            BookId = 5,
            Rating = BookRating.ThreeStars,
            Comment = "Okay"
        };

        var expectedMessage = $"Review with id {id} not found";
        _mockReviewService
            .Setup(s => s.UpdateAsync(It.Is<ReviewModel>(m => m.Id == id), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));


        // act & assert — global exception middleware maps this to 404 ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Update(id, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockReviewService.Verify(s => s.UpdateAsync(It.IsAny<ReviewModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(10)]
    public async Task UpdateReview_ShouldThrowKeyNotFoundException_WhenBookDoesNotExist(int id)
    {
        // arrange
        var input = new ReviewRequest
        {
            UserId = 1,
            BookId = 999,
            Rating = BookRating.FiveStars,
            Comment = "Excellent!"
        };

        var expectedMessage = $"Book with id {input.BookId} not found";
        _mockReviewService
            .Setup(s => s.UpdateAsync(It.Is<ReviewModel>(m => m.Id == id && m.BookId == input.BookId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));

        // act & assert — global exception middleware maps this to 404 ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Update(id, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockReviewService.Verify(s => s.UpdateAsync(It.IsAny<ReviewModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(10)]
    public async Task UpdateReview_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist(int id)
    {
        // arrange
        var input = new ReviewRequest
        {
            UserId = 999,
            BookId = 5,
            Rating = BookRating.FiveStars,
            Comment = "Excellent!"
        };

        var expectedMessage = $"User with id {input.UserId} not found";
        _mockReviewService
            .Setup(s => s.UpdateAsync(It.Is<ReviewModel>(m => m.Id == id && m.UserId == input.UserId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));

        // act & assert — global exception middleware maps this to 404 ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Update(id, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockReviewService.Verify(s => s.UpdateAsync(It.IsAny<ReviewModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(10)]
    public async Task GetReviewById_ShouldReturnReview_WhenReviewExists(int id)
    {
        // arrange
        var review = new ReviewModel
        {
            Id = id,
            UserId = 1,
            BookId = 5,
            Rating = BookRating.FourStars,
            Comment = "Great book"
        };

        _mockReviewService
            .Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(review);


        // act
        var result = await _controller.Get(id, CancellationToken.None);


        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<ReviewDto>(okResponse.Value);

        Assert.Equal(review.Id, dto.Id);
        Assert.Equal(review.UserId, dto.UserId);
        Assert.Equal(review.BookId, dto.BookId);
        Assert.Equal(review.Rating, dto.Rating);
        Assert.Equal(review.Comment, dto.Comment);

        _mockReviewService.Verify(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(999)]
    public async Task GetReviewById_ShouldThrowKeyNotFoundException_WhenReviewDoesNotExist(int id)
    {
        // arrange
        var expectedMessage = $"Review with id {id} not found";
        _mockReviewService
            .Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));


        // act & assert — global exception middleware maps this to 404 ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Get(id, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockReviewService.Verify(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(5)]
    public async Task GetReviewsByBookId_ShouldReturnReviews_WhenReviewsExist(int bookId)
    {
        // arrange
        var reviews = new List<ReviewModel>
        {
            new() { Id = 1, UserId = 1, BookId = bookId, Rating = BookRating.FiveStars, Comment = "Loved it!" },
            new() { Id = 2, UserId = 2, BookId = bookId, Rating = BookRating.ThreeStars, Comment = "It was okay." }
        };

        _mockReviewService
            .Setup(s => s.GetByBookIdAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reviews);


        // act
        var result = await _controller.GetByBookId(bookId, CancellationToken.None);


        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dtos = Assert.IsType<List<ReviewDto>>(okResponse.Value);

        Assert.Equal(2, dtos.Count);
        Assert.Equal(reviews[0].Id, dtos[0].Id);
        Assert.Equal(reviews[1].Id, dtos[1].Id);

        _mockReviewService.Verify(s => s.GetByBookIdAsync(bookId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(5)]
    public async Task GetReviewsByBookId_ShouldReturnEmptyList_WhenNoReviewsExist(int bookId)
    {
        // arrange
        _mockReviewService
            .Setup(s => s.GetByBookIdAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);


        // act
        var result = await _controller.GetByBookId(bookId, CancellationToken.None);


        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dtos = Assert.IsType<List<ReviewDto>>(okResponse.Value);

        Assert.Empty(dtos);

        _mockReviewService.Verify(s => s.GetByBookIdAsync(bookId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(10)]
    public async Task DeleteReview_ShouldReturnNoContent_WhenCallerIsOwner(int id)
    {
        // arrange
        const int callerId = 1;
        SetCaller(callerId);

        _mockReviewService
            .Setup(s => s.DeleteAsync(id, callerId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);


        // act
        var result = await _controller.Delete(id, CancellationToken.None);


        // assert
        Assert.IsType<NoContentResult>(result);

        _mockReviewService.Verify(s => s.DeleteAsync(id, callerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(999)]
    public async Task DeleteReview_ShouldThrowKeyNotFoundException_WhenReviewDoesNotExist(int id)
    {
        // arrange
        const int callerId = 1;
        SetCaller(callerId);

        var expectedMessage = $"Review with id {id} not found";
        _mockReviewService
            .Setup(s => s.DeleteAsync(id, callerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));


        // act & assert — global exception middleware maps this to 404 ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Delete(id, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockReviewService.Verify(s => s.DeleteAsync(id, callerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(10)]
    public async Task DeleteReview_ShouldThrowUnauthorizedAccessException_WhenCallerIsNotOwner(int id)
    {
        // arrange
        const int callerId = 2;
        SetCaller(callerId);

        var expectedMessage = "You do not own this review.";
        _mockReviewService
            .Setup(s => s.DeleteAsync(id, callerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException(expectedMessage));


        // act & assert — global exception middleware maps this to 403 ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _controller.Delete(id, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _mockReviewService.Verify(s => s.DeleteAsync(id, callerId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
