using Microsoft.EntityFrameworkCore;
using PhysicsProject.Core.Abstractions;
using PhysicsProject.Core.Domain;
using PhysicsProject.Infrastructure.Persistence.Entities;
using PhysicsProject.Infrastructure.Persistence;

namespace PhysicsProject.Infrastructure.Persistence.Repositories;

public sealed class EfUserRepository : IUserRepository
{
    private readonly PhysicsProjectDbContext _dbContext;

    public EfUserRepository(PhysicsProjectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> FindByUserNameAsync(string userName, CancellationToken ct)
    {
        var entity = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == userName, ct);
        return entity is null ? null : MapToDomain(entity);
    }

    public async Task AddAsync(User user, CancellationToken ct)
    {
        var entity = new UserEntity
        {
            Id = user.Id,
            UserName = user.UserName,
            PasswordHash = user.PasswordHash,
            CreatedAt = user.CreatedAt
        };
        _dbContext.Users.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
    }

    private static User MapToDomain(UserEntity entity)
    {
        var user = new User
        {
            Id = entity.Id,
            UserName = entity.UserName,
            CreatedAt = entity.CreatedAt
        };
        user.SetPasswordHash(entity.PasswordHash);
        return user;
    }
}
