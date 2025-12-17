using Microsoft.AspNetCore.Mvc;
using PhysicsProject.Api.Contracts;
using PhysicsProject.Core.Abstractions;
using PhysicsProject.Infrastructure.Data;

namespace PhysicsProject.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TemplatesController : ControllerBase
{
    private readonly IProblemRepository _problemRepository;

    public TemplatesController(IProblemRepository problemRepository)
    {
        _problemRepository = problemRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TemplateResponse>>> GetTemplates(CancellationToken ct)
    {
        var templates = await _problemRepository.GetTemplatesAsync(ct);
        var mapped = templates.Select(t => new TemplateResponse(t.Id, t.Name)).ToList();
        return Ok(mapped);
    }

    [HttpGet("default")]
    public ActionResult<TemplateResponse> GetDefaultTemplate()
    {
        return Ok(new TemplateResponse(ProblemTemplateSeed.DefaultTemplateId, "Базовый физический тест"));
    }
}

