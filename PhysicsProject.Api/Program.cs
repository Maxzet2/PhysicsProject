// filepath: PhysicsProject.Api/Program.cs
using PhysicsProject.Core;
using PhysicsProject.Application;
using PhysicsProject.Infrastructure;
using PhysicsProject.Core.Abstractions;
using PhysicsProject.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCore(); // если есть в проекте
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();

// Добавьте регистрацию реализации ITemplateService, если она не регистрируется внутри AddCore/AddInfrastructure
builder.Services.AddScoped<ITemplateService, TemplateService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("Default");
app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", () => Results.Ok("OK"));
app.Run();
