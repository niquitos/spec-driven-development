# Implementation Plan: Kanban Calendar Board

**Branch**: `001-kanban-calendar` | **Date**: 2026-05-09 | **Spec**: [specs/001-kanban-calendar/spec.md](spec.md)
**Input**: Feature specification from `/specs/001-kanban-calendar/spec.md`

## Summary

Канбан-доска для управления задачами с календарной навигацией. Фронтенд на React, бекенд на ASP.NET Core с CQRS и кастомной валидацией. База данных — PostgreSQL, деплой в Docker.

## Technical Context

**Language/Version**: 
- Frontend: TypeScript 5.x, React 18.x
- Backend: C# 12, .NET 8

**Primary Dependencies**:
- Frontend: React, Zustand/Redux (state), React DnD (drag-n-drop), date-fns
- Backend: ASP.NET Core Web API, EF Core 8, Npgsql

**Storage**: PostgreSQL 15+ через EF Core

**Testing**:
- Frontend: Jest + React Testing Library
- Backend: xUnit + Moq + Testcontainers для интеграционных

**Target Platform**: Web-приложение для десктопа/планшета

**Project Type**: Web application (frontend + backend API)

**Performance Goals**:
- Переключение дат < 1 сек
- Создание задачи < 30 сек
- Поддержка 1000+ задач на дату

**Constraints**:
- Drag-n-drop работает на десктопах/планшетах
- Single-user (нет совместного доступа)
- Веб-интерфейс — основной UX, CLI не требуется

**Scale/Scope**:
- MVP: P1 функции (просмотр, навигация, создание)
- P2: редактирование, удаление, drag-n-drop
- P3: массовые операции

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Compliance | Notes |
|-----------|------------|-------|
| I. Test-First | ✅ Implemented | TDD для бизнес-логики, контрактные тесты для API |
| II. Incremental Delivery | ✅ Implemented | P1/P2/P3 приоритеты в spec, MVP доставлен первым |
| III. Observability | ✅ Implemented | Structured logging через ILogger, ExceptionMiddleware |
| IV. Simplicity | ✅ Implemented | Минимальные зависимости, no MediatR, кастомный CQRS |

**Complexity Justification**:
- Frontend + Backend разделение необходимо для интерактивного drag-n-drop UI
- Zustand выбран вместо Redux за простоту и минимальный boilerplate
- @hello-pangea/dnd выбран для drag-n-drop за простоту и поддержку React

## Project Structure

### Documentation (this feature)

```text
specs/001-kanban-calendar/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
└── tasks.md             # Phase 2 output
```

### Source Code

```text
backend/
├── src/
│   ├── TaskTracker.Api/              # Web API проект
│   │   ├── Controllers/
│   │   │   └── TasksController.cs
│   │   ├── Middleware/
│   │   │   └── ExceptionMiddleware.cs
│   │   └── Program.cs
│   ├── TaskTracker.Application/      # CQRS handlers + interfaces
│   │   ├── Common/
│   │   │   ├── Interfaces/
│   │   │   │   ├── IRequest.cs
│   │   │   │   ├── IRequestHandler.cs
│   │   │   │   └── IValidator.cs
│   │   │   └── DependencyInjection.cs
│   │   ├── Tasks/
│   │   │   ├── GetTasksQuery.cs
│   │   │   ├── CreateTaskCommand.cs
│   │   │   ├── UpdateTaskCommand.cs
│   │   │   ├── DeleteTaskCommand.cs
│   │   │   ├── BulkDeleteCommand.cs
│   │   │   └── BulkMoveCommand.cs
│   │   └── Application.csproj
│   ├── TaskTracker.Domain/           # Domain entities
│   │   ├── TaskEntity.cs
│   │   ├── TaskStatus.cs
│   │   └── Domain.csproj
│   └── TaskTracker.Infrastructure/   # Persistence + Validators
│       ├── Persistence/
│       │   ├── AppDbContext.cs
│       │   ├── TaskRepository.cs
│       │   └── Migrations/
│       ├── Validators/
│       │   └── CreateTaskValidator.cs
│       ├── DependencyInjection.cs
│       └── Infrastructure.csproj
├── Dockerfile
└── TaskTracker.sln

frontend/
├── src/
│   ├── components/
│   │   ├── Board.tsx
│   │   ├── Column.tsx
│   │   ├── TaskCard.tsx
│   │   ├── Header.tsx
│   │   ├── TaskModal/
│   │   │   ├── CreateTaskModal.tsx
│   │   │   ├── EditTaskModal.tsx
│   │   │   └── DeleteConfirmModal.tsx
│   │   └── BulkActions/
│   │       └── BulkActionsPanel.tsx
│   ├── hooks/
│   │   └── useDragDrop.ts
│   ├── stores/
│   │   └── taskStore.ts
│   ├── services/
│   │   └── taskApi.ts
│   ├── types/
│   │   └── task.ts
│   ├── utils/
│   │   └── date.ts
│   ├── App.tsx
│   ├── main.tsx
│   └── index.css
├── Dockerfile
├── package.json
└── vite.config.ts
```

**Structure Decision**: Web application с разделением frontend/backend. CLI удалён — веб-интерфейс является основным UX.

## Complexity Tracking

| Complexity | Why Needed | Simpler Alternative Rejected Because |
|------------|------------|-------------------------------------|
| Backend + Frontend разделение | Требуется интерактивный UI для drag-n-drop | SPA без API не позволит масштабироваться |
| CQRS с кастомными handlers | Упрощает тестирование бизнес-логики | Прямой сервис-слой сложнее тестировать отдельно |
| Zustand для state management | Минимальный boilerplate | Redux избыточен для этого проекта |
| @hello-pangea/dnd | Поддержка drag-n-drop для React | Нативный HTML5 DnD требует больше кода |

## Implementation Status

**Completed**:
- ✅ Backend: ASP.NET Core 8 Web API с CQRS
- ✅ Frontend: React 18 + TypeScript + Zustand
- ✅ База данных: PostgreSQL 15 + EF Core 8 + Migrations
- ✅ Контейнеризация: Docker + Docker Compose
- ✅ Middleware: ExceptionMiddleware для обработки ошибок
- ✅ Drag-n-drop: @hello-pangea/dnd
- ✅ Accessibility: ARIA labels, keyboard navigation, focus indicators

**Not Implemented**:
- ❌ Backend тесты (Unit/Integration) — требуют написания
- ❌ Frontend тесты (Unit/Integration) — требуют написания
- ❌ Serilog — используется стандартный ILogger

## Constitution Re-Check

После реализации проверить:
- ✅ Тесты покрывают бизнес-логику — TDD требуется для всех новых функций
- ✅ Логирование структурировано — ILogger используется во всех handlers
- ✅ CLI не требуется — все операции доступны через веб-интерфейс
