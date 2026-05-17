# Implementation Plan: Назначение исполнителей задач

**Branch**: `002-task-assignee` | **Date**: 2026-05-17 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/002-task-assignee/spec.md`

## Summary

Добавление поля "исполнитель" (assignee) к задачам. Исполнитель — опциональная строка. Назначение через комбобокс при создании/редактировании задачи. Фильтрация по исполнителям через мультиселект с хранением состояния в URL query-параметрах. Список доступных исполнителей формируется из всех существующих задач.

## Technical Context

**Language/Version**: 
- Frontend: TypeScript 5.x, React 18.x
- Backend: C# 12, .NET 8

**Primary Dependencies**:
- Frontend: React, Zustand (state), @hello-pangea/dnd, date-fns
- Backend: ASP.NET Core Web API, EF Core 8, Npgsql

**Storage**: PostgreSQL 15+ через EF Core, новая миграция для поля Assignee

**Testing**:
- Frontend: Jest + React Testing Library
- Backend: xUnit + Moq + Testcontainers

**Target Platform**: Web-приложение

**Project Type**: Web application (frontend + backend API)

**Performance Goals**: Фильтрация и назначение исполнителя — мгновенный отклик (< 500 мс)

**Constraints**:
- Фильтр хранится в query-параметрах URL (assignees=...), переживает перезагрузку и переключение дат
- Single-user, без аутентификации
- Исполнитель — строка, не привязан к пользователю системы

**Scale/Scope**:
- P1: Назначение исполнителя + просмотр на карточке
- P2: Фильтрация по исполнителям

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Compliance | Notes |
|-----------|------------|-------|
| I. Test-First | ✅ Implemented | TDD для всей новой бизнес-логики |
| II. Incremental Delivery | ✅ Implemented | P1 (назначение/просмотр) → P2 (фильтрация) — независимо тестируемы |
| III. Observability | ✅ Implemented | ILogger используется во всех handlers |
| IV. Simplicity | ✅ Implemented | Assignee — простая строка, без отдельной таблицы/сущности |

## Project Structure

### Documentation (this feature)

```text
specs/002-task-assignee/
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
│   ├── TaskTracker.Api/
│   │   └── Controllers/
│   │       └── TasksController.cs          # + assignees query param
│   ├── TaskTracker.Application/
│   │   └── Tasks/
│   │       ├── GetTasksQuery.cs             # + Assignees filter
│   │       ├── CreateTaskCommand.cs         # + Assignee
│   │       └── UpdateTaskCommand.cs         # + Assignee
│   ├── TaskTracker.Domain/
│   │   └── TaskEntity.cs                   # + Assignee (string?)
│   └── TaskTracker.Infrastructure/
│       ├── Persistence/
│       │   ├── AppDbContext.cs
│       │   └── Migrations/                 # Новая миграция
│       └── Validators/
│           └── CreateTaskValidator.cs       # Валидация assignee

frontend/
├── src/
│   ├── components/
│   │   ├── TaskCard.tsx                    # + Отображение assignee
│   │   ├── Header.tsx                     # + AssigneeFilter
│   │   ├── AssigneeFilter.tsx             # Новый компонент фильтра
│   │   ├── AssigneeCombobox.tsx           # Новый компонент комбобокса
│   │   └── TaskModal/
│   │       ├── CreateTaskModal.tsx         # + Assignee поле
│   │       └── EditTaskModal.tsx           # + Assignee поле
│   ├── stores/
│   │   └── taskStore.ts                   # + assigneeFilter, assigneeList
│   ├── services/
│   │   └── taskApi.ts                     # + assignees params
│   ├── types/
│   │   └── task.ts                        # + assignee поле
│   └── hooks/
│       └── useAssigneeFilter.ts           # Новый хук для URL sync
```

**Structure Decision**: Та же структура frontend/backend. Нет новых проектов/пакетов.

## Complexity Tracking

> Нет нарушений Constitution — Complexity Tracking не требуется.

## Design Decisions

1. **Assignee как строка на TaskEntity** — без отдельной таблицы. Список уникальных исполнителей формируется через SELECT DISTINCT из задач. Это соответствует принципу Simplicity (YAGNI) — отдельная сущность понадобится только если появятся атрибуты исполнителя (цвет, контакты и т.д.).
2. **Комбобокс через datalist** — нативный HTML5-элемент `datalist` для простоты. Если потребуется кастомизация — заменить на кастомный компонент.
3. **Фильтр в query-параметрах** — `?assignees=Иван,Петр` в URL. Переживает перезагрузку и переключение дат.
4. **API фильтрации** — GET /api/tasks?date=...&assignees=Иван,Петр — бекенд фильтрует по списку.
5. **Валидация assignee** — не пустая строка, не только пробелы, макс 100 символов. Регистронезависимое сравнение.

## Implementation Status

- ❌ Backend: Добавить Assignee в TaskEntity
- ❌ Backend: Добавить Assignee в CreateTaskCommand / UpdateTaskCommand
- ❌ Backend: Добавить assignees filter в GetTasksQuery
- ❌ Backend: Новая миграция EF Core
- ❌ Backend: Валидация assignee
- ❌ Backend: Тесты (unit + integration)
- ❌ Frontend: Добавить assignee в Task type + DTOs
- ❌ Frontend: AssigneeCombobox компонент
- ❌ Frontend: Assignee в CreateTaskModal / EditTaskModal
- ❌ Frontend: Отображение assignee на TaskCard
- ❌ Frontend: AssigneeFilter компонент
- ❌ Frontend: Фильтр в URL query-параметрах
- ❌ Frontend: Тесты

## Constitution Re-Check

После реализации проверить:
- ✅ Тесты покрывают логику фильтрации и назначения — TDD обязательно
- ✅ Никакой лишней сложности — assignee просто строка
- ✅ Фильтр через query-параметры — без дополнительного хранилища состояния
