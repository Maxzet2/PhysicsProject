using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PhysicsProject.Core.Abstractions;
using PhysicsProject.Core.Domain;
using PhysicsProject.Infrastructure.Persistence.Entities;
using PhysicsProject.Infrastructure.Persistence;

namespace PhysicsProject.Infrastructure.Persistence.Repositories;

public sealed class EfProblemRepository : IProblemRepository
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly PhysicsProjectDbContext _dbContext;

    public EfProblemRepository(PhysicsProjectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProblemTemplate?> GetTemplateByIdAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.ProblemTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, ct);
        return entity is null ? null : MapToDomain(entity);
    }

    public async Task SaveInstanceAsync(ProblemInstance instance, CancellationToken ct)
    {
        var entity = await _dbContext.ProblemInstances.FirstOrDefaultAsync(i => i.Id == instance.Id, ct);
        if (entity is null)
        {
            entity = new ProblemInstanceEntity { Id = instance.Id };
            _dbContext.ProblemInstances.Add(entity);
        }

        entity.TemplateId = instance.TemplateId;
        entity.Seed = instance.Seed;
        entity.TemplateType = instance.TemplateType;
        entity.Statement = instance.Statement;
        entity.ParametersJson = SerializeParameters(instance.Parameters);
        entity.NormalizedCorrectAnswer = instance.NormalizedCorrectAnswer;

        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<ProblemInstance?> GetInstanceByIdAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.ProblemInstances
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id, ct);
        return entity is null ? null : MapToDomain(entity);
    }

    public async Task<IEnumerable<ProblemTemplate>> GetTemplatesAsync(CancellationToken ct)
    {
        var entities = await _dbContext.ProblemTemplates
            .AsNoTracking()
            .ToListAsync(ct);
        return entities.Select(MapToDomain).ToArray();
    }

    private static ProblemTemplate MapToDomain(ProblemTemplateEntity entity)
    {
        var template = new ProblemTemplate
        {
            Id = entity.Id,
            Name = entity.Name,
            TemplateType = entity.TemplateType,
            JsonSpec = entity.JsonSpec
        };

        if (!entity.IsActive)
        {
            template.Deactivate();
        }

        return template;
    }

    private static ProblemInstance MapToDomain(ProblemInstanceEntity entity)
    {
        var parameters = DeserializeParameters(entity.ParametersJson);
        var instance = new ProblemInstance
        {
            Id = entity.Id,
            TemplateId = entity.TemplateId,
            Seed = entity.Seed,
            Statement = entity.Statement,
            Parameters = parameters,
            NormalizedCorrectAnswer = entity.NormalizedCorrectAnswer
        };
        instance.AssignTemplateInfo(entity.TemplateType);
        return instance;
    }

    private static string SerializeParameters(IReadOnlyDictionary<string, object> parameters)
        => JsonSerializer.Serialize(parameters, SerializerOptions);

    private static IReadOnlyDictionary<string, object> DeserializeParameters(string payload)
    {
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(payload, SerializerOptions) ?? new();
        return dictionary;
    }
}
