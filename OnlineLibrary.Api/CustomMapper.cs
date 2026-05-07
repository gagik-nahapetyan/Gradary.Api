using OnlineLibrary.Api.Dtos.Auth;
using OnlineLibrary.Api.Dtos.Author;
using OnlineLibrary.Api.Dtos.Book;
using OnlineLibrary.Api.Dtos.BookCollection;
using OnlineLibrary.Api.Dtos.Category;
using OnlineLibrary.Api.Dtos.Review;
using OnlineLibrary.Api.Dtos.User;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Api;

/// <summary>
/// Represents the <see cref="CustomMapper"/> class.
/// </summary>
public static class CustomMapper
{
    /// <summary>
    /// Maps <see cref="BookRequest"/> to <see cref="BookModel"/>.
    /// </summary>
    /// <param name="dto">The provided object to map from.</param>
    /// <param name="id">The id of the book.</param>
    /// <returns>The book model.</returns>
    public static BookModel ToModel(this BookRequest dto, int id = 0)
    {
        return new BookModel
        {
            Id = id,
            Title = dto.Title,
            Subtitle = dto.Subtitle,
            Description = dto.Description,
            AuthorId = dto.AuthorId,
            CategoryId = dto.CategoryId
        };
    }

    /// <summary>
    /// Maps <see cref="BookModel"/> to <see cref="BookDto"/>.
    /// </summary>
    /// <param name="model">The provided object to map from.</param>
    /// <returns>The book dto.</returns>
    public static BookDto ToDto(this BookModel model)
    {
        return new BookDto
        {
            Id = model.Id,
            Title = model.Title,
            Subtitle = model.Subtitle,
            AuthorId = model.AuthorId,
            AuthorName = model.AuthorName,
            CategoryId = model.CategoryId,
            CategoryName = model.CategoryName,
            Description = model.Description,
            ImageUrl = model.ImageUrl
        };
    }

    /// <summary>
    /// Maps <see cref="BookModel"/> to <see cref="BookListDto"/>.
    /// </summary>
    /// <param name="model">The provided object to map from.</param>
    /// <returns>The book list dto.</returns>
    public static BookListDto ToListDto(this BookModel model)
    {
        return new BookListDto
        {
            Id = model.Id,
            Title = model.Title,
            AuthorName = model.AuthorName,
            CategoryName = model.CategoryName,
            ImageUrl = model.ImageUrl
        };
    }

    /// <summary>
    /// Maps <see cref="AuthorRequest"/> to <see cref="AuthorModel"/>.
    /// </summary>
    /// <param name="dto">The request to map from.</param>
    /// <param name="id">The author id; use 0 for create, existing id for update.</param>
    /// <returns>The author model.</returns>
    public static AuthorModel ToModel(this AuthorRequest dto, int id = 0)
    {
        return new AuthorModel
        {
            Id = id,
            FullName = dto.FullName,
            Biography = dto.Biography
        };
    }

    /// <summary>
    /// Maps <see cref="AuthorModel"/> to <see cref="AuthorDto"/>.
    /// </summary>
    /// <param name="model">The model to map from.</param>
    /// <returns>The author dto.</returns>
    public static AuthorDto ToDto(this AuthorModel model)
    {
        return new AuthorDto
        {
            Id = model.Id,
            FullName = model.FullName,
            Biography = model.Biography,
            ImageUrl = model.ImageUrl
        };
    }

