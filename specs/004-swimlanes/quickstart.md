# Quickstart: Swimlane-группировка задач на доске

**Функция**: 004-swimlanes | **Дата**: 2026-05-25

## Предварительные требования

- .NET 8 SDK
- Node.js 18+
- Docker + Docker Compose (для PostgreSQL)
- Запущенный PostgreSQL (через `docker-compose up -d` из корня проекта)

## Запуск проекта

```bash
# Backend
cd backend
dotnet restore
dotnet ef database update --project src/TaskTracker.Infrastructure --startup-project src/TaskTracker.Api
dotnet run --project src/TaskTracker.Api

# Frontend (отдельный терминал)
cd frontend
npm install
npm run dev
```

## Порядок реализации (TDD)

### Шаг 1: Backend — Domain

1. Добавить `string? Swimlane` в `TaskEntity.cs`
2. Обновить `AppDbContext.cs`: `Property(e => e.Swimlane).HasMaxLength(100)`, добавить индекс `IX_tasks_Date_Swimlane`
3. Создать миграцию `AddSwimlaneToTask`

### Шаг 2: Backend — Application (TDD)

4. **Тест** → `GetSwimlanesQueryHandlerTests.cs` — падающий тест на получение уникальных swimlane (включая case-insensitive группировку и сортировку)
5. **Реализация** → `GetSwimlanesQuery.cs` — query, handler, validator
6. Добавить `GetSwimlanesAsync` в `ITaskRepository`
7. Реализовать `GetSwimlanesAsync` в `TaskRepository` (`.GroupBy(t => t.Swimlane!.ToLower())`, `.OrderBy()` с «без категории» первым)
8. Расширить `CreateTaskCommand` полем `Swimlane` (maxlength 100, null-нормализация), обновить handler и validator
9. Расширить `UpdateTaskCommand` полем `Swimlane` (maxlength 100, null-нормализация), обновить handler и validator
10. **Тесты** → `CreateTaskCommandHandlerTests.cs` и `UpdateTaskCommandHandlerTests.cs`:
    - Создание задачи с swimlane
    - Создание задачи без swimlane (null)
    - Пробельные строки нормализуются в null
    - Превышение maxlength → ошибка валидации
    - Case-insensitive: создание с «фронтенд» при существующем «Фронтенд»
11. Расширить `GetTasksQuery` для фильтрации по swimlane (параметр `swimlanes`)

### Шаг 3: Backend — API (TDD)

12. **Тест** → `SwimlaneEndpointTests.cs`:
    - `GET /api/tasks/swimlanes?date=...` — возвращает уникальные swimlane
    - `GET /api/tasks/swimlanes` без date → 400 Bad Request
    - `GET /api/tasks/swimlanes?date=...` — пустой массив при отсутствии swimlane
    - `POST /api/tasks` с swimlane → 201 Created
    - `POST /api/tasks` с swimlane > 100 символов → 400 Bad Request
    - `PUT /api/tasks/{id}` с swimlane → 204 No Content
    - `PUT /api/tasks/{id}` с swimlane=null → очищает поле
    - `GET /api/tasks?date=...&swimlanes=Фронтенд` — фильтрация по swimlane
    - `GET /api/tasks?date=...&assignees=Иван&swimlanes=Фронтенд` — комбинированный фильтр (И)
13. Добавить `GET /api/tasks/swimlanes` в `TasksController`
14. Расширить `GET /api/tasks` — параметр `swimlanes`
15. Расширить `POST /api/tasks` и `PUT /api/tasks/{id}` — поле `Swimlane` в request records
16. Применить миграцию к БД
17. Запустить все тесты: `dotnet test`

### Шаг 4: Frontend — типы и API

18. Добавить `swimlane: string | null` в `Task`, `swimlane?: string` в DTO (types/task.ts)
19. Создать `utils/swimlane.ts` с `normalizeSwimlaneKey()`, `DEFAULT_SWIMLANE_KEY`, `DEFAULT_SWIMLANE_DISPLAY`
20. Добавить `getSwimlanes()` в `taskApi.ts`
21. Расширить `getTasks()` для параметра `swimlanes`

### Шаг 5: Frontend — Store

22. Добавить `swimlaneList`, `collapsedSwimlanes`, `loadSwimlaneList`, `toggleSwimlaneCollapse` в `taskStore.ts`
23. Обновить `createTask`, `updateTask` — передавать swimlane
24. Обновить `loadTasks` — загружать swimlaneList параллельно
25. Обновить `moveTask` — обрабатывать изменение swimlane при drag-and-drop

### Шаг 6: Frontend — Компоненты (TDD)

