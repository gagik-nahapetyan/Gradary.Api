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
            Title = entity.Title,
            AuthorId = entity.AuthorId,
            CategoryId = entity.CategoryId,
            Description = entity.Description
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
}
