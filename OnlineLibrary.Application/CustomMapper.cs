using OnlineLibrary.Domain.Entities;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application;

public static class CustomMapper
{
    public static Book ToEntity(this BookModel model)
    {
        return new Book
        {
            Id = model.Id,
            ShortTitle = model.ShortTitle,
            FullTitle = model.FullTitle,
            AuthorId = model.AuthorId,
            CategoryId = model.CategoryId,
            Description = model.Description
        };
    }

    public static BookModel ToModel(this Book entity)
    {
        return new BookModel 
        { 
            Id = entity.Id,
            ShortTitle = entity.ShortTitle,
            FullTitle = entity.FullTitle,
            AuthorId = entity.AuthorId,
            CategoryId = entity.CategoryId,
            Description = entity.Description
        };
    }

    public static Author ToEntity(this AuthorModel model)
    {
        return new Author
        {
            Id = model.Id,
            FullName = model.FullName,
            Biography = model.Biography
        };
    }

    public static AuthorModel ToModel(this Author entity)
    {
        return new AuthorModel
        {
            Id = entity.Id,
            FullName = entity.FullName,
            Biography = entity.Biography
        };
    }

    public static User ToEntity(this UserModel model)
    {
        return new User
        {
            Id = model.Id,
            FullName = model.FullName,
            Email = model.Email,
            PasswordHash = model.PasswordHash,
            Role = model.Role,
            CreatedAt = model.CreatedAt,
            CreatedBy = model.CreatedBy,
            UpdatedAt = model.UpdatedAt,
            UpdatedBy = model.UpdatedBy
        };
    }

    public static UserModel ToModel(this User entity)
    {
        return new UserModel
        {
            Id = entity.Id,
            FullName = entity.FullName,
            Email = entity.Email,
            PasswordHash = entity.PasswordHash,
            Role = entity.Role,
            CreatedAt = entity.CreatedAt,
            CreatedBy = entity.CreatedBy,
            UpdatedAt = entity.UpdatedAt,
            UpdatedBy = entity.UpdatedBy
        };
    }

    public static Category ToEntity(this CategoryModel model)
    {
        return new Category
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            ParentId = model.ParentId
        };
    }

    public static CategoryModel ToModel(this Category entity)
    {
        return new CategoryModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            ParentId = entity.ParentId
        };
    }

    public static Review ToEntity(this ReviewModel model)
    {
        return new Review
        {
            Id = model.Id,
            UserId = model.UserId,
            BookId = model.BookId,
            Rating = model.Rating,
            Comment = model.Comment
        };
    }

    public static ReviewModel ToModel(this Review entity)
    {
        return new ReviewModel
        {
            Id = entity.Id,
            UserId = entity.UserId,
            BookId = entity.BookId,
            Rating = entity.Rating,
            Comment = entity.Comment
        };
    }
}
