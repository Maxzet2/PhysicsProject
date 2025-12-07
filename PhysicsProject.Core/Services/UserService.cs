using System.Security.Cryptography;
using System.Text;
using PhysicsProject.Core.Abstractions;
using PhysicsProject.Core.Domain;

namespace PhysicsProject.Core.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _users;

    public UserService(IUserRepository users)
    {
        _users = users;
    }

    public async Task<User> RegisterAsync(string userName, string password, CancellationToken ct)
    {
        var existing = await _users.FindByUserNameAsync(userName, ct);
        if (existing is not null)
            throw new InvalidOperationException("UserName already registered");

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = userName
        };
        user.SetPasswordHash(HashPassword(password));
        await _users.AddAsync(user, ct);
        return user;
    }

    public async Task<User?> AuthenticateAsync(string userName, string password, CancellationToken ct)
    {
        var existing = await _users.FindByUserNameAsync(userName, ct);
        if (existing is null) return null;
        return VerifyPassword(password, existing.PasswordHash) ? existing : null;
    }

    private static string HashPassword(string password)
    {
        // placeholder hashing (replace with BCrypt/Argon2 in Infrastructure)
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }

    private static bool VerifyPassword(string password, string hash)
        => HashPassword(password).Equals(hash, StringComparison.OrdinalIgnoreCase);
}


