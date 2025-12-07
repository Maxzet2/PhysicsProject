using Microsoft.AspNetCore.Mvc;
using PhysicsProject.Api.Contracts;
using PhysicsProject.Core.Abstractions;

namespace PhysicsProject.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register(RegisterRequest request, CancellationToken ct)
    {
        try
        {
            var user = await _userService.RegisterAsync(request.UserName, request.Password, ct);
            return Ok(new RegisterResponse(user.Id, user.UserName));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var user = await _userService.AuthenticateAsync(request.UserName, request.Password, ct);
        if (user is null) return Unauthorized(new LoginResponse(Guid.Empty, string.Empty, "Invalid credentials"));

        return Ok(new LoginResponse(user.Id, user.UserName));
    }
}
