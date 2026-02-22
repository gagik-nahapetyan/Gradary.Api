using OnlineLibrary.Domain.Entities;

namespace OnlineLibrary.Application.Abstractions.Repositories;

/// <summary>
/// Represents an abstraction of the user repository.
/// </summary>
public interface IUserRepository : IRepository<User>
{
}
