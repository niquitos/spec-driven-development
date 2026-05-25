# Задачи: Swimlane-группировка задач на доске

**Ввод**: Проектные документы из `/specs/004-swimlanes/`

**Предварительные требования**: plan.md (обязательно), spec.md (обязательно), research.md, data-model.md, contracts/swimlane-api.md, quickstart.md

**Тесты**: Конституция требует TDD (Red-Green-Refactor). Тестовые задачи включены для каждого уровня (unit, integration, component).

**Организация**: Задачи сгруппированы по пользовательским историям для обеспечения независимой реализации и тестирования каждой истории.

## Формат: `[ID] [P?] [История] Описание`

- **[P]**: Может выполняться параллельно (разные файлы, нет зависимостей)
- **[История]**: К какой пользовательской истории относится задача (US1, US2, US3, US4)
- Включены точные пути к файлам

## Соглашения о путях

- **Backend**: `backend/src/TaskTracker.{Layer}/` и `backend/tests/`
- **Frontend**: `frontend/src/`
- Файлы тестов: `backend/tests/TaskTracker.UnitTests/Tasks/` и `backend/tests/TaskTracker.IntegrationTests/`

---

## Фаза 1: Настройка (Общая инфраструктура)

**Цель**: Добавить поле Swimlane к доменной модели и создать миграцию

- [ ] T001 Добавить свойство `string? Swimlane` в `backend/src/TaskTracker.Domain/TaskEntity.cs`
- [ ] T002 Настроить Swimlane в `backend/src/TaskTracker.Infrastructure/Persistence/AppDbContext.cs`: `Property(e => e.Swimlane).HasMaxLength(100)`, добавить индекс `IX_tasks_Date_Swimlane` на `(Date, Swimlane)`
- [ ] T003 Создать EF Core миграцию `AddSwimlaneToTask` через `dotnet ef migrations add`

**Контрольная точка**: Миграция применена, колонка Swimlane существует в БД

---

## Фаза 2: Фундамент (Backend API для swimlane)

**Цель**: Backend полностью поддерживает поле Swimlane — все CRUD-операции и запрос уникальных значений

**⚠️ КРИТИЧНО**: Frontend-задачи не могут начаться, пока эта фаза не завершена

### Тесты (TDD — написать первыми)

- [ ] T004 [P] Unit-тест: `GetSwimlanesQueryHandlerTests.cs` в `backend/tests/TaskTracker.UnitTests/Tasks/` — получение уникальных swimlane, case-insensitive группировка, сортировка («Без категории» не входит, алфавитный порядок)
- [ ] T005 [P] Unit-тест: расширить `CreateTaskCommandHandlerTests.cs` — создание задачи с swimlane, без swimlane, пробельные строки → null, maxlength > 100 → ошибка
- [ ] T006 [P] Unit-тест: расширить `UpdateTaskCommandHandlerTests.cs` — обновление swimlane, очистка swimlane (null), пробельные строки → null, maxlength > 100 → ошибка
- [ ] T007 [P] Integration-тест: `SwimlaneEndpointTests.cs` в `backend/tests/TaskTracker.IntegrationTests/` — GET /api/tasks/swimlanes (успех, пустой результат, 400 без date), POST/PUT с swimlane, фильтрация по swimlanes

### Реализация

