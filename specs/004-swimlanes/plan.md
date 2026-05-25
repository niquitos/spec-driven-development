# План реализации: Swimlane-группировка задач на доске

**Ветка**: `feature/004-swimlanes` | **Дата**: 2026-05-25 | **Спецификация**: [spec.md](./spec.md)

**Ввод**: Спецификация функции из `/specs/004-swimlanes/spec.md`

## Сводка

Добавить на канбан-доскe горизонтальную группировку задач по swimlane — виртуальным группам, определяемым значением одноимённого текстового поля у задач. Задачи без группы попадают в swimlane «Без категории». Поддерживаются сворачивание/разворачивание swimlane с сохранением состояния в localStorage, автодополнение поля swimlane при редактировании, регистронезависимое совпадение имён и drag-and-drop между swimlane (вертикально) и колонками (горизонтально).

## Технический контекст

**Язык/Версия**: C# 12 (.NET 8) для backend, TypeScript 5 для frontend

**Основные зависимости**:
- Backend: ASP.NET Core 8, EF Core 8 (Npgsql), Scrutor
- Frontend: React 18, Zustand 4.5, @hello-pangea/dnd, Axios, date-fns, Vite 5

**Хранилище**: PostgreSQL 15 (через EF Core / Npgsql)

**Тестирование**:
- Backend: xUnit (unit-тесты для handlers/validators, integration-тесты для API)
- Frontend: Vitest + @testing-library/react (component tests), прямые unit-тесты для Zustand store

**Целевая платформа**: Docker (Linux-контейнеры), браузер (Chrome, Firefox, Safari)

**Тип проекта**: Веб-приложение (full-stack: REST API + SPA)

**Цели производительности**:
- Отображение доски с 200 задачами — не более 2 секунд (SC-001)
- Сворачивание/разворачивание swimlane — <100мс (re-render), визуальная анимация 200мс (SC-002)
- Изменение swimlane у задачи отражается на доске через optimistic update — мгновенно; подтверждение API — не более 2 секунд (SC-004)
- Автодополнение swimlane — фильтрация <50мс при 20 уникальных значениях (SC-003)
- SC-003 «90% пользователей могут назначить swimlane с первой попытки» — верифицируется через эвристическую оценку UX: автодополнение с free-text вводом, placeholder-подсказка, мгновенная обратная связь при выборе. Формальное usability-тестирование не входит в план; критерий считается выполненным при реализации перечисленных UX-паттернов

**Ограничения**:
- Swimlane — виртуальная группировка, не отдельная сущность (без таблицы в БД)
- Регистронезависимое совпадение имён swimlane с единым алгоритмом нормализации (`.toLowerCase()` на обеих сторонах)
- Состояние сворачивания — только localStorage по нормализованным ключам (не синхронизируется между устройствами; при смене устройства/браузера свёрнутые состояния сбрасываются — все swimlane отображаются развёрнутыми)
- Порядок swimlane фиксирован: «Без категории» — сверху, остальные — по алфавиту. Пользовательская перестановка swimlane (drag-and-drop строк) не входит в данную функцию
- Максимальная длина swimlane — 100 символов (валидация на backend и frontend)

**Масштаб/объём**: До 200 задач на доске, до ~20 уникальных swimlane

## Проверка конституции

*ВРАТА: Должна пройти перед исследованием Фазы 0. Повторная проверка после проектирования Фазы 1.*

| Принцип | Статус | Комментарий |
|---------|--------|-------------|
| I. TDD | ✅ | Каждый новый handler, validator и endpoint начинается с падающего теста. Компоненты frontend — через @testing-library/react |
| II. DDD | ✅ | Swimlane-поле добавляется к TaskEntity (Domain). Новый query/handler в Application. Репозиторий расширяется методом получения уникальных swimlane |
| III. SOLID | ✅ | Новый GetSwimlanesQuery + handler (SRP, OCP). Существующие команды расширяются полем swimlane. DI-регистрация через Scrutor |
| Tech stack | ✅ | C# 12/.NET 8, React 18, Zustand, EF Core 8 — всё в рамках конституции |
| Styles | ✅ | Единый CSS-файл (index.css) — без CSS-модулей |
| API | ✅ | REST, JSON — новый endpoint GET /api/tasks/swimlanes для списка уникальных значений |
| Git workflow | ✅ | Feature-ветка feature/004-swimlanes создана через SpecKit |

**Нарушения конституции**: Нет

## Граничные случаи и уточнения спецификации

### Визуальная модель (CHK001, CHK008)

