# Quickstart: Kanban Calendar Board

**Feature**: 001-kanban-calendar  
**Date**: 2026-05-09  

---

## Prerequisites

- .NET 8 SDK
- Node.js 18+
- Docker Desktop
- PostgreSQL 15+ (или через Docker)

---

## Development Setup

### 1. Database

```bash
# Запуск PostgreSQL через Docker
docker run -d \
  --name tasktracker-db \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=tasktracker \
  -p 5432:5432 \
  postgres:15-alpine
```

### 2. Backend

```bash
cd backend

# Установить зависимости
dotnet restore

# Настроить строку подключения (appsettings.Development.json)
{
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Port=5432;Database=tasktracker;Username=postgres;Password=postgres"
  }
}

# Применить миграции
dotnet ef database update --project src/TaskTracker.Infrastructure

# Запустить API
dotnet run --project src/TaskTracker.Api
```

API доступен на `http://localhost:5000`

### 3. Frontend

```bash
cd frontend

# Установить зависимости
npm install

# Настроить API URL (src/.env.development)
VITE_API_URL=http://localhost:5000

# Запустить dev-сервер
npm run dev
```

Frontend доступен на `http://localhost:5173`

---

## Docker Deployment

### Build

```bash
docker-compose build
```

### Run

```bash
docker-compose up -d
```

Сервисы:
- API: `http://localhost:5000`
- Frontend: `http://localhost:80`
- Database: `localhost:5432`

### Logs

```bash
docker-compose logs -f api
docker-compose logs -f frontend
```

### Stop

```bash
docker-compose down
```

---

## Testing

### Backend Tests

```bash
cd backend

# Unit tests
dotnet test tests/TaskTracker.UnitTests

# Integration tests (требует Docker)
dotnet test tests/TaskTracker.IntegrationTests
```

### Frontend Tests

```bash
cd frontend

# Unit tests
npm run test

# E2E tests (требует запущенное приложение)
npm run test:e2e
```

---

## CLI Usage

```bash
cd backend

# Запустить CLI
dotnet run --project src/TaskTracker.Cli -- tasks list --date 2026-05-09

# Создать задачу
dotnet run --project src/TaskTracker.Cli -- tasks create --title "Test" --date 2026-05-09

# Удалить задачу
dotnet run --project src/TaskTracker.Cli -- tasks delete --id <guid>
```

---

## Common Tasks

### Добавить миграцию

```bash
cd backend
dotnet ef migrations add <Name> --project src/TaskTracker.Infrastructure
```

### Сбросить базу данных

```bash
dotnet ef database drop --force --project src/TaskTracker.Infrastructure
dotnet ef database update --project src/TaskTracker.Infrastructure
```

### Очистить кэш npm

```bash
cd frontend
npm cache clean --force
rm -rf node_modules package-lock.json
npm install
```

---

## Troubleshooting

### Database connection error

```bash
# Проверить доступность PostgreSQL
docker ps | grep tasktracker-db
docker logs tasktracker-db
```

### Port conflict

```bash
# Освободить порт 5432
docker stop tasktracker-db
```

### Frontend build errors

```bash
cd frontend
rm -rf node_modules
npm install
```

### Migration errors

```bash
# Удалить все миграции и создать заново
dotnet ef migrations remove --project src/TaskTracker.Infrastructure
dotnet ef migrations add Initial --project src/TaskTracker.Infrastructure
```
