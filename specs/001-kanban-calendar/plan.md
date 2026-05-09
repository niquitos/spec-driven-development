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
- CLI для основных операций бекенда

**Scale/Scope**:
- MVP: P1 функции (просмотр, навигация, создание)
- P2: редактирование, удаление, drag-n-drop
- P3: массовые операции

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Compliance | Notes |
|-----------|------------|-------|
| I. CLI-First | ⚠️ Requires justification | Backend API должен иметь CLI для основных операций (создание задачи, просмотр, удаление) |
| II. Test-First | ✅ Planned | TDD для бизнес-логики, контрактные тесты для API |
| III. Incremental Delivery | ✅ Built-in | P1/P2/P3 приоритеты в spec |
| IV. Observability | ✅ Planned | Structured logging (Serilog), метрики |
| V. Simplicity | ✅ Planned | Минимальные зависимости, no MediatR |

**Complexity Justification** (CLI-First):
- Веб-интерфейс — основной UX для канбан-доски
- CLI будет для админских операций и скриптов
- Drag-n-drop требует GUI

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
│   ├── TaskTracker.Api/         # Web API проект
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   └── Program.cs
│   ├── TaskTracker.Application/ # CQRS handlers
│   │   ├── Common/
│   │   │   ├── Interfaces/
│   │   │   ├── Behaviors/
│   │   │   └── Validation/
│   │   ├── Features/
│   │   │   ├── Tasks/
│   │   │   │   ├── CreateTask/
│   │   │   │   ├── UpdateTask/
│   │   │   │   ├── DeleteTask/
│   │   │   │   └── GetTasksByDate/
│   │   │   └── Dates/
│   │   └── Application.csproj
│   ├── TaskTracker.Domain/      # Domain entities
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   └── Domain.csproj
│   ├── TaskTracker.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs
│   │   │   ├── Configurations/
│   │   │   └── Migrations/
│   │   ├── Validators/          # Custom IValidator
│   │   └── Infrastructure.csproj
│   └── TaskTracker.Cli/         # CLI для админских операций
│       ├── Commands/
│       └── Cli.csproj
├── tests/
│   ├── TaskTracker.UnitTests/
│   ├── TaskTracker.IntegrationTests/
│   └── TaskTracker.Cli.Tests/
├── Dockerfile
├── docker-compose.yml
└── appsettings.json

frontend/
├── src/
│   ├── components/
│   │   ├── Board/
│   │   │   ├── Board.tsx
│   │   │   ├── Column.tsx
│   │   │   └── TaskCard.tsx
│   │   ├── Header/
│   │   │   ├── DateNavigator.tsx
│   │   │   └── DatePicker.tsx
│   │   ├── TaskModal/
│   │   │   ├── CreateTaskModal.tsx
│   │   │   ├── EditTaskModal.tsx
│   │   │   └── DeleteConfirmModal.tsx
│   │   └── BulkActions/
│   ├── hooks/
│   │   ├── useTasks.ts
│   │   └── useDragDrop.ts
│   ├── stores/
│   │   └── taskStore.ts
│   ├── services/
│   │   └── api.ts
│   ├── types/
│   │   └── task.ts
│   ├── App.tsx
│   └── main.tsx
├── tests/
│   ├── unit/
│   └── integration/
├── package.json
├── vite.config.ts
└── index.html
```

**Structure Decision**: Web application с разделением frontend/backend. CLI проект для бекенд-операций (Constitution I. CLI-First).

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Backend + Frontend разделение | Требуется интерактивный UI для drag-n-drop | SPA без API не позволит масштабироваться |
| CQRS с кастомными handlers | Явное требование, упрощает тестирование | Прямой сервис-слой сложнее тестировать отдельно |
| Отдельный CLI проект | Constitution требует CLI-First | Встроенный CLI в Web API менее гибок |

## Phase 0: Research Tasks

- [ ] R001: Исследовать кастомную реализацию IValidator без FluentValidation
- [ ] R002: Найти best practices CQRS с кастомным IRequest/IRequestHandler
- [ ] R003: Исследовать React DnD библиотеки для drag-n-drop
- [ ] R004: Определить структуру API контрактов для задач
- [ ] R005: Исследовать настройку EF Core с PostgreSQL

## Phase 1: Design Artifacts

**После research.md**:
- `data-model.md`: Task, TaskStatus, DateRange entities
- `contracts/`: API endpoints schema, CLI commands
- `quickstart.md`: Запуск через docker-compose

## Constitution Re-Check

После Phase 1 проверить:
- CLI команды покрывают основные операции
- Тесты покрывают бизнес-логику
- Логирование структурировано