Доска представляет собой матрицу swimlane × статус-колонка. Каждый swimlane — горизонтальная строка, протягивающаяся через все колонки статуса (Новые, В работе, Готово). В заголовке swimlane отображается название и общее количество задач (суммарно по всем колонкам). Пустые ячейки (колонки без задач в данном swimlane) остаются Droppable-областью для drag-and-drop.

### Drag-and-drop в свёрнутый swimlane (CHK032)

Свёрнутый swimlane принимает перетаскиваемую задачу. Задача обновляется через API, но swimlane не разворачивается автоматически — пользователь видит изменение счётчика задач в заголовке.

### Drag-and-drop последней задачи из swimlane (CHK016, CHK026)

При перетаскивании последней задачи из swimlane — optimistic update: задача мгновенно перемещается, исходный swimlane исчезает. При ошибке API — откат (rollback), swimlane появляется снова.

### Диагональное перемещение (CHK024)

При одновременном изменении swimlane и status (перетаскивание по диагонали) — оба поля обновляются одним API-вызовом `updateTask`.

### Drag-and-drop из «Без категории» (CHK025)

Перемещение из «Без категории» в именованный swimlane: `task.swimlane = displayName` (оригинальное написание). Обратное перемещение: `task.swimlane = null`.

### Пустой swimlane (CHK034)

Swimlane, в котором есть задачи, но все они находятся в других колонках (текущая колонка пуста), отображается как горизонтальная полоса с пустой ячейкой в данной колонке. Ячейка остаётся Droppable-областью.

### Регистронезависимость — единый алгоритм (CHK014, CHK045)

