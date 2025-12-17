using Microsoft.AspNetCore.Mvc;

namespace PhysicsProject.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DemoController : ControllerBase
    {
        public record DemoQuestion(Guid Id, string Title, string Statement, string[] Options);
        public record DemoSession(Guid Id, Guid UserId, DateTimeOffset StartedAt, DemoQuestion[] Items);

        [HttpPost("create-demo-session")]
        public IActionResult CreateDemoSession([FromQuery] int numItems = 10)
        {
            var rnd = new Random();
            var items = Enumerable.Range(1, numItems).Select(i =>
                new DemoQuestion(
                    Guid.NewGuid(),
                    $"Вопрос {i}",
                    $"Текст задачи {i}: тело задачи с параметром = {rnd.Next(1,100)}",
                    new[] { "A", "B", "C", "D" }
                )
            ).ToArray();

            var session = new DemoSession(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow, items);
            return Ok(session);
        }
    }
}