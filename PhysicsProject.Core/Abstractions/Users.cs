using PhysicsProject.Core.Domain;

namespace PhysicsProject.Core.Abstractions;

public interface IUserRepository
{
    Task<User?> FindByUserNameAsync(string userName, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
}

public interface IUserService
{
    Task<User> RegisterAsync(string userName, string password, CancellationToken ct);
    Task<User?> AuthenticateAsync(string userName, string password, CancellationToken ct);
}