- [ ] T008 Создать `GetSwimlanesQuery.cs` в `backend/src/TaskTracker.Application/Tasks/` — query record, handler с `.GroupBy(t => t.Swimlane!.ToLower()).Select(g => g.First().Swimlane!).OrderBy(...)`, validator
- [ ] T009 Добавить `Task<string[]> GetSwimlanesAsync(CancellationToken ct)` в `backend/src/TaskTracker.Application/Tasks/ITaskRepository.cs`
- [ ] T010 Реализовать `GetSwimlanesAsync` в `backend/src/TaskTracker.Infrastructure/Persistence/TaskRepository.cs` — case-insensitive группировка, сортировка («без категории» не входит в список)
- [ ] T011 Расширить `CreateTaskCommand.cs` в `backend/src/TaskTracker.Application/Tasks/` — добавить `string? Swimlane = null`, валидация maxlength 100, null-нормализация в handler
- [ ] T012 Расширить `UpdateTaskCommand.cs` в `backend/src/TaskTracker.Application/Tasks/` — добавить `string? Swimlane = null`, валидация maxlength 100, null-нормализация в handler
- [ ] T013 Расширить `GetTasksQuery.cs` handler в `backend/src/TaskTracker.Application/Tasks/` — поддержка параметра `swimlanes` для фильтрации (case-insensitive, комбинирование с assignees через AND)
- [ ] T014 Расширить `TaskRepository.cs` — добавить параметр `swimlanes` в `GetByDateAsync`, фильтрация `.Where(t => swimlanes.Contains(t.Swimlane.ToLower()))`
- [ ] T015 Добавить `GET /api/tasks/swimlanes?date=` в `backend/src/TaskTracker.Api/Controllers/TasksController.cs` — инжекция `IRequestHandler<GetSwimlanesQuery, string[]>`
- [ ] T016 Расширить `GET /api/tasks` в `TasksController.cs` — параметр `swimlanes` (comma-separated)
- [ ] T017 Расширить `CreateTaskRequest` и `UpdateTaskRequest` в `TasksController.cs` — добавить `string? Swimlane`

**Контрольная точка**: `dotnet test` проходит, Swagger показывает поле swimlane во всех endpoints

---

## Фаза 3: Пользовательская история 1 — Просмотр задач в swimlane на доске (Приоритет: P1) 🎯 MVP

**Цель**: Пользователь открывает доску и видит задачи, сгруппированные по горизонтальным swimlane, с swimlane «Без категории» для задач без группы

**Независимый тест**: Открыть доску → задачи распределены по swimlane → задачи без swimlane в «Без категории» → swimlane протягивается через все колонки

### Тесты для US1

- [ ] T018 [P] [US1] Component-тест: `SwimlaneRow.test.tsx` — рендеринг горизонтальной полосы с задачами, пустые ячейки как Droppable
- [ ] T019 [P] [US1] Component-тест: `SwimlaneHeader.test.tsx` — отображение названия swimlane и количества задач

### Реализация US1

- [ ] T020 [P] [US1] Добавить `swimlane: string | null` в `frontend/src/types/task.ts` (Task, CreateTaskDto, UpdateTaskDto)
- [ ] T021 [P] [US1] Создать `frontend/src/utils/swimlane.ts` — `normalizeSwimlaneKey()`, `DEFAULT_SWIMLANE_KEY`, `DEFAULT_SWIMLANE_DISPLAY`, `groupBySwimlane()`
- [ ] T022 [P] [US1] Добавить `getSwimlanes(date: string)` в `frontend/src/services/taskApi.ts`, расширить `getTasks` параметром `swimlanes`
- [ ] T023 [US1] Расширить `frontend/src/stores/taskStore.ts` — добавить `swimlaneList: string[]`, `loadSwimlaneList()`, вызов `loadSwimlaneList` после мутаций (create, update, delete, move)
- [ ] T024 [US1] Создать `frontend/src/components/SwimlaneRow.tsx` — горизонтальная полоса с тремя Column-компонентами (New, InProgress, Done), пустые ячейки как Droppable-области, каждая ячейка с droppableId `{normalizeSwimlaneKey(swimlane)}:{TaskStatus}`
- [ ] T025 [US1] Создать `frontend/src/components/SwimlaneHeader.tsx` — заголовок swimlane с названием (displayName), количеством задач (общее по всем колонкам), и placeholder для collapse toggle
- [ ] T026 [US1] Рефакторинг `frontend/src/components/Board.tsx` — заменить прямую отрисовку Column на группировку по swimlane: загрузка swimlaneList, группировка задач через `groupBySwimlane()`, отрисовка SwimlaneRow для каждой группы (сначала «Без категории», потом по алфавиту)
- [ ] T027 [US1] Адаптировать `frontend/src/components/Column.tsx` — принимать `swimlaneKey` как prop, формировать droppableId как `{swimlaneKey}:{status}`
- [ ] T028 [US1] Адаптировать `frontend/src/components/TaskCard.tsx` — передавать `swimlaneKey` в drag data для обновления swimlane при DnD (подготовка для US4)

**Контрольная точка**: Доска отображает задачи по swimlane, «Без категории» первый, остальные по алфавиту, пустые ячейки видны

