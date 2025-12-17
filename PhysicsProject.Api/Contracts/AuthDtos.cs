namespace PhysicsProject.Api.Contracts;

public record RegisterRequest(string UserName, string Password);
public record RegisterResponse(Guid UserId, string UserName, string Message = "Registration successful");

public record LoginRequest(string UserName, string Password);
public record LoginResponse(Guid UserId, string UserName, string Message = "Login successful");

