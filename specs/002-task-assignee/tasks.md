---
description: "Список задач для реализации назначения исполнителей"
---

# Tasks: Назначение исполнителей задач

**Input**: Документы из `/specs/002-task-assignee/`  
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/api.md

**Tests**: Включены согласно Test-First принципу конституции проекта. Тесты пишутся до реализации.

**Organization**: Задачи сгруппированы по user story для независимой реализации и тестирования каждой.

## Формат: `[ID] [P?] [Story] Описание`

- **[P]**: Можно выполнять параллельно (разные файлы, нет зависимостей)
- **[Story]**: Какая user story (US1, US2, US3)
- Указывать точный путь к файлу

## Phase 1: Фундаментальные (Блокирующие)

**Purpose**: Backend-модель + frontend-типы — блокирует все user stories

- [x] T001 Добавить поле Assignee в TaskEntity в backend/src/TaskTracker.Domain/TaskEntity.cs
- [x] T002 [P] Добавить поле Assignee в CreateTaskCommand в backend/src/TaskTracker.Application/Tasks/CreateTaskCommand.cs
- [x] T003 [P] Добавить поле Assignee в UpdateTaskCommand в backend/src/TaskTracker.Application/Tasks/UpdateTaskCommand.cs
- [x] T003.5 [P] [US1] Юнит-тест — CreateTaskCommandHandler проверяет присвоение Assignee в backend/tests/TaskTracker.Application.Tests/Tasks/CreateTaskCommandHandlerTests.cs
- [x] T003.6 [P] [US1] Юнит-тест — UpdateTaskCommandHandler проверяет обновление Assignee в backend/tests/TaskTracker.Application.Tests/Tasks/UpdateTaskCommandHandlerTests.cs
- [x] T004 Добавить валидацию Assignee в CreateTaskCommandValidator в backend/src/TaskTracker.Infrastructure/Validators/CreateTaskValidator.cs
- [x] T005 Добавить присвоение Assignee в CreateTaskCommandHandler в backend/src/TaskTracker.Application/Tasks/CreateTaskCommand.cs
- [x] T006 Добавить присвоение Assignee в UpdateTaskCommandHandler в backend/src/TaskTracker.Application/Tasks/UpdateTaskCommand.cs
- [x] T007 [P] Добавить поле assignee в интерфейс Task в frontend/src/types/task.ts
- [x] T008 [P] Добавить поле assignee в CreateTaskDto в frontend/src/types/task.ts
- [x] T009 [P] Добавить поле assignee в UpdateTaskDto в frontend/src/types/task.ts
- [x] T010 Создать EF Core миграцию для колонки Assignee в backend/src/TaskTracker.Infrastructure/Persistence/Migrations/

**Checkpoint**: Backend и frontend типы обновлены — поле assignee проходит через весь стек

---

## Phase 2: User Stories 1+2 — Назначение и просмотр исполнителя (Priority: P1) 🎯 MVP

**Goal**: Пользователь может назначить исполнителя при создании/редактировании задачи и видеть его на карточке

**Independent Test**: Создать задачу с новым исполнителем, проверить что имя отображается на карточке. Создать ещё одну задачу — проверить что исполнитель появился в выпадающем списке.

### Тесты для User Story 1+2 ⚠️

- [x] T011 [P] [US1] Юнит-тест CreateTaskCommandValidator — валидация assignee (пусто, пробелы, макс. длина) в backend/tests/TaskTracker.Application.Tests/Tasks/CreateTaskCommandValidatorTests.cs
- [x] T012 [P] [US1] Юнит-тест UpdateTaskCommandHandler — обновление assignee в backend/tests/TaskTracker.Application.Tests/Tasks/UpdateTaskCommandHandlerTests.cs
- [x] T013 [US1] Интеграционный тест — создание задачи с assignee через API (test-server + test-containers) в backend/tests/TaskTracker.Integration.Tests/Tasks/CreateTaskWithAssigneeTests.cs
- [x] T014 [US1] Frontend-тест — AssigneeCombobox отображает существующих исполнителей в frontend/tests/unit/AssigneeCombobox.test.tsx

### Реализация User Stories 1+2

- [x] T015 [P] [US1] Создать компонент AssigneeCombobox в frontend/src/components/AssigneeCombobox.tsx
- [x] T016 [US1] Добавить поле assignee в CreateTaskModal в frontend/src/components/TaskModal/CreateTaskModal.tsx
- [x] T017 [US1] Добавить поле assignee в EditTaskModal в frontend/src/components/TaskModal/EditTaskModal.tsx
- [x] T018 [US1] Отобразить имя исполнителя на TaskCard в frontend/src/components/TaskCard.tsx
- [x] T019 [US1] Добавить assigneeList (производный список) в taskStore в frontend/src/stores/taskStore.ts

**Checkpoint**: MVP готов — исполнителя можно назначить и видно на карточке

