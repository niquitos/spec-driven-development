# План реализации: Рефакторинг — исправление потери данных и повышение качества

**Ветка**: `feature/005-refactoring` | **Дата**: 2026-05-28 | **Спецификация**: [spec.md](./spec.md)

**Ввод**: Спецификация функции из `/specs/005-refactoring/spec.md`

## Сводка

Исправление критических багов потери данных (пропадание свимлейнов), добавление PATCH-эндпоинта для частичного обновления задач, исправление SwimlaneCombobox (синхронизация props с состоянием), обработка сетевых ошибок при перетаскивании, и создание фронтенд-тестов для предотвращения регрессии.

## Технический контекст

**Язык/Версия**: C# 12 / .NET 8 (backend), TypeScript 5.x (frontend)

**Основные зависимости**: ASP.NET Core 8, EF Core 8 (Npgsql), React 18, Zustand 4.5, @hello-pangea/dnd, Vitest + @testing-library/react

**Хранилище**: PostgreSQL 15

**Тестирование**: xUnit (backend unit), IntegrationTestBase с Testcontainers (backend integration), Vitest + @testing-library/react (frontend)

**Целевая платформа**: Веб-приложение (браузер)

**Тип проекта**: Веб-приложение (frontend + backend)

**Цели производительности**: Перетаскивание задачи — отклик < 200мс (оптимистичный UI), PATCH-запрос — p95 < 300мс

**Ограничения**: Обратная совместимость PUT-эндпоинта, сохранение существующих тестов

**Масштаб/объём**: ~10K строк кода, 4 основных компонента для изменения

## Проверка конституции

| Принцип | Статус | Примечание |
|---------|--------|------------|
| I. TDD | ✅ Соответствует | Новые эндпоинты и исправления багов начинаются с тестов |
| II. DDD | ✅ Соответствует | Новые команды (PatchTaskCommand) следуют паттерну CQRS |
| III. SOLID | ✅ Соответствует | PATCH-хэндлер — новый класс, не модификация существующего |
| Технологические ограничения | ✅ Соответствует | .NET 8, React 18, Zustand — без новых зависимостей |
| Рабочий процесс | ✅ Соответствует | Feature-ветка, SDD-цикл, коммиты после каждого шага |

Нарушений конституции нет. Отслеживание сложности не требуется.

## Структура проекта

### Документация (этой функции)

```text
specs/005-refactoring/
├── plan.md              # Этот файл
├── spec.md              # Спецификация функции
├── research.md          # Исследование (Фаза 0)
├── data-model.md        # Модель данных (Фаза 1)
├── quickstart.md        # Краткое руководство (Фаза 1)
├── contracts/           # API-контракты (Фаза 1)
│   └── patch-task.md    # PATCH /api/tasks/{id}
└── checklists/
    └── requirements.md  # Чеклист требований
```

### Исходный код (корень репозитория)

```text
backend/
├── src/
│   ├── TaskTracker.Domain/
│   │   └── TaskEntity.cs              # Без изменений (существующая модель)
│   ├── TaskTracker.Application/
│   │   └── Tasks/
│   │       ├── PatchTaskCommand.cs    # НОВЫЙ: команда + хэндлер PATCH
│   │       └── UpdateTaskCommand.cs    # Без изменений (существующий PUT)
│   ├── TaskTracker.Api/
│   │   └── Controllers/
│   │       └── TasksController.cs     # ДОБАВЛЕНО: PATCH-эндпоинт
│   └── TaskTracker.Infrastructure/    # Без изменений
└── tests/
    ├── TaskTracker.UnitTests/
    │   └── Features/Tasks/
    │       └── PatchTask/             # НОВЫЙ: тесты PATCH-хэндлера
    └── TaskTracker.IntegrationTests/
        └── Tasks/
            └── PatchTaskTests.cs      # НОВЫЙ: интеграционные тесты PATCH

frontend/
├── src/
│   ├── api/
│   │   └── taskApi.ts                 # ДОБАВЛЕНО: patchTask()
│   ├── components/
│   │   ├── SwimlaneCombobox.tsx       # ИСПРАВЛЕНО: синхронизация value с props
│   │   └── TaskModal/
│   │       ├── EditTaskModal.tsx      # БЕЗ ИЗМЕНЕНИЙ (использует PUT)
│   │       └── CreateTaskModal.tsx    # БЕЗ ИЗМЕНЕНИЙ
│   └── stores/
│       └── taskStore.ts              # ИСПРАВЛЕНО: moveTask использует PATCH, rollback при ошибке
└── tests/
    ├── unit/
    │   ├── SwimlaneCombobox.test.tsx  # НОВЫЙ: тест синхронизации props
    │   └── taskStore.test.ts          # РАСШИРЕН: тесты moveTask с PATCH
    └── integration/
        └── DragDrop.test.tsx           # РАСШИРЕН: тесты сохранения свимлейна
```

**Решение по структуре**: Вариант 2 — веб-приложение (frontend + backend). Существующая структура проекта сохраняется. Новые файлы добавляются в соответствии с принятыми паттернами (CQRS-команда + хэндлер в Application, эндпоинт в Api Controller).