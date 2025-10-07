using PhysicsProject.Core.Domain;

namespace PhysicsProject.Core.Abstractions;

public interface IUserRepository
{
    Task<User?> FindByEmailAsync(string email, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
}

public interface IUserService
{
    Task<User> RegisterAsync(string email, string password, CancellationToken ct);
}


