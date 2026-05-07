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
            Title = model.Title,
            Subtitle = model.Subtitle,
            Description = model.Description,
            AuthorId = model.AuthorId,
            CategoryId = model.CategoryId
        };
    }

    public static BookModel ToModel(this Book entity)
    {
        return new BookModel
        {
            Id = entity.Id,
            Title = entity.Title,
            Subtitle = entity.Subtitle,
            Description = entity.Description,
            AuthorId = entity.AuthorId,
            AuthorName = entity.Author?.FullName,
            CategoryId = entity.CategoryId,
            CategoryName = entity.Category?.Name
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
            PasswordHash = model.PasswordHash!,
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

    public static BookCollection ToEntity(this BookCollectionModel model)
    {
        return new BookCollection
        {
            Id = model.Id,
            UserId = model.UserId,
            Name = model.Name,
            Description = model.Description,
            Status = model.Status
        };
    }

    public static BookCollectionModel ToModel(this BookCollection entity)
    {
        return new BookCollectionModel
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Name = entity.Name,
            Description = entity.Description,
            Status = entity.Status,
            Items = entity.Items.Select(i => i.ToModel()).ToList()
        };
    }

    public static BookCollectionItem ToEntity(this BookCollectionItemModel model)
    {
        return new BookCollectionItem
        {
            Id = model.Id,
            BookCollectionId = model.BookCollectionId,
            BookId = model.BookId,
            Status = model.Status,
            Position = model.Position
        };
    }

    public static BookCollectionItemModel ToModel(this BookCollectionItem entity)
    {
        return new BookCollectionItemModel
        {
            Id = entity.Id,
            BookCollectionId = entity.BookCollectionId,
            BookId = entity.BookId,
            BookTitle = entity.Book?.Title,
            Status = entity.Status,
            Position = entity.Position
        };
    }
}
