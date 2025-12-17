using Microsoft.Extensions.DependencyInjection;
using PhysicsProject.Core.Abstractions;
using PhysicsProject.Core.Services;
using PhysicsProject.Core.Generators;  

namespace PhysicsProject.Core;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddCoreAbstractions();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IStudyPlanService, StudyPlanService>();
        services.AddScoped<ISectionFlowService, SectionFlowService>();
        services.AddScoped<ITaskGenerator, FileTemplateGenerator>();
        services.AddScoped<IAnswerEvaluator, SimpleAnswerEvaluator>();

        return services;
    }
}
