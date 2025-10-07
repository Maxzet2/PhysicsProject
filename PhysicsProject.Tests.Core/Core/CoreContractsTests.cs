using Microsoft.Extensions.DependencyInjection;
using PhysicsProject.Core;
using PhysicsProject.Core.Domain;
using Xunit;

namespace PhysicsProject.Tests.Core;

public class CoreContractsTests
{
    [Fact]
    public void CanRegisterCoreServices()
    {
        var services = new ServiceCollection();
        services.AddCore();
        var provider = services.BuildServiceProvider();
        Assert.NotNull(provider);
    }

    [Fact]
    public void CanConstructDomainModels()
    {
        var template = new ProblemTemplate
        {
            Id = Guid.NewGuid(),
            Name = "Kinematics",
            TemplateType = "kinematics.v1",
            JsonSpec = "{}"
        };
        var instance = new ProblemInstance
        {
            Id = Guid.NewGuid(),
            TemplateId = template.Id,
            Seed = 123,
            Statement = "Test",
            Parameters = new Dictionary<string, object>(),
            NormalizedCorrectAnswer = "42"
        };
        Assert.Equal(template.Id, instance.TemplateId);
    }
}


