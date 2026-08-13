using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.Interfaces;

/// <summary>
/// Represents a repository interface for performing CRUD operations on User entities.
/// </summary>
public interface IUserRepository : IRepository<UserEntity> { }