---

## Фаза 4: Пользовательская история 2 — Сворачивание и разворачивание swimlane (Приоритет: P2)

**Цель**: Пользователь может сворачивать/разворачивать swimlane, состояние сохраняется в localStorage

**Независимый тест**: Свернуть swimlane → задачи скрываются, виден заголовок → развернуть → задачи появляются → обновить страницу → состояние сохраняется

### Тесты для US2

- [ ] T029 [P] [US2] Unit-тест: `useSwimlaneCollapse.test.ts` — чтение/запись localStorage, toggle, нормализованные ключи

### Реализация US2

- [ ] T030 [US2] Создать `frontend/src/hooks/useSwimlaneCollapse.ts` — хук для управления collapsedSwimlanes (Set<string> нормализованных ключей), чтение/запись localStorage по ключу `tasktracker_collapsed_swimlanes`, функции `isCollapsed(key)`, `toggle(key)`, `collapseAll()`, `expandAll()`
- [ ] T031 [US2] Расширить `frontend/src/stores/taskStore.ts` — добавить `collapsedSwimlanes: Set<string>`, `toggleSwimlaneCollapse(swimlaneKey: string)`, интеграция с `useSwimlaneCollapse`
- [ ] T032 [US2] Обновить `frontend/src/components/SwimlaneHeader.tsx` — добавить кнопку сворачивания с ARIA-атрибутами (`role="button"`, `aria-expanded`, `aria-controls`, `aria-label`), обработка `Enter`/`Space` для клавиатурной навигации
- [ ] T033 [US2] Обновить `frontend/src/components/SwimlaneRow.tsx` — условный рендеринг: развёрнутый (задачи видны) или свёрнутый (только SwimlaneHeader с количеством задач), скрытые задачи с `aria-hidden="true"`
- [ ] T034 [US2] Добавить стили сворачивания/разворачивания в `frontend/src/index.css` — анимация CSS transition на `max-height` и `opacity` (200мс), медиа-запрос `prefers-reduced-motion: reduce` для мгновенного переключения

**Контрольная точка**: Swimlane сворачивается/разворачивается, состояние сохраняется в localStorage, ARIA-атрибуты работают

---

## Фаза 5: Пользовательская история 3 — Назначение swimlane задаче через поле (Приоритет: P2)

**Цель**: Пользователь может задать/изменить swimlane через поле с автодополнением при создании/редактировании задачи

**Независимый тест**: Открыть EditTaskModal → ввести swimlane → задача перемещается → ввести новое значение → создаётся новый swimlane → очистить поле → задача в «Без категории»

### Тесты для US3

- [ ] T035 [P] [US3] Component-тест: `SwimlaneCombobox.test.tsx` — рендеринг, автодополнение (case-insensitive substring), выбор из списка, ввод нового значения, maxlength 100, ARIA-атрибуты

### Реализация US3

- [ ] T036 [US3] Создать `frontend/src/components/SwimlaneCombobox.tsx` — автодополнение по аналогии с AssigneeCombobox: props `value`, `options: string[]`, `onChange`, `placeholder="Выберите swimlane..."`, ARIA-атрибуты (`role="combobox"`, `aria-expanded`, `aria-controls="swimlane-listbox"`), case-insensitive substring фильтрация через `.toLowerCase()`, maxlength 100, free-text ввод
- [ ] T037 [US3] Расширить `frontend/src/components/TaskModal/CreateTaskModal.tsx` — добавить SwimlaneCombobox с `value=""` (по умолчанию), `options={swimlaneList}`, `onChange={handleSwimlaneChange}`
- [ ] T038 [US3] Расширить `frontend/src/components/TaskModal/EditTaskModal.tsx` — добавить SwimlaneCombobox с `value={task.swimlane ?? ""}`, `options={swimlaneList}`, `onChange={handleSwimlaneChange}`, возможность очистить (null → «Без категории»)
- [ ] T039 [US3] Обновить `frontend/src/stores/taskStore.ts` — расширить `createTask` и `updateTask` для передачи поля `swimlane`, вызов `loadSwimlaneList()` после мутаций (обновление списка swimlane)

**Контрольная точка**: Поле swimlane с автодополнением работает в CreateTaskModal и EditTaskModal, новые swimlane создаются автоматически