26. **Тест** → `SwimlaneCombobox` — рендеринг, автодополнение, выбор, maxlength, ARIA-атрибуты
27. Создать `SwimlaneCombobox.tsx` (по аналогии с `AssigneeCombobox`, другие ARIA-роли и placeholder)
28. **Тест** → `SwimlaneHeader` — отображение имени, количества задач, collapse toggle, ARIA-атрибуты
29. Создать `SwimlaneHeader.tsx` — заголовок swimlane + количество задач + кнопка сворачивания с ARIA
30. **Тест** → `SwimlaneRow` — отображение задач в свёрнутом/развёрнутом состоянии, пустые ячейки
31. Создать `SwimlaneRow.tsx` — горизонтальная полоса с колонками (включая пустые ячейки как Droppable)
32. Создать `useSwimlaneCollapse.ts` — hook для localStorage persistence (ключ: нормализованные lowercase-ключи)

### Шаг 7: Frontend — Board refactor

33. Рефакторинг `Board.tsx` — группировка задач по swimlane (матрица swimlane × статус)
34. Обновить `Column.tsx` — droppableId с составным ключом `{normalizeSwimlaneKey(swimlane)}:{TaskStatus}`
35. Обновить `TaskCard.tsx` — передача swimlane в drag data
36. Обновить drag-and-drop handler — парсинг составного droppableId, обновление swimlane при вертикальном перемещении, обработка «без категории» → null

### Шаг 8: Frontend — Модальные окна

37. Добавить поле swimlane в `CreateTaskModal.tsx` (SwimlaneCombobox, опциональное, по умолчанию null → «Без категории»)
38. Добавить поле swimlane в `EditTaskModal.tsx` (SwimlaneCombobox, текущее значение)

### Шаг 9: Frontend — Стили

39. Добавить стили для swimlane в `index.css`:
    - Горизонтальная полоса (строка матрицы)
    - Заголовок swimlane с количеством задач и кнопкой collapse
    - Анимация сворачивания/разворачивания (CSS transition, 200мс)
    - Пустые ячейки в матрице (Droppable-области)
    - Свёрнутый swimlane (узкая полоса, только заголовок)
    - `prefers-reduced-motion: reduce` — отключение анимации

### Шаг 10: Frontend — Accessibility

40. Добавить ARIA-атрибуты:
    - `role="button"`, `aria-expanded`, `aria-controls`, `aria-label` на кнопке toggle
    - `aria-hidden="true"` на свёрнутом содержимом swimlane
41. Клавиатурная навигация: `Enter`/`Space` для toggle, Tab-порядок через swimlane

### Шаг 11: Проверка

42. Запустить все backend-тесты: `dotnet test`
43. Запустить все frontend-тесты: `npm run test`
44. Ручное тестирование:
    - Создать задачи с разными swimlane — проверить группировку
    - Свернуть/развернуть swimlane — проверить сохранение в localStorage
    - Перетащить задачу между swimlane — проверить обновление поля
    - Перетащить задачу в свёрнутый swimlane — проверить, что swimlane не разворачивается
    - Перетащить последнюю задачу из swimlane — проверить, что swimlane исчезает
    - Ввести swimlane с разным регистром — проверить case-insensitive группировку
    - Ввести swimlane > 100 символов — проверить валидацию
    - Применить фильтр по assignee — проверить ортогональность с swimlane
    - Проверить ARIA-атрибуты и клавиатурную навигацию

## Команды для тестирования

```bash
# Backend
cd backend
dotnet test

# Frontend
cd frontend
npm run test
```

## Ключевые файлы для проверки

| Файл | Что проверить |
|------|---------------|
| `TaskEntity.cs` | Добавлено свойство `Swimlane` |
| `AppDbContext.cs` | Конфигурация Swimlane (maxlength 100) + индекс |
| `GetSwimlanesQuery.cs` | Новый query + handler (case-insensitive, сортировка) |
| `TaskRepository.cs` | Метод `GetSwimlanesAsync` |
| `CreateTaskCommand.cs` | Поле Swimlane, validator (maxlength 100, null-нормализация) |
| `UpdateTaskCommand.cs` | Поле Swimlane, validator (maxlength 100, null-нормализация) |
| `TasksController.cs` | Эндпоинт `GET /api/tasks/swimlanes`, параметр `swimlanes` в GET tasks |
| `utils/swimlane.ts` | Функция `normalizeSwimlaneKey`, константы |
| `Board.tsx` | Группировка по swimlane, матрица swimlane × статус |
| `SwimlaneRow.tsx` | Горизонтальная полоса, пустые ячейки, Droppable |
| `SwimlaneHeader.tsx` | Заголовок + количество задач + collapse toggle (ARIA) |
| `SwimlaneCombobox.tsx` | Автодополнение swimlane, maxlength, ARIA |
| `useSwimlaneCollapse.ts` | localStorage persistence (нормализованные ключи) |
| `taskStore.ts` | Новые поля и действия для swimlane |
| `index.css` | Стили swimlane, анимация, reduced-motion |