Единая функция нормализации на обеих сторонах: `.ToLower()` (C#) и `.toLowerCase()` (TypeScript). НЕ используется `toLocaleLowerCase()` на frontend, т.к. это создаёт расхождение с backend для некоторых Unicode-символов. Ключ для localStorage, droppableId и группировки — всегда нормализованный (lowercase).

### Валидация (CHK030, CHK031)

- **Превышение длины** (>100 символов): frontend maxlength на input, backend `400 Bad Request` с детальным сообщением
- **Пробельные строки**: нормализуются в `null` на обеих сторонах (`string.IsNullOrWhiteSpace` на backend, `trim()` + empty check на frontend)

### Фантомные записи в localStorage (CHK033)

При удалении swimlane (последняя задача ушла) — запись остаётся в localStorage, но не вызывает проблем: при следующем рендере несуществующий ключ не найдёт соответствующей группы и будет проигнорирован. Если пользователь заново создаёт swimlane с тем же именем — старая запись активирует свёрнутое состояние, что является ожидаемым поведением.

### Взаимодействие с bulk-операциями (CHK007, CHK044)

Поле swimlane сохраняется при bulk-операциях: bulk delete (задачи удаляются, swimlane обновляются), bulk move (задачи переносятся на другую дату с сохранением swimlane), move-incomplete-to-tomorrow (задачи переносятся с сохранением swimlane). Swimlane НЕ очищается и NOT переназначается автоматически.

### Фильтрация и группировка (CHK006, CHK048)

Фильтр по assignee и группировка по swimlane работают ортогонально: сначала фильтрация (backend-side), затем группировка отфильтрованных задач (frontend-side). Если в swimlane нет задач после фильтрации — swimlane не отображается.

### Количество задач (CHK002, CHK046)

В заголовке swimlane отображается общее количество задач (суммарно по всем колонкам), а не per-column. Формат: «Без категории (5)», «Фронтенд (12)».

### Уточнение приёмочных критериев (CHK019, CHK020, CHK021, CHK022)

- **US-1.3 «задачи размещаются в соответствующих колонках внутри этой swimlane»**: задачи внутри каждой колонки swimlane упорядочены по полю `order`, аналогично текущему поведению доски без swimlane
- **US-2.3 «состояние сворачивания сохраняется»**: тестовый критерий — после обновления страницы (F5) свёрнутые swimlane остаются свёрнутыми, развёрнутые — развёрнутыми. Ключи в localStorage — нормализованные (lowercase)
- **US-3.2 «автоматически создаётся новый swimlane»**: «автоматически» означает — без перезагрузки страницы, без дополнительных действий пользователя, мгновенно через optimistic update. Новый swimlane появляется на доске сразу после сохранения задачи с новым значением swimlane
- **US-4.1 «поле swimlane автоматически обновляется»**: «автоматически» означает — без модального окна подтверждения, без перезагрузки страницы, мгновенно через optimistic update. Drag-and-drop обновляет поле swimlane за один API-вызов

### Доступность (CHK038, CHK039, CHK040)

- **ARIA-атрибуты**: кнопка toggle swimlane имеет `role="button"`, `aria-expanded`, `aria-controls`, `aria-label`
- **Клавиатура**: `Enter`/`Space` сворачивает/разворачивает swimlane. Tab-порядок: заголовок → следующая задача/заголовок
- **Анимация**: CSS `transition` на `max-height` и `opacity`, длительность 200мс. При `prefers-reduced-motion: reduce` — мгновенное переключение без анимации

## Структура проекта

### Документация (этой функции)

```text
specs/004-swimlanes/
├── plan.md              # Этот файл (результат команды /speckit-plan)
├── research.md          # Результат Фазы 0 (команда /speckit-plan)
├── data-model.md        # Результат Фазы 1 (команда /speckit-plan)
├── quickstart.md        # Результат Фазы 1 (команда /speckit-plan)
├── contracts/           # Результат Фазы 1 (команда /speckit-plan)
│   └── swimlane-api.md  # REST API контракты
└── tasks.md             # Результат Фазы 2 (команда /speckit-tasks — НЕ создаётся командой /speckit-plan)
```

### Исходный код (корень репозитория)

```text
backend/
├── src/
│   ├── TaskTracker.Domain/
│   │   └── TaskEntity.cs                  # + Swimlane? property
│   ├── TaskTracker.Application/
│   │   └── Tasks/
│   │       ├── GetSwimlanesQuery.cs       # НОВЫЙ: query + handler + validator
│   │       ├── CreateTaskCommand.cs        # + Swimlane? в command, maxlength 100
│   │       ├── UpdateTaskCommand.cs        # + Swimlane? в command, maxlength 100
│   │       ├── GetTasksQuery.cs            # + swimlane в ответе
│   │       ├── GetTaskByIdQuery.cs         # + swimlane в ответе
│   │       └── ITaskRepository.cs          # + GetSwimlanesAsync
│   └── TaskTracker.Infrastructure/
│       ├── Persistence/
│       │   ├── AppDbContext.cs             # + DbConfiguration для Swimlane (maxlength 100, index)
│       │   ├── TaskRepository.cs           # + GetSwimlanesAsync, расширение GetByDateAsync
│       │   └── Migrations/                # Новая миграция AddSwimlaneToTask
│       └── TaskTracker.Api/
│           └── Controllers/TasksController.cs  # + GET /api/tasks/swimlanes, + swimlanes param
└── tests/
    ├── TaskTracker.UnitTests/
    │   └── Tasks/
    │       ├── GetSwimlanesQueryHandlerTests.cs  # НОВЫЙ
    │       ├── CreateTaskCommandHandlerTests.cs  # + тесты с swimlane (включая maxlength)
    │       └── UpdateTaskCommandHandlerTests.cs  # + тесты с swimlane (включая null-нормализацию)
    └── TaskTracker.IntegrationTests/
        └── SwimlaneEndpointTests.cs             # НОВЫЙ

frontend/
├── src/
│   ├── types/task.ts                  # + swimlane: string | null
│   ├── services/taskApi.ts            # + getSwimlanes(), + swimlanes param в getTasks
│   ├── stores/taskStore.ts            # + swimlaneList, collapsedSwimlanes, actions
│   ├── utils/swimlane.ts             # НОВЫЙ: normalizeSwimlaneKey, DEFAULT_SWIMLANE_KEY
│   ├── hooks/
│   │   └── useSwimlaneCollapse.ts     # НОВЫЙ: localStorage persistence
│   ├── components/
│   │   ├── Board.tsx                  # Рефакторинг: группировка по swimlane, матрица
│   │   ├── Column.tsx                 # Адаптация: droppableId с составным ключом
│   │   ├── TaskCard.tsx               # + swimlane в drag-and-drop
│   │   ├── SwimlaneRow.tsx            # НОВЫЙ: горизонтальная полоса + пустые ячейки
│   │   ├── SwimlaneHeader.tsx         # НОВЫЙ: заголовок + количество + collapse toggle (ARIA)
│   │   ├── SwimlaneCombobox.tsx       # НОВЫЙ: автодополнение (аналог AssigneeCombobox)
│   │   └── TaskModal/
│   │       ├── CreateTaskModal.tsx    # + поле swimlane (опциональное, по умолчанию null)
│   │       └── EditTaskModal.tsx      # + поле swimlane
│   └── index.css                      # + стили для swimlane (анимация, collapsed/expanded)
```

**Решение по структуре**: Вариант 2 (веб-приложение) — структура `backend/` + `frontend/`. Новые файлы добавляются в существующие директории в соответствии с Clean Architecture.

## Отслеживание сложности

> **Заполнять ТОЛЬКО если проверка конституции имеет нарушения, которые необходимо обосновать**

Нет нарушений конституции. Таблица не требуется.