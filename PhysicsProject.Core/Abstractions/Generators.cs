using Microsoft.Extensions.DependencyInjection;
using PhysicsProject.Core.Domain;

namespace PhysicsProject.Core.Abstractions;

public interface ITaskGenerator
{
    ProblemInstance Generate(ProblemTemplate template, long seed, GenerationConstraints constraints);
    bool CanHandle(string templateType);
}

public interface IAnswerEvaluator
{
    EvaluationResult Evaluate(ProblemInstance instance, UserAnswer answer);
    bool CanHandle(string templateType);
}

public interface ITemplateValidator
{
    ValidationResult Validate(ProblemTemplate template);
}

public sealed record GenerationConstraints(int? MaxAttempts = 100);
public sealed record UserAnswer(string Raw);
public sealed record EvaluationResult(bool IsCorrect, decimal ScoreAwarded = 0m, string Feedback = "");
public sealed record ValidationResult(bool IsValid, string Message = "");

public static class CoreAbstractionsRegistration
{
    public static IServiceCollection AddCoreAbstractions(this IServiceCollection services)
    {
        return services;
    }
}


