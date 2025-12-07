using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PhysicsProject.Infrastructure.Data;
using PhysicsProject.Infrastructure.Persistence.Entities;

namespace PhysicsProject.Infrastructure.Persistence;

public sealed class ProblemTemplateSeeder
{
    private readonly PhysicsProjectDbContext _dbContext;
    private readonly IHostEnvironment _environment;

    public ProblemTemplateSeeder(PhysicsProjectDbContext dbContext, IHostEnvironment environment)
    {
        _dbContext = dbContext;
        _environment = environment;
    }

    public async Task EnsureSeededAsync(CancellationToken ct)
    {
        if (await _dbContext.ProblemTemplates.AnyAsync(ct))
        {
            return;
        }

        var templates = ProblemTemplateSeed.LoadDefaultTemplates(_environment.ContentRootPath);
        foreach (var template in templates)
        {
            _dbContext.ProblemTemplates.Add(new ProblemTemplateEntity
            {
                Id = template.Id,
                Name = template.Name,
                TemplateType = template.TemplateType,
                JsonSpec = template.JsonSpec,
                IsActive = true
            });
        }

        await _dbContext.SaveChangesAsync(ct);
    }
}
