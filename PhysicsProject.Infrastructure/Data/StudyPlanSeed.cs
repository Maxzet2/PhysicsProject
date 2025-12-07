using PhysicsProject.Core.Domain;

namespace PhysicsProject.Infrastructure.Data;

public static class StudyPlanSeed
{
    public static IReadOnlyList<Chapter> DefaultPlan => new List<Chapter>
    {
        new()
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"),
            Title = "Механика",
            Description = "Базовые основы движения и взаимодействий",
            OrderIndex = 1,
            Sections = new []
            {
                new Section
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1"),
                    ChapterId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"),
                    Title = "Кинематика",
                    Description = "Законы движения материальной точки",
                    OrderIndex = 1,
                    TemplateId = ProblemTemplateSeed.DefaultTemplateId,
                    DefaultQuestionCount = 10,
                    TestTimeLimitSeconds = 600
                },
                new Section
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2"),
                    ChapterId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"),
                    Title = "Динамика",
                    Description = "Силы и законы Ньютона",
                    OrderIndex = 2,
                    TemplateId = ProblemTemplateSeed.DefaultTemplateId,
                    DefaultQuestionCount = 10,
                    TestTimeLimitSeconds = 600
                }
            }
        }
    };
}
