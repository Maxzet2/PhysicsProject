using PhysicsProject.Core;
using PhysicsProject.Application;
using PhysicsProject.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Wire up modular layers
builder.Services.AddCore();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();
// Simple health endpoint
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