---

## Фаза 6: Пользовательская история 4 — Drag-and-drop задач между swimlane (Приоритет: P3)

**Цель**: Пользователь может перетащить задачу между swimlane (вертикально) и между колонками (горизонтально), поле swimlane обновляется автоматически

**Независимый тест**: Перетащить задачу из swimlane «Фронтенд» в «Бэкенд» → поле swimlane обновилось → перетащить из «Без категории» в именованный → swimlane задан → горизонтальное перемещение в рамках swimlane → только статус меняется

### Тесты для US4

- [ ] T040 [P] [US4] Unit-тест: расширить `taskStore.test.ts` — `moveTask` с изменением swimlane (вертикальное перемещение), `moveTask` без изменения swimlane (горизонтальное перемещение), обработка «без категории» → null
- [ ] T041 [P] [US4] Component-тест: `Board.swimlane.test.tsx` — парсинг составного droppableId, drag-and-drop между swimlane, drag-and-drop в свёрнутый swimlane

### Реализация US4

- [ ] T042 [US4] Обновить drag-and-drop handler в `frontend/src/components/Board.tsx` — парсинг составного droppableId `{swimlaneKey}:{status}`, определение нового swimlaneKey и status из destination, обработка «без категории» → null
- [ ] T043 [US4] Обновить `frontend/src/stores/taskStore.ts` — расширить `moveTask` для обновления swimlane при вертикальном перемещении: если `swimlaneKey` изменился → отправить `updateTask({ swimlane: displayName })`, если `swimlaneKey === DEFAULT_SWIMLANE_KEY` → отправить `swimlane: null`
- [ ] T044 [US4] Обновить `frontend/src/components/SwimlaneRow.tsx` — свёрнутый swimlane принимает drag-and-drop (задача добавляется, swimlane не разворачивается, счётчик обновляется), пустые ячейки — Droppable-области
- [ ] T045 [US4] Реализовать optimistic update для drag-and-drop: задача мгновенно перемещается в целевой swimlane, при ошибке API — откат (rollback), исходный swimlane восстанавливается

**Контрольная точка**: Drag-and-drop работает в обоих направлениях, поле swimlane обновляется, optimistic update корректен

---

## Фаза 7: Доработка и сквозные задачи

**Цель**: Интеграция с bulk-операциями, accessibility, валидация, стили

- [ ] T046 [P] Unit-тесты: расширить `BulkDeleteCommandHandlerTests.cs`, `BulkMoveCommandHandlerTests.cs`, `MoveIncompleteToTomorrowCommandHandlerTests.cs` в `backend/tests/TaskTracker.UnitTests/Tasks/` — подтвердить, что поле swimlane сохраняется при bulk-операциях (не очищается, не переназначается)
- [ ] T047 [P] Добавить стили для swimlane в `frontend/src/index.css` — визуальная модель (матрица swimlane × статус), заголовок с количеством, свёрнутый/развёрнутый вид, стили для пустых ячеек, обрезка длинных названий swimlane (text-overflow: ellipsis, title при наведении)
- [ ] T048 [P] Добавить валидацию maxlength 100 на frontend — `maxLength={100}` в SwimlaneCombobox input, trim + empty → null перед отправкой
- [ ] T049 [P] Обновить `frontend/src/stores/taskStore.ts` — убедиться что `loadSwimlaneList` вызывается после каждой мутации (create, update, delete, move, bulkDelete, bulkMove, moveIncompleteToTomorrow). Расширить `GetTaskByIdQuery` для включения поля `swimlane` в ответ
- [ ] T050 Валидация quickstart.md — пройти все шаги от TDD до ручного тестирования, убедиться что `dotnet test` и `npm run test` проходят

---

## Зависимости и порядок выполнения

### Зависимости фаз

- **Настройка (Фаза 1)**: Нет зависимостей — можно начать немедленно
- **Фундамент (Фаза 2)**: Зависит от завершения Фазы 1 — БЛОКИРУЕТ все пользовательские истории
- **US1 (Фаза 3)**: Зависит от Фазы 2 — можно начать после завершения backend API
- **US2 (Фаза 4)**: Зависит от US1 (нужен SwimlaneRow и SwimlaneHeader) — расширяет существующие компоненты
- **US3 (Фаза 5)**: Зависит от US1 (нужен swimlaneList в store) — может разрабатываться параллельно с US2
- **US4 (Фаза 6)**: Зависит от US1 (нужен Board с составными droppableId) — может разрабатываться после US1
- **Доработка (Фаза 7)**: Зависит от завершения всех желаемых пользовательских историй

