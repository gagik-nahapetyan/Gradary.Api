using OnlineLibrary.Api.Dtos;
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
    /// <returns>The book model.</returns>
    public static BookModel ToModel(this BookRequest dto, int id = 0)
    {
        return new BookModel
        {
            Id = id,
            Title = dto.Title,
            AuthorId = dto.AuthorId,
            CategoryId = dto.CategoryId,
            Description = dto.Description
        };
    }

    /// <summary>
    /// Maps <see cref="BookModel"/> to <see cref="BookDto"/>.
    /// </summary>
    /// <param name="dto">The provided object to map from.</param>
    /// <returns>The book dto.</returns>
    public static BookDto ToDto(this BookModel model)
    {
        return new BookDto
        {
            Id = model.Id,
            Title = model.Title,
            AuthorId = model.AuthorId,
            CategoryId = model.CategoryId,
            Description = model.Description
        };
    }

    /// <summary>
    /// Maps <see cref="UserRequest"/> to <see cref="UserModel"/>.
    /// </summary>
    /// <param name="dto">The request to map from.</param>
    /// <param name="id">The user id; use 0 for create, existing id for update.</param>
    /// <returns>The user model.</returns>
    public static UserModel ToModel(this UserRequest dto, int id = 0)
    {
        return new UserModel
        {
            Id = id,
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = string.Empty,
            Role = dto.Role,
            Password = string.IsNullOrEmpty(dto.Password) ? null : dto.Password
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
            Description = dto.Description
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
            Description = model.Description
        };
    }
}
