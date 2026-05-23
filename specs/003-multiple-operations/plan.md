# План реализации: Массовые операции над задачами

**Ветка**: `feature/003-multiple-operations` | **Дата**: 2026-05-23 | **Спецификация**: [spec.md](./spec.md)

**Ввод**: Спецификация функции из `/specs/003-multiple-operations/spec.md`

## Сводка

Реализация массовых операций над задачами: удаление, изменение даты и перенос невыполненных задач на завтра. Большая часть инфраструктуры (выделение задач, bulk-удаление, bulk-перенос даты) уже реализована. Основная新增-функция — кнопка «Перенести на завтра», которая переносит все невыполненные задачи (status ≠ Done) на завтрашний день без подтверждения.

## Технический контекст

**Язык/Версия**: TypeScript 5.x (frontend), C# 12 / .NET 8 (backend)

**Основные зависимости**:
- Frontend: React 18, Zustand 4.5, @hello-pangea/dnd 18, date-fns 3.6, Axios 1.6, Vite 5
- Backend: ASP.NET Core 8, EF Core 8 (Npgsql), Scrutor, Swashbuckle

**Хранилище**: PostgreSQL 15 (через EF Core 8 + Npgsql)

**Тестирование**: Vitest + @testing-library/react (frontend), xUnit + Moq (backend unit), xUnit + WebApplicationFactory (backend integration)

**Целевая платформа**: Веб-приложение (браузер, десктоп + мобильный вид)

**Тип проекта**: Full-stack веб-приложение (React SPA + ASP.NET Core API)

**Цели производительности**: Массовые операции до 100 задач за <3 сек, UI-реакция <100мс

**Ограничения**: Нет подтверждений диалогами для «Перенести на завтра», выделение снимается после каждой операции

**Масштаб/объём**: Малый — 3 новых кнопки/действия, 1 новый API-эндпоинт, ~5 файлов на frontend, ~3 на backend

## Проверка конституции

*ВРАТА: Должна пройти перед исследованием Фазы 0. Повторная проверка после проектирования Фазы 1.*

**Статус**: Конституция проекта не заполнена (содержит шаблонные плейсхолдеры). Врата конституции не применимы — нет установленных принципов для проверки. Рекомендуется заполнить конституцию на более позднем этапе.

**Нарушения**: Нет (конституция не установлена).

## Структура проекта

### Документация (этой функции)

```text
specs/003-multiple-operations/
├── plan.md              # Этот файл (результат команды /speckit-plan)
├── research.md          # Результат Фазы 0 (команда /speckit-plan)
├── data-model.md        # Результат Фазы 1 (команда /speckit-plan)
├── quickstart.md        # Результат Фазы 1 (команда /speckit-plan)
├── contracts/           # Результат Фазы 1 (команда /speckit-plan)
│   └── api.md
├── checklists/
│   └── requirements.md  # Существующий чеклист требований
└── spec.md              # Спецификация (уже существует)
```

### Исходный код (корень репозитория)

```text
frontend/
├── src/
│   ├── components/
│   │   ├── Board.tsx                  # Канбан-доска (включает BulkActionsPanel)
│   │   ├── Header.tsx                 # Навигация по датам — ДОБАВИТЬ кнопку «Перенести на завтра»
│   │   ├── BulkActions/
│   │   │   └── BulkActionsPanel.tsx    # Панель массовых действий (УДАЛИТЬ window.confirm)
│   │   ├── Board/
│   │   │   └── TaskCheckbox.tsx        # Checkbox выделения задач
│   │   ├── TaskCard.tsx               # Карточка задачи
│   │   └── TaskModal/                 # Модальные окна
│   ├── stores/
│   │   └── taskStore.ts               # Zustand-стор — ДОБАВИТЬ moveIncompleteToTomorrow()
│   ├── services/
│   │   └── taskApi.ts                 # API-клиент — ДОБАВИТЬ moveIncompleteToTomorrow()
│   ├── types/
│   │   └── task.ts                    # Типы Task, TaskStatus
│   ├── hooks/
│   ├── utils/
│   │   └── date.ts                    # Утилиты дат
│   └── index.css                      # Стили — ДОБАВИТЬ стили для «Перенести на завтра»
├── tests/
│   ├── unit/
│   └── integration/
│       └── BulkActions.test.tsx        # Интеграционные тесты
└── package.json

backend/
├── src/
│   ├── TaskTracker.Api/
│   │   └── Controllers/
│   │       └── TasksController.cs      # REST API — ДОБАВИТЬ эндпоинт MoveIncompleteToTomorrow
│   ├── TaskTracker.Application/
│   │   └── Tasks/
│   │       ├── BulkDeleteCommand.cs    # Существующий
│   │       ├── BulkMoveCommand.cs       # Существующий
│   │       ├── MoveIncompleteToTomorrowCommand.cs  # НОВЫЙ — перенос невыполненных задач
│   │       └── ITaskRepository.cs       # Интерфейс репозитория — ДОБАВИТЬ метод
│   ├── TaskTracker.Domain/
│   │   ├── TaskEntity.cs              # Доменная сущность
│   │   └── TaskStatus.cs              # Enum: New=0, InProgress=1, Done=2
│   └── TaskTracker.Infrastructure/
│       └── Persistence/
│           ├── AppDbContext.cs
│           └── TaskRepository.cs       # Реализация — ДОБАВИТЬ метод
└── tests/
    ├── TaskTracker.UnitTests/
    └── TaskTracker.IntegrationTests/
```

**Решение по структуре**: Вариант 2 (веб-приложение). Frontend и backend — отдельные проекты. Frontend использует React+Zustand+Vite, backend — ASP.NET Core API с CQRS. Изменения минимальны: добавляется 1 новый command/handler на backend, 1 новый API-эндпоинт, 1 новый метод в репозитории, 1 кнопка в Header и 1 action в Zustand-сторе.

## Отслеживание сложности

> Нет нарушений конституции — таблица не требуется.