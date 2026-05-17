# Quickstart: Назначение исполнителей задач

## Backend

Миграция БД:

```bash
cd backend/src/TaskTracker.Infrastructure
dotnet ef migrations add AddAssigneeToTask
dotnet ef database update
```

## Frontend

Сборка:

```bash
cd frontend
npm install   # если новые зависимости
npm run build
```

## Запуск

```bash
docker-compose up --build
```

## Тестирование

```bash
# Backend
cd backend
dotnet test

# Frontend
cd frontend
npm test
```
