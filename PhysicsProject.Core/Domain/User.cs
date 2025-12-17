namespace PhysicsProject.Core.Domain;

public sealed class User
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public void SetPasswordHash(string hash)
    {
        PasswordHash = hash;
    }
}


