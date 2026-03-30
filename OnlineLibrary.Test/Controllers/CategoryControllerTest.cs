using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineLibrary.Api.Controllers;
using OnlineLibrary.Api.Dtos.Category;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Test.Controllers;

public class CategoryControllerTest
{
    private readonly Mock<ICategoryService> _categoryServiceMock;
    private readonly CategoryController _controller;

    public CategoryControllerTest()
    {
        _categoryServiceMock = new Mock<ICategoryService>();
        _controller = new CategoryController(_categoryServiceMock.Object);
    }

    [Fact]
    public async Task CreateCategory_ShouldCreateCategory_WhenInputIsValid()
    {
        // arrange
        var input = new CategoryRequest
        {
            Name = "Nonfiction",
            Description = "Nonfiction category",
            ParentId = null
        };

        var createdModel = new CategoryModel
        {
            Id = 123,
            Name = input.Name,
            Description = input.Description,
            ParentId = input.ParentId,
        };

        _categoryServiceMock
            .Setup(s => s.CreateAsync(It.Is<CategoryModel>(c =>
                c.Name == input.Name &&
                c.Description == input.Description &&
                c.ParentId == input.ParentId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdModel);


        // act
        var result = await _controller.Create(input, CancellationToken.None);

        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<CategoryDto>(okResponse.Value);

        Assert.Equal(createdModel.Id, dto.Id);
        Assert.Equal(createdModel.Name, dto.Name);
        Assert.Equal(createdModel.Description, dto.Description);
        Assert.Equal(createdModel.ParentId, dto.ParentId);

        _categoryServiceMock.Verify(s => s.CreateAsync(It.IsAny<CategoryModel>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task CreateCategory_ShouldCreateSubCategory_WhenInputIsValid()
    {
        // arrange
        var input = new CategoryRequest
        {
            Name = "Historical",
            Description = "Historical is a subcategory of a nonfiction category.",
            ParentId = 123
        };

        var createdModel = new CategoryModel
        {
            Id = 234,
            Name = input.Name,
            Description = input.Description,
            ParentId = input.ParentId,
        };

        _categoryServiceMock
            .Setup(s => s.CreateAsync(
                It.Is<CategoryModel>(c =>
                    c.Name == input.Name &&
                    c.Description == input.Description &&
                    c.ParentId == input.ParentId), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdModel);

        // act
        var result = await _controller.Create(input, CancellationToken.None);

        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<CategoryDto>(okResponse.Value);

        Assert.Equal(createdModel.Id, dto.Id);
        Assert.Equal(createdModel.Name, dto.Name);
        Assert.Equal(createdModel.Description, dto.Description);
        Assert.Equal(createdModel.ParentId, dto.ParentId);

        _categoryServiceMock.Verify(s => s.CreateAsync(It.IsAny<CategoryModel>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task CreateCategory_ShouldThrowKeyNotFound_WhenParentDoesNotExist()
    {
        // arrange
        var input = new CategoryRequest
        {
            Name = "Historical",
            Description = "Historical is a subcategory of a nonfiction category.",
            ParentId = -1
        };

        var expectedMessage = $"Parent category with id {input.ParentId.Value} not found.";

        _categoryServiceMock
            .Setup(s => s.CreateAsync(
                It.Is<CategoryModel>(c =>
                    c.Name == input.Name &&
                    c.Description == input.Description &&
                    c.ParentId == input.ParentId), 
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));

        // act & assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Create(input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _categoryServiceMock.Verify(s => s.CreateAsync(It.IsAny<CategoryModel>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Theory]
    [InlineData(123)]
    public async Task UpdateCategory_ShouldUpdateCategory_WhenInputIsValid(int id)
    {
        // arrange
        var input = new CategoryRequest
        {
            Name = "Nonfiction",
            Description = "Updated description",
            ParentId = null
        };

        _categoryServiceMock
            .Setup(s => s.UpdateAsync(
                It.Is<CategoryModel>(c =>
                    c.Id == id &&
                    c.Name == input.Name &&
                    c.Description == input.Description &&
                    c.ParentId == input.ParentId), 
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // act
        var result = await _controller.Update(id, input, CancellationToken.None);

        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<CategoryDto>(okResponse.Value);

        Assert.Equal(id, dto.Id);
        Assert.Equal(input.Name, dto.Name);
        Assert.Equal(input.Description, dto.Description);
        Assert.Equal(input.ParentId, dto.ParentId);

        _categoryServiceMock.Verify(s => s.UpdateAsync(It.IsAny<CategoryModel>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Theory]
    [InlineData(234)]
    public async Task UpdateCategory_ShouldUpdateSubCategory_WhenInputIsValid(int id)
    {
        // arrange
        var input = new CategoryRequest
        {
            Name = "Historical",
            Description = "Updated subcategory description.",
            ParentId = 123
        };

        _categoryServiceMock
            .Setup(s => s.UpdateAsync(
                It.Is<CategoryModel>(c =>
                    c.Id == id &&
                    c.Name == input.Name &&
                    c.Description == input.Description &&
                    c.ParentId == input.ParentId), 
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // act
        var result = await _controller.Update(id, input, CancellationToken.None);

        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<CategoryDto>(okResponse.Value);

        Assert.Equal(id, dto.Id);
        Assert.Equal(input.Name, dto.Name);
        Assert.Equal(input.Description, dto.Description);
        Assert.Equal(input.ParentId, dto.ParentId);

        _categoryServiceMock.Verify(s => s.UpdateAsync(It.IsAny<CategoryModel>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Theory]
    [InlineData(123)]
    public async Task UpdateCategory_ShouldThrowKeyNotFound_WhenCategoryDoesNotExist(int id)
    {
        // arrange
        var input = new CategoryRequest
        {
            Name = "Nonfiction",
            Description = "Updated description",
            ParentId = null
        };

        var expectedMessage = $"Category with id {id} not found";

        _categoryServiceMock
            .Setup(s => s.UpdateAsync(
                It.Is<CategoryModel>(c =>
                    c.Id == id &&
                    c.Name == input.Name &&
                    c.Description == input.Description &&
                    c.ParentId == input.ParentId), 
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));

        // act & assert — global exception middleware maps this to 404 ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Update(id, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _categoryServiceMock.Verify(s => s.UpdateAsync(It.IsAny<CategoryModel>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Theory]
    [InlineData(234)]
    public async Task UpdateCategory_ShouldThrowKeyNotFound_WhenParentDoesNotExist(int id)
    {
        // arrange
        var input = new CategoryRequest
        {
            Name = "Historical",
            Description = "Updated subcategory description.",
            ParentId = -1
        };

        var expectedMessage = $"Parent category with id {input.ParentId.Value} not found.";

        _categoryServiceMock
            .Setup(s => s.UpdateAsync(
                It.Is<CategoryModel>(c =>
                    c.Id == id &&
                    c.Name == input.Name &&
                    c.Description == input.Description &&
                    c.ParentId == input.ParentId), 
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));

        // act & assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Update(id, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _categoryServiceMock.Verify(s => s.UpdateAsync(It.IsAny<CategoryModel>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Theory]
    [InlineData(234)]
    public async Task UpdateCategory_ShouldThrowArgumentException_WhenParentIdEqualsCategoryId(int id)
    {
        // arrange
        var input = new CategoryRequest
        {
            Name = "Science",
            Description = "Cannot be own parent",
            ParentId = id
        };

        var expected = new ArgumentException("Category cannot be its own parent.", nameof(CategoryModel.ParentId));

        _categoryServiceMock
            .Setup(s => s.UpdateAsync(
                It.Is<CategoryModel>(c =>
                    c.Id == id &&
                    c.Name == input.Name &&
                    c.Description == input.Description &&
                    c.ParentId == input.ParentId), 
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(expected);

        // act & arrange
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _controller.Update(id, input, CancellationToken.None));
        Assert.Equal(expected.Message, ex.Message);

        _categoryServiceMock.Verify(s => s.UpdateAsync(It.IsAny<CategoryModel>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Theory]
    [InlineData(234)]
    public async Task UpdateCategory_ShouldThrowInvalidOperationException_WhenHierarchyWouldCycle(int id)
    {
        var input = new CategoryRequest
        {
            Name = "Science",
            Description = "Would introduce a cycle",
            ParentId = 2
        };

        var expectedMessage = "Category hierarchy cannot contain cycles.";

        _categoryServiceMock
            .Setup(s => s.UpdateAsync(
                It.Is<CategoryModel>(c =>
                    c.Id == id &&
                    c.Name == input.Name &&
                    c.Description == input.Description &&
                    c.ParentId == input.ParentId), 
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException(expectedMessage));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.Update(id, input, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _categoryServiceMock.Verify(s => s.UpdateAsync(It.IsAny<CategoryModel>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Theory]
    [InlineData(123)]
    public async Task GetCategoryById_ShouldReturnCategory_WhenCategoryExists(int id)
    {
        // arrange
        var category = new CategoryModel
        {
            Id = id,
            Name = "Science",
            Description = "Science Category",
            ParentId = null
        };

        _categoryServiceMock.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(category);

        // act
        var result = await _controller.Get(id, CancellationToken.None);

        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<CategoryDto>(okResponse.Value);

        Assert.Equal(id, dto.Id);

        _categoryServiceMock.Verify(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(123)]
    public async Task GetCategoryById_ShouldThrowKeyNotFound_WhenCategoryDoesNotExist(int id)
    {
        // arrange
        var expectedMessage = $"Category with id {id} not found";
        _categoryServiceMock
            .Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedMessage));

        // act & assert — global exception middleware maps this to 404 ProblemDetails when the API runs end-to-end
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Get(id, CancellationToken.None));
        Assert.Equal(expectedMessage, ex.Message);

        _categoryServiceMock.Verify(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCategories_ShouldReturnCategories_WhenCategoriesExist()
    {
        // arrange
        var categories = new List<CategoryModel>
        {
            new()
            {
                Id = 1,
                Name = "Fiction",
                Description = "Fiction books",
                ParentId = null
            },
            new()
            {
                Id = 2,
                Name = "Biography",
                Description = "Biographical books",
                ParentId = 1
            }
        };

        _categoryServiceMock.Setup(s => s.GetAsync(It.IsAny<CancellationToken>())).ReturnsAsync(categories);

        // act
        var result = await _controller.Get(CancellationToken.None);

        // assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        var dtos = Assert.IsType<List<CategoryDto>>(okResponse.Value);

        Assert.Equal(2, dtos.Count);
        Assert.Equal(categories[0].Id, dtos[0].Id);
        Assert.Equal(categories[1].Id, dtos[1].Id);

        _categoryServiceMock.Verify(s => s.GetAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
