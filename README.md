# Task Tracker — Канбан-доска с календарём

Веб-приложение для управления задачами с календарной навигацией. Вдохновлено канбан-досками, но вместо спринтов используются даты.

## Возможности

- **Просмотр задач на дату**: Три колонки — "Новые", "В процессе", "Сделаны"
- **Навигация по датам**: Кнопки назад/вперёд и date-picker
- **Создание задач**: Нажатие "+" в любой колонке
- **Редактирование**: Изменение названия, описания и даты задачи
- **Удаление**: С подтверждением
- **Drag-n-drop**: Перетаскивание между колонками и внутри колонки
- **Массовые операции**: Выбор нескольких задач и групповое удаление/перемещение

## Быстрый старт

### Запуск из Docker Hub (рекомендуется)

Образы публикуются в Docker Hub: [`niquitos1985`](https://hub.docker.com/r/niquitos1985)

```bash
# Windows (PowerShell)
.\docker-start.ps1

# Linux/Mac
./docker-start.sh
```

После запуска:
- Frontend: http://localhost:3000
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger

### Сборка и публикация образов

```bash
# Сборка образов (Windows)
.\docker-build.ps1

# Сборка образов (Linux/Mac)
./docker-build.sh

# Публикация в Docker Hub (требуется авторизация)
docker login
.\docker-push.ps1  # или ./docker-push.sh
```

### Локальная разработка

#### Backend

```bash
cd backend/src/TaskTracker.Api
dotnet run
```

#### Frontend

```bash
cd frontend
npm install
npm run dev
```

## Технологии

| Компонент | Технология |
|-----------|------------|
| Frontend | React 18, TypeScript, Zustand, React DnD |
| Backend | ASP.NET Core 8, C# 12, EF Core 8 |
| База данных | PostgreSQL 15 |
| Контейнеризация | Docker, Docker Compose |

## Структура проекта

```
task-tracker/
├── frontend/          # React приложение
│   ├── src/
│   │   ├── components/
│   │   ├── stores/
│   │   ├── services/
│   │   └── types/
│   ├── Dockerfile
│   └── package.json
├── backend/           # ASP.NET Core API
│   ├── src/
│   │   ├── TaskTracker.Api/
│   │   ├── TaskTracker.Application/
│   │   ├── TaskTracker.Domain/
│   │   └── TaskTracker.Infrastructure/
│   ├── Dockerfile
│   └── TaskTracker.sln
├── docker-compose.yml
└── README.md
```

## API Endpoints

| Метод | Endpoint | Описание |
|-------|----------|----------|
| GET | /api/tasks?date=YYYY-MM-DD | Получить задачи на дату |
| POST | /api/tasks | Создать задачу |
| PUT | /api/tasks/{id} | Обновить задачу |
| DELETE | /api/tasks/{id} | Удалить задачу |
| POST | /api/tasks/bulk/delete | Массовое удаление |
| POST | /api/tasks/bulk/move | Массовое перемещение |

## Переменные окружения

### Backend

| Переменная | Значение по умолчанию |
|------------|----------------------|
| ASPNETCORE_ENVIRONMENT | Development |
| ASPNETCORE_URLS | http://+:8080 |
| ConnectionStrings__DefaultConnection | Host=db;Database=tasktracker;Username=postgres;Password=postgres |

### Frontend

| Переменная | Значение по умолчанию |
|------------|----------------------|
| VITE_API_URL | http://localhost:5000/api |

## Лицензия

MIT