---

## Phase 3: User Story 3 — Фильтрация по исполнителям (Priority: P2)

**Goal**: Пользователь может фильтровать задачи по одному или нескольким исполнителям. Фильтр хранится в URL query-параметрах.

**Independent Test**: Выбрать двух исполнителей в фильтре, переключиться на другую дату и обратно — фильтр сохраняется.

### Тесты для User Story 3 ⚠️

- [x] T020 [P] [US2] Юнит-тест GetTasksQueryHandler — фильтрация по assignee в backend/tests/TaskTracker.UnitTests/Tasks/GetTasksQueryHandlerTests.cs
- [x] T021 [P] [US2] Интеграционный тест — GET /api/tasks?assignees=... фильтр (test-server + test-containers) в backend/tests/TaskTracker.Integration.Tests/Tasks/FilterByAssigneeTests.cs
- [x] T022 [US2] Frontend-тест — AssigneeFilter сохраняет выбор в URL в frontend/tests/unit/AssigneeFilter.test.tsx
- [x] T022.1 [US2] Frontend-тест — AssigneeFilter отображает визуальный индикатор (цвет + текст) при активном фильтре в frontend/tests/unit/AssigneeFilter.test.tsx

### Реализация User Story 3

- [x] T023 [US2] Добавить параметр assignees фильтра в GetTasksQuery в backend/src/TaskTracker.Application/Tasks/GetTasksQuery.cs
- [x] T024 [US2] Обновить GetTasksQueryHandler для фильтрации по списку assignee в backend/src/TaskTracker.Application/Tasks/GetTasksQuery.cs
- [x] T025 [US2] Обновить ITaskRepository.GetByDateAsync для поддержки фильтра assignee в backend/src/TaskTracker.Application/Tasks/ITaskRepository.cs
- [x] T026 [US2] Обновить реализацию репозитория для фильтрации по assignee в backend/src/TaskTracker.Infrastructure/Persistence/TaskRepository.cs
- [x] T027 [US2] Добавить параметр assignees в GET /api/tasks в backend/src/TaskTracker.Api/Controllers/TasksController.cs
- [x] T028 [US2] Создать хук useAssigneeFilter в frontend/src/hooks/useAssigneeFilter.ts
- [x] T029 [P] [US2] Создать компонент AssigneeFilter в frontend/src/components/AssigneeFilter.tsx
- [x] T030 [US2] Добавить AssigneeFilter в Header в frontend/src/components/Header.tsx
- [x] T031 [US2] Добавить параметр assignees в taskApi.getTasks в frontend/src/services/taskApi.ts
- [x] T032 [US2] Добавить состояние assigneeFilter + логику фильтрации задач в taskStore в frontend/src/stores/taskStore.ts
- [x] T032.1 [US2] Отображать сообщение “Нет задач, соответствующих фильтру” при пустом результате в frontend/src/components/TaskBoard.tsx

**Checkpoint**: Фильтр работает — задачи фильтруются по исполнителю, фильтр сохраняется в URL

---

## Phase 4: Полировка и сквозные улучшения

**Purpose**: Завершающие улучшения

