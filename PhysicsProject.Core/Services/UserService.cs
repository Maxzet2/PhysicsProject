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

    public async Task<User> RegisterAsync(string email, string password, CancellationToken ct)
    {
        var existing = await _users.FindByEmailAsync(email, ct);
        if (existing is not null)
            throw new InvalidOperationException("Email already registered");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email
        };
        user.SetPasswordHash(HashPassword(password));
        await _users.AddAsync(user, ct);
        return user;
    }

    private static string HashPassword(string password)
    {
        // placeholder hashing (replace with BCrypt/Argon2 in Infrastructure)
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}