### Зависимости пользовательских историй

- **US1 (P1)**: Может начаться после Фундамента — нет зависимостей от других историй
- **US2 (P2)**: Зависит от US1 (нужны SwimlaneRow, SwimlaneHeader) — расширяет, не создаёт с нуля
- **US3 (P2)**: Зависит от US1 (нужен swimlaneList в store, backend API) — может разрабатываться параллельно с US2
- **US4 (P3)**: Зависит от US1 (нужен Board с составными droppableId) — может разрабатываться параллельно с US2/US3

### Внутри каждой пользовательской истории

- Тесты ДОЛЖНЫ быть написаны первыми и НЕ ПРОХОДИТЬ перед реализацией (конституция: TDD)
- Backend-задачи перед frontend-задачами (в рамках Фундамента)
- Компоненты перед их использованием в Board
- Основная реализация перед интеграцией

### Возможности параллелизма

Внутри Фазы 2 (Фундамент):
- T004, T005, T006, T007 — все тесты параллельно
- T008, T009, T010 — параллельно (разные файлы)

Внутри Фазы 3 (US1):
- T018, T019 — параллельно
- T020, T021, T022 — параллельно (разные файлы)

Внутри Фазы 5 (US3) и Фазы 4 (US2):
- US2 и US3 могут разрабатываться параллельно разными разработчиками

---

## Пример параллелизма: Фундамент

```bash
# Параллельно: все тесты (T004-T007)
Задача: "Unit-тест GetSwimlanesQueryHandlerTests.cs"
Задача: "Unit-тест CreateTaskCommandHandlerTests.cs (расширить)"
Задача: "Unit-тест UpdateTaskCommandHandlerTests.cs (расширить)"
Задача: "Integration-тест SwimlaneEndpointTests.cs"

# Параллельно: реализация после тестов (T008-T010)
Задача: "GetSwimlanesQuery.cs"
Задача: "ITaskRepository.cs — GetSwimlanesAsync"
Задача: "TaskRepository.cs — GetSwimlanesAsync"
```

---

## Стратегия реализации

### Сначала MVP (только Пользовательская история 1)

1. Завершить Фазу 1: Настройка (доменная модель + миграция)
2. Завершить Фазу 2: Фундамент (backend API для swimlane)
3. Завершить Фазу 3: US1 — Просмотр задач в swimlane
4. **СТОП И ВАЛИДАЦИЯ**: Открыть доску → задачи сгруппированы по swimlane → «Без категории» первый → пустые ячейки видны
5. Развёртывание/демо

### Инкрементальная доставка

1. Настройка + Фундамент → Backend готов
2. Добавить US1 → Доска с swimlane → **Демо (MVP!)**
3. Добавить US2 → Сворачивание/разворачивание → Демо
4. Добавить US3 → Поле swimlane с автодополнением → Демо
5. Добавить US4 → Drag-and-drop между swimlane → Демо
6. Доработка → Bulk-операции, стили, accessibility → Финальная версия

### Стратегия параллельной работы команды

С двумя разработчиками после Фундамента:

1. Оба завершают Настройку + Фундамент вместе
2. После Фундамента:
   - Разработчик A: US1 (Просмотр swimlane) → US4 (Drag-and-drop)
   - Разработчик B: US2 (Сворачивание) → US3 (Поле swimlane)
3. Оба завершают Доработку вместе

---

## Заметки

- Задачи [P] = разные файлы, нет зависимостей
- Метка [US1–US4] привязывает задачу к конкретной пользовательской истории для трассировки
- Каждая пользовательская история должна быть независимо завершаемой и тестируемой
- Конституция требует TDD: сначала падающий тест, затем минимальная реализация
- Делайте коммит после каждой задачи или логической группы
- Останавливайтесь на любой контрольной точке для независимой валидации истории
- Избегайте: расплывчатых задач, конфликтов одних файлов, меж-исторных зависимостей, нарушающих независимость