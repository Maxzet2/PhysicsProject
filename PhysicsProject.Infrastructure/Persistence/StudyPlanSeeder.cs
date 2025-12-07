using Microsoft.EntityFrameworkCore;
using PhysicsProject.Infrastructure.Data;
using PhysicsProject.Infrastructure.Persistence.Entities;

namespace PhysicsProject.Infrastructure.Persistence;

public sealed class StudyPlanSeeder
{
    private readonly PhysicsProjectDbContext _dbContext;

    public StudyPlanSeeder(PhysicsProjectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task EnsureSeededAsync(CancellationToken ct)
    {
        if (await _dbContext.Chapters.AnyAsync(ct))
        {
            return;
        }

        foreach (var chapter in StudyPlanSeed.DefaultPlan)
        {
            var chapterEntity = new ChapterEntity
            {
                Id = chapter.Id,
                Title = chapter.Title,
                Description = chapter.Description,
                OrderIndex = chapter.OrderIndex
            };

            foreach (var section in chapter.Sections)
            {
                chapterEntity.Sections.Add(new SectionEntity
                {
                    Id = section.Id,
                    ChapterId = section.ChapterId,
                    Title = section.Title,
                    Description = section.Description,
                    OrderIndex = section.OrderIndex,
                    TemplateId = section.TemplateId,
                    DefaultQuestionCount = section.DefaultQuestionCount,
                    TestTimeLimitSeconds = section.TestTimeLimitSeconds
                });
            }

            _dbContext.Chapters.Add(chapterEntity);
        }

        await _dbContext.SaveChangesAsync(ct);
    }
}
