
# PhysicsProject

## Требования

- **.NET 9 SDK** (https://dotnet.microsoft.com/download)
- **Node.js** (18+; https://nodejs.org/)
- **npm** (обычно входит в Node.js)
- **PostgreSQL** (14+, локально или через Docker)
- **Docker** (для контейнеризации, опционально)

## Быстрый старт (Makefile)

```bash
# 1. Клонируйте репозиторий и перейдите в папку проекта
cd PhysicsProject

# 2. Установите зависимости (NuGet + npm)
make install

# 3. Соберите и скопируйте фронтенд в API
make sync-static

# 4. Запустите PostgreSQL (локально или через Docker)
# Пример через Docker:
docker compose up -d db

# 5. Запустите API (SPA уже скопирован в wwwroot)
make api-run

# 6. Для разработки фронта отдельно:
make frontend-dev
```

## Основные команды Makefile

| Команда            | Описание                                                      |
|-------------------|--------------------------------------------------------------|
| `make install`    | Установить все зависимости (dotnet + npm)                     |
| `make sync-static`| Собрать SPA и скопировать в wwwroot API                       |
| `make api-run`    | Запустить API (SPA уже внутри)                                |
| `make frontend-dev`| Запустить Vite dev server для фронта                         |
| `make docker-up`  | Собрать SPA, скопировать, поднять всё через docker compose   |
| `make clean`      | Очистить dist и wwwroot                                       |

## Ручной запуск без Makefile

1. Установите зависимости:
	- `dotnet restore`
	- `cd PhysicsProject.Web && npm install`
2. Соберите фронтенд:
	- `cd PhysicsProject.Web && npm run build`
3. Скопируйте содержимое `PhysicsProject.Web/dist` в `PhysicsProject.Api/wwwroot`
4. Запустите PostgreSQL (или используйте Docker Compose)
5. Запустите API:
	- `dotnet run --project PhysicsProject.Api`

## Настройка базы данных

- По умолчанию строка подключения: `Host=localhost;Port=5432;Database=physics_project;Username=postgres;Password=postgres`
- Можно изменить в `PhysicsProject.Api/appsettings.Development.json` или через переменную окружения `ConnectionStrings__Default`
- Миграции и сиды применяются автоматически при старте API

## Запуск через Docker Compose

```bash
make docker-up
```
- API будет доступен на http://localhost:8080
- База — на 5432 (логин/пароль: postgres)

## Типовые проблемы

- **Кнопки "Тренировка"/"Тест" не работают:**
  - Проверьте, что база данных запущена и доступна
  - Проверьте, что SPA собран и скопирован в wwwroot
  - Проверьте, что файл `default_template.json` попал в папку вывода API (это делается автоматически)
- **Ошибка подключения к БД:**
  - Проверьте строку подключения и что Postgres слушает на нужном порту

## TODO/Планы
- Прогрессивное открытие разделов (сначала тренировка, потом тест)
- Улучшение UX и логики отображения прогресса
- Документация по API

---

Для любых вопросов: @Maxzet2
