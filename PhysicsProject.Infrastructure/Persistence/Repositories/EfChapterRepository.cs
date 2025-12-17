using System.Linq;
using Microsoft.EntityFrameworkCore;
using PhysicsProject.Core.Abstractions;
using PhysicsProject.Core.Domain;
using PhysicsProject.Infrastructure.Persistence.Entities;

namespace PhysicsProject.Infrastructure.Persistence.Repositories;

public sealed class EfChapterRepository : IChapterRepository
{
    private readonly PhysicsProjectDbContext _dbContext;

    public EfChapterRepository(PhysicsProjectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Chapter>> GetAllAsync(CancellationToken ct)
    {
        var entities = await _dbContext.Chapters
            .AsNoTracking()
            .Include(c => c.Sections)
            .OrderBy(c => c.OrderIndex)
            .ToListAsync(ct);

        return entities.Select(MapToDomain).ToArray();
    }

    public async Task<Section?> GetSectionByIdAsync(Guid sectionId, CancellationToken ct)
    {
        var entity = await _dbContext.Sections
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == sectionId, ct);
        if (entity is null)
        {
            return null;
        }

        return new Section
        {
            Id = entity.Id,
            ChapterId = entity.ChapterId,
            Title = entity.Title,
            Description = entity.Description,
            OrderIndex = entity.OrderIndex,
            TemplateId = entity.TemplateId,
            DefaultQuestionCount = entity.DefaultQuestionCount,
            TestTimeLimitSeconds = entity.TestTimeLimitSeconds
        };
    }

    private static Chapter MapToDomain(ChapterEntity entity)
    {
        var sections = entity.Sections
            .OrderBy(s => s.OrderIndex)
            .Select(s => new Section
            {
                Id = s.Id,
                ChapterId = s.ChapterId,
                Title = s.Title,
                Description = s.Description,
                OrderIndex = s.OrderIndex,
                TemplateId = s.TemplateId,
                DefaultQuestionCount = s.DefaultQuestionCount
            })
            .ToArray();

        return new Chapter
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            OrderIndex = entity.OrderIndex,
            Sections = sections
        };
    }
}