- [x] T033 P Добавить ARIA-метки и клавиатурную доступность в AssigneeCombobox в frontend/src/components/AssigneeCombobox.tsx
- [x] T034 P Добавить ARIA-метки в AssigneeFilter в frontend/src/components/AssigneeFilter.tsx
- [x] T034.1 Обработка: удаление последней задачи, соответствующей активному фильтру — фильтр остаётся активным, доска пустая с сообщением в frontend/src/stores/taskStore.ts
- [x] T034.2 Тест: удаление единственной задачи под фильтром оставляет фильтр активным в frontend/tests/stores/taskStore.test.ts
- [x] T035 Добавить структурированное логирование операций с assignee в backend-обработчиках
- [x] T036 Запустить валидацию по [quickstart.md](http://quickstart.md) (миграция, сборка, тесты)

---

## Phase 5: Техдолг — Интеграционные тесты (Real DB)

**Purpose**: Покрыть существующий и новый функционал интеграционными тестами с реальной БД через test-server + test-containers

### Инфраструктура для интеграционных тестов

- [ ] T037 Создать скрипт [setup-integration-tests.sh](http://setup-integration-tests.sh) для создания тестовой БД (bash: `createdb task_tracker_test`) в scripts/setup-integration-tests.sh
- [ ] T038 Создать скрипт [teardown-integration-tests.sh](http://teardown-integration-tests.sh) для удаления тестовой БД (bash: `dropdb task_tracker_test`) в scripts/teardown-integration-tests.sh
- [ ] T039 Настроить IntegrationTestWebAppFactory с test-containers (PostgreSQL container) в backend/tests/TaskTracker.Integration.Tests/IntegrationTestWebAppFactory.cs
- [ ] T040 Настроить базовый класс IntegrationTestBase с инициализацией БД и клиентом API в backend/tests/TaskTracker.Integration.Tests/IntegrationTestBase.cs
- [ ] T041 Создать [run-integration-tests.sh](http://run-integration-tests.sh) (создать БД → dotnet test → удалить БД) в scripts/run-integration-tests.sh

### Тесты существующего функционала

- [ ] T042 P Интеграционный тест — создание задачи (POST /api/tasks) в backend/tests/TaskTracker.Integration.Tests/Tasks/CreateTaskTests.cs
- [ ] T043 P Интеграционный тест — получение задач по дате (GET /api/tasks?date=) в backend/tests/TaskTracker.Integration.Tests/Tasks/GetTasksTests.cs
- [ ] T044 P Интеграционный тест — обновление задачи (PUT /api/tasks/{id}) в backend/tests/TaskTracker.Integration.Tests/Tasks/UpdateTaskTests.cs
- [ ] T045 P Интеграционный тест — удаление задачи (DELETE /api/tasks/{id}) в backend/tests/TaskTracker.Integration.Tests/Tasks/DeleteTaskTests.cs
- [ ] T046 P Интеграционный тест — bulk delete (POST /api/tasks/bulk/delete) в backend/tests/TaskTracker.Integration.Tests/Tasks/BulkDeleteTests.cs
- [ ] T047 P Интеграционный тест — bulk move (POST /api/tasks/bulk/move) в backend/tests/TaskTracker.Integration.Tests/Tasks/BulkMoveTests.cs
- [ ] T048 P Интеграционный тест — валидация (ошибки при пустом заголовке, длинном описании) в backend/tests/TaskTracker.Integration.Tests/Tasks/ValidationTests.cs
- [ ] T049 Интеграционный тест — ошибка 404 при обновлении/удалении несуществующей задачи в backend/tests/TaskTracker.Integration.Tests/Tasks/NotFoundTests.cs

---

## Зависимости и порядок выполнения

### Зависимости фаз

- **Phase 1**: Нет зависимостей — можно начинать сразу
- **Phase 2**: Зависит от Phase 1 — БЛОКИРУЕТ все user stories
- **Phase 3**: Зависит от Phase 2 (нужны данные assignee для фильтрации)
- **Phase 4**: Зависит от Phase 2, Phase 3
- **Phase 5**: Не зависит от других фаз — можно выполнять параллельно с Phase 2/3/4

### Зависимости User Story

- **User Story 1+2 (P1)**: Можно начинать после Phase 1 — Нет зависимостей от других stories
- **User Story 3 (P2)**: Зависит от US1+2 — нужны задачи с assignee для фильтрации

### Возможности для параллельного выполнения

- Все фундаментальные backend-задачи (T001-T006) и frontend-типы (T007-T009) можно выполнять параллельно
- Все тесты для одной story с меткой P можно запускать параллельно
- Backend (T023-T027) и frontend (T028-T032) в Phase 3 можно параллелить

---

## Примеры параллельного выполнения

### Phase 1

```bash
# Backend + frontend типы параллельно:
Task: "T001 Добавить Assignee в TaskEntity"
Task: "T002 Добавить Assignee в CreateTaskCommand"
Task: "T007 Добавить assignee в тип Task"
```

### Phase 2

```bash
# Тесты параллельно:
Task: "T011 Тесты CreateTaskCommandValidator"
Task: "T012 Тесты UpdateTaskCommandHandler"

# Компоненты параллельно:
Task: "T015 AssigneeCombobox"
Task: "T018 Отображение на TaskCard"
```

### Phase 3

```bash
# Backend + frontend параллельно:
Task: "T023 Фильтр assignee в GetTasksQuery"
Task: "T028 Хук useAssigneeFilter"
```

---

## Стратегия реализации

### MVP First (Phase 1 + Phase 2)

1. Выполнить Phase 1: Фундаментальные (backend + типы)
2. Выполнить Phase 2: US1+2 (назначение + просмотр)
3. **ОСТАНОВИТЬСЯ и ПРОВЕРИТЬ**: Протестировать создание assignee + отображение
4. Деплой/демо если готово

### Инкрементальная доставка

1. Выполнить Phase 1 → Фундамент готов
2. Добавить Phase 2 (US1+2) → Протестировать → Деплой/Демо (MVP!)
3. Добавить Phase 3 (US3) → Протестировать → Деплой/Демо
4. Добавить Phase 4 (Полировка) → Финальная валидация

---

## Примечания

- P задачи = разные файлы, нет зависимостей
- US1 = User Stories 1+2 (назначение/просмотр), US2 = User Story 3 (фильтр)
- Тесты ДОЛЖНЫ падать перед реализацией (TDD)
- Assignee — nullable строка, без отдельной сущности
- Фильтр в URL query-параметрах (?assignees=Иван,Петр)
- Коммитить после каждой задачи или логической группы