    /// <summary>
    /// Maps <see cref="UserCreateRequest"/> to <see cref="UserModel"/>.
    /// </summary>
    /// <param name="dto">The create request to map from.</param>
    /// <returns>The user model.</returns>
    public static UserModel ToModel(this UserCreateRequest dto)
    {
        return new UserModel
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Role = dto.Role
        };
    }

    /// <summary>
    /// Maps <see cref="UserUpdateRequest"/> to <see cref="UserModel"/>.
    /// Does not carry a password; the service preserves the existing hash.
    /// </summary>
    /// <param name="dto">The update request to map from.</param>
    /// <param name="id">The id of the user being updated.</param>
    /// <returns>The user model.</returns>
    public static UserModel ToModel(this UserUpdateRequest dto, int id)
    {
        return new UserModel
        {
            Id = id,
            FullName = dto.FullName,
            Email = dto.Email,
            Role = dto.Role
        };
    }

    /// <summary>
    /// Maps <see cref="UserModel"/> to <see cref="UserDto"/>. Does not include password hash.
    /// </summary>
    /// <param name="model">The model to map from.</param>
    /// <returns>The user dto.</returns>
    public static UserDto ToDto(this UserModel model)
    {
        return new UserDto
        {
            Id = model.Id,
            FullName = model.FullName,
            Email = model.Email,
            Role = model.Role
        };
    }

    /// <summary>
    /// Maps <see cref="CategoryRequest"/> to <see cref="CategoryModel"/>.
    /// </summary>
    /// <param name="dto">The provided object to map from.</param>
    /// <param name="id">The category id; use 0 for create, existing id for update.</param>
    /// <returns>The category model.</returns>
    public static CategoryModel ToModel(this CategoryRequest dto, int id = 0)
    {
        return new CategoryModel
        {
            Id = id,
            Name = dto.Name,
            Description = dto.Description,
            ParentId = dto.ParentId
        };
    }

    /// <summary>
    /// Maps <see cref="CategoryModel"/> to <see cref="CategoryDto"/>.
    /// </summary>
    /// <param name="model">The model to map from.</param>
    /// <returns>The category dto.</returns>
    public static CategoryDto ToDto(this CategoryModel model)
    {
        return new CategoryDto
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            ParentId = model.ParentId
        };
    }

    /// <summary>
    /// Maps <see cref="ReviewRequest"/> to <see cref="ReviewModel"/>.
    /// </summary>
    /// <param name="dto">The provided object to map from.</param>
    /// <param name="id">The review id; use 0 for create, existing id for update.</param>
    /// <returns>The review model.</returns>
    public static ReviewModel ToModel(this ReviewRequest dto, int id = 0)
    {
        return new ReviewModel
        {
            Id = id,
            UserId = dto.UserId,
            BookId = dto.BookId,
            Rating = dto.Rating,
            Comment = dto.Comment
        };
    }

    /// <summary>
    /// Maps <see cref="AuthResult"/> to <see cref="LoginResponse"/>.
    /// </summary>
    /// <param name="result">The auth result to map from.</param>
    /// <param name="expiresAt">The UTC expiry time of the access token.</param>
    /// <returns>The login response.</returns>
    public static LoginResponse ToResponse(this AuthResult result, DateTime expiresAt)
    {
        return new LoginResponse
        {
            AccessToken = result.AccessToken,
            ExpiresAt = expiresAt,
            User = result.User.ToDto()
        };
    }

    /// <summary>
    /// Maps <see cref="ReviewModel"/> to <see cref="ReviewDto"/>.
    /// </summary>
    /// <param name="model">The model to map from.</param>
    /// <returns>The review dto.</returns>
    public static ReviewDto ToDto(this ReviewModel model)
    {
        return new ReviewDto
        {
            Id = model.Id,
            UserId = model.UserId,
            BookId = model.BookId,
            Rating = model.Rating,
            Comment = model.Comment
        };
    }

    public static BookCollectionModel ToModel(this BookCollectionRequest dto, int userId, int id = 0)
    {
        return new BookCollectionModel
        {
            Id = id,
            UserId = userId,
            Name = dto.Name,
            Description = dto.Description,
            Status = dto.Status
        };
    }

    public static BookCollectionDto ToDto(this BookCollectionModel model)
    {
        return new BookCollectionDto
        {
            Id = model.Id,
            UserId = model.UserId,
            Name = model.Name,
            Description = model.Description,
            Status = model.Status,
            Items = model.Items.Select(i => i.ToDto()).ToList()
        };
    }

    public static BookCollectionItemModel ToModel(this BookCollectionItemRequest dto)
    {
        return new BookCollectionItemModel
        {
            BookId = dto.BookId,
            Status = dto.Status,
            Position = dto.Position
        };
    }

    public static BookCollectionItemDto ToDto(this BookCollectionItemModel model)
    {
        return new BookCollectionItemDto
        {
            Id = model.Id,
            BookId = model.BookId,
            BookTitle = model.BookTitle,
            Status = model.Status,
            Position = model.Position
        };
    }
}
