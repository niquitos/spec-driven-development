# Quickstart: Массовые операции над задачами

**Функция**: 003-multiple-operations | **Дата**: 2026-05-23

## Предварительные требования

- Node.js 18+
- .NET 8 SDK
- Docker (для PostgreSQL)
- Запущенный PostgreSQL через `docker-compose up db`

## Запуск backend

```bash
cd backend
dotnet run --project src/TaskTracker.Api
```

API доступен на `http://localhost:5000` (или порт из `launchSettings.json`).

## Запуск frontend

```bash
cd frontend
npm install
npm run dev
```

Приложение доступно на `http://localhost:5173`.

## Запуск тестов

### Backend

```bash
cd backend
dotnet test
```

### Frontend

```bash
cd frontend
npm run test
```

## Ключевые файлы для реализации

### Backend (создать/изменить)

1. **Создать**: `backend/src/TaskTracker.Application/Tasks/MoveIncompleteToTomorrowCommand.cs`
   - Command record (без параметров)
   - Response record: `MoveIncompleteToTomorrowResponse(int Moved, DateTime TargetDate)`
   - Handler: находит все задачи с `Status != Done`, обновляет `Date` на завтра, возвращает количество

2. **Изменить**: `backend/src/TaskTracker.Application/Tasks/ITaskRepository.cs`
   - Добавить: `Task<int> MoveIncompleteToTomorrowAsync(DateTime tomorrow, CancellationToken ct)`

3. **Изменить**: `backend/src/TaskTracker.Infrastructure/Persistence/TaskRepository.cs`
   - Реализовать метод через `ExecuteUpdateAsync`

4. **Изменить**: `backend/src/TaskTracker.Api/Controllers/TasksController.cs`
   - Добавить эндпоинт `POST /api/tasks/bulk/move-incomplete-to-tomorrow`
   - Инжектить `IRequestHandler<MoveIncompleteToTomorrowCommand, MoveIncompleteToTomorrowResponse>`

### Frontend (создать/изменить)

5. **Изменить**: `frontend/src/services/taskApi.ts`
   - Добавить: `moveIncompleteToTomorrow(): Promise<{ moved: number; targetDate: string }>`

6. **Изменить**: `frontend/src/stores/taskStore.ts`
   - Добавить action: `moveIncompleteToTomorrow(): Promise<void>`

7. **Изменить**: `frontend/src/components/Header.tsx`
   - Добавить кнопку «Перенести на завтра»

8. **Изменить**: `frontend/src/components/BulkActions/BulkActionsPanel.tsx`
   - Удалить `window.confirm()` из `handleBulkDelete`

9. **Изменить**: `frontend/src/index.css`
   - Добавить стили для кнопки «Перенести на завтра»

## Проверка

После реализации:

1. Запустить backend и frontend
2. Создать несколько задач на разных датах (New, InProgress, Done)
3. Нажать «Перенести на завтра» — все невыполненные задачи должны перенестись
4. Выделить несколько задач, нажать «Удалить» — задачи удаляются без подтверждения
5. Выделить несколько задач, нажать «Переместить» — выбрать дату, задачи переносятся