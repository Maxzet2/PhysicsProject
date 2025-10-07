# PhysicsProject

Разделённый проект: Core (домен), Application (use-cases), Infrastructure (БД, внешние сервисы), Api (ASP.NET Core Web API).

## Запуск локально

```bash
cd PhysicsProject
dotnet build
cd PhysicsProject.Api
dotnet run
```

API будет доступно на http://localhost:5120, OpenAPI при Development.

## Слои
- Core: доменные модели и интерфейсы, `services.AddCore()`
- Application: обработчики случаев использования, `services.AddApplication()`
- Infrastructure: реализации портов (например, Postgres), `services.AddInfrastructure()`
- Api: минимальный хост, контроллеры, Swagger/OpenAPI

Здоровье: GET /health
