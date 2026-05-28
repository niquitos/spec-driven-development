# Краткое руководство: Рефакторинг — исправление потери данных

**Дата**: 2026-05-28 | **Ветка**: `feature/005-refactoring`

## Быстрый старт

### Запуск проекта

```bash
# Backend (из корня репозитория)
cd backend
dotnet run --project src/TaskTracker.Api

# Frontend (из корня репозитория)
cd frontend
npm install  # только при первой установке
npm run dev
```

### Запуск тестов

```bash
# Backend unit-тесты
cd backend
dotnet test tests/TaskTracker.UnitTests

# Backend интеграционные тесты (требуется Docker для Testcontainers)
cd backend
dotnet test tests/TaskTracker.IntegrationTests

# Frontend тесты
cd frontend
npm run test
```

## Ключевые изменения

### 1. PATCH-эндпоинт (backend)

Новый эндпоинт для частичного обновления задач. Следует CQRS-паттерну проекта.

**Файлы для создания:**
- `backend/src/TaskTracker.Application/Tasks/PatchTaskCommand.cs` — command record + handler
- `backend/src/TaskTracker.Application/Validators/PatchTaskCommandValidator.cs` — валидация
- Обновить `TasksController.cs` — добавить `[HttpPatch("{id}")]`

**Семантика:** отсутствующее поле = не изменять, `null`/`""` для строки = очистить.

### 2. Исправление moveTask (frontend)

Заменить PUT на PATCH в методе `moveTask()` хранилища Zustand.

**Файлы для изменения:**
- `frontend/src/api/taskApi.ts` — добавить `patchTask()`
- `frontend/src/stores/taskStore.ts` — исправить `moveTask()`: использовать `patchTask` вместо `updateTask`, добавить swimlane, добавить rollback при ошибке

### 3. Исправление SwimlaneCombobox (frontend)

Добавить `useEffect` для синхронизации `inputValue` с `value` prop.

**Файлы для изменения:**
- `frontend/src/components/SwimlaneCombobox.tsx` — добавить `useEffect(() => { setInputValue(value); }, [value])`
- `frontend/src/components/AssigneeCombobox.tsx` — аналогичное исправление

### 4. Фронтенд-тесты

**Файлы для создания:**
- `frontend/tests/unit/SwimlaneCombobox.test.tsx` — тест синхронизации props
- `frontend/tests/unit/PatchTaskApi.test.ts` — тест нового API-метода

**Файлы для расширения:**
- `frontend/tests/unit/taskStore.test.ts` — добавить тесты moveTask с PATCH и rollback
- `frontend/tests/integration/DragDrop.test.tsx` — добавить тест сохранения свимлейна

### 5. Backend-тесты

**Файлы для создания:**
- `backend/tests/TaskTracker.UnitTests/Features/Tasks/PatchTask/` — handler tests, validation tests, contract tests
- `backend/tests/TaskTracker.IntegrationTests/Tasks/PatchTaskTests.cs` — endpoint integration tests

## Порядок реализации (TDD)

1. **Backend PATCH**: Тесты → Command + Validator → Handler → Controller
2. **Frontend API**: Тест → `patchTask()` в taskApi
3. **Frontend Store**: Тест → исправление `moveTask()` (PATCH + swimlane + rollback)
4. **Frontend Combobox**: Тест → исправление синхронизации props
5. **Integration-тесты**: DragDrop со свимлейном, PATCH через HTTP