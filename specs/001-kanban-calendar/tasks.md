# Tasks: Kanban Calendar Board

**Input**: Design documents from `/specs/001-kanban-calendar/`
**Prerequisites**: plan.md, spec.md, data-model.md, contracts/api.md, contracts/cli.md, quickstart.md

**Tests**: TDD mandatory — тесты пишутся перед реализацией для всей бизнес-логики

**Organization**: Tasks organized by user story for independent implementation and testing

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (US1, US2, US3...)
- Include exact file paths in descriptions

## Path Conventions

- **Backend**: `backend/src/TaskTracker.*`
- **Frontend**: `frontend/src/`
- **Tests**: `backend/tests/`, `frontend/tests/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [X] T001 Create backend solution structure: `backend/TaskTracker.sln`
- [X] T002 Create frontend project: `frontend/package.json` with TypeScript, React, Vite
- [X] T003 [P] Create backend projects: `TaskTracker.Api.csproj`, `TaskTracker.Application.csproj`, `TaskTracker.Domain.csproj`, `TaskTracker.Infrastructure.csproj`, `TaskTracker.Cli.csproj`
- [X] T004 [P] Configure ESLint + Prettier in `frontend/.eslintrc.js`, `frontend/.prettierrc`
- [X] T005 [P] Configure xUnit in `backend/tests/TaskTracker.UnitTests/TaskTracker.UnitTests.csproj`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T006 [P] Create `TaskStatus` enum in `backend/src/TaskTracker.Domain/TaskStatus.cs`
- [X] T007 [P] Create `TaskEntity` class in `backend/src/TaskTracker.Domain/TaskEntity.cs`
- [X] T008 [P] Create `IRequest<TResponse>` and `IRequest` interfaces in `backend/src/TaskTracker.Application/IRequest.cs`
- [X] T009 [P] Create `IRequestHandler<TRequest, TResponse>` interface in `backend/src/TaskTracker.Application/IRequestHandler.cs`
- [X] T010 [P] Create `IValidator<T>` interface in `backend/src/TaskTracker.Application/IValidator.cs`
- [X] T011 [P] Create `AppDbContext` in `backend/src/TaskTracker.Infrastructure/Persistence/AppDbContext.cs`
- [X] T012 Setup PostgreSQL connection string in `backend/src/TaskTracker.Api/appsettings.Development.json`
- [X] T013 [P] Create `ITaskRepository` interface in `backend/src/TaskTracker.Application/Tasks/ITaskRepository.cs`
- [X] T014 [P] Create `TaskRepository` implementation in `backend/src/TaskTracker.Infrastructure/Persistence/TaskRepository.cs`
- [X] T015 [P] Create CQRS handlers: `GetTasksQuery`, `CreateTaskCommand`, `UpdateTaskCommand`, `DeleteTaskCommand`, `MoveTaskCommand`
- [X] T016 [P] Create `DependencyInjection` for Application and Infrastructure
- [X] T017 [P] Create `TasksController` in `backend/src/TaskTracker.Api/Controllers/TasksController.cs`
- [X] T018 [P] Create frontend types: `frontend/src/types/task.ts`
- [X] T019 [P] Create Zustand store: `frontend/src/stores/taskStore.ts`
- [X] T020 [P] Create frontend API client: `frontend/src/services/taskApi.ts`

**Checkpoint**: Foundation ready — user story implementation can now begin in parallel

---

## Phase 3: User Story 1 — Просмотр задач на дату (Priority: P1) 🎯 MVP

**Goal**: Пользователь видит канбан-доску с задачами на выбранную дату

**Independent Test**: Можно открыть приложение, выбрать дату, увидеть задачи в правильных колонках

### Tests for User Story 1 ⚠️

- [X] T021 [P] [US1] Contract test for `GET /api/tasks?date=YYYY-MM-DD` in `backend/tests/TaskTracker.UnitTests/Features/Tasks/GetTasksByDate/GetTasksByDateContractTests.cs`
- [X] T022 [P] [US1] Validation test for invalid date format in `backend/tests/TaskTracker.UnitTests/Features/Tasks/GetTasksByDate/GetTasksByDateValidationTests.cs`
- [ ] T023 [P] [US1] Frontend integration test for board rendering in `frontend/tests/integration/Board.test.tsx`

### Implementation for User Story 1

- [X] T024 [P] [US1] Create `GetTasksByDateQuery` in `backend/src/TaskTracker.Application/Tasks/GetTasksQuery.cs`
- [X] T025 [P] [US1] Create `GetTasksByDateHandler` in `backend/src/TaskTracker.Application/Tasks/GetTasksQuery.cs`
- [X] T026 [US1] Create `TasksController.GetTasksByDate` in `backend/src/TaskTracker.Api/Controllers/TasksController.cs`
- [X] T027 [P] [US1] Create `Column` component in `frontend/src/components/Column.tsx`
- [X] T028 [P] [US1] Create `TaskCard` component in `frontend/src/components/TaskCard.tsx`
- [X] T029 [US1] Create `Board` component in `frontend/src/components/Board.tsx`
- [X] T030 [P] [US1] Create `Header` component with date navigation in `frontend/src/components/Header.tsx`
- [X] T031 [US1] Wire up `GET /api/tasks` call in `frontend/src/stores/taskStore.ts`
- [X] T032 [US1] Create `App.tsx` with Board + Header layout
- [X] T033 [P] [US1] Add aria-labels to icon buttons in `Column.tsx` (accessibility FR-016)
- [X] T034 [P] [US1] Add keyboard tab navigation to `Board.tsx` (accessibility FR-015)
- [X] T035 [P] [US1] Add focus indicators to interactive elements (accessibility FR-017)

**Checkpoint**: User Story 1 complete — board displays tasks for selected date

---

## Phase 4: User Story 2 — Навигация по датам (Priority: P1)

**Goal**: Пользователь перемещается между датами кнопками и date-picker

**Independent Test**: Кнопки "назад/вперёд" меняют дату, picker открывает календарь

### Tests for User Story 2 ⚠️

- [X] T036 [P] [US2] Unit test for DateNavigator previous/next logic in `frontend/tests/unit/DateNavigator.test.tsx`
- [X] T037 [P] [US2] Integration test for date change triggering task reload in `frontend/tests/integration/DateNavigation.test.tsx`

### Implementation for User Story 2

- [X] T038 [P] [US2] Add `date-fns` dependency to `frontend/package.json`
- [X] T039 [P] [US2] Create `DatePicker` component in `frontend/src/components/Header.tsx` (native date input)
- [X] T040 [US2] Add date state management in `frontend/src/stores/taskStore.ts`
- [X] T041 [US2] Wire up navigation buttons in `Header.tsx`
- [X] T042 [US2] Update `Board` component to use selected date from store

**Checkpoint**: User Story 2 complete — date navigation functional

---

## Phase 5: User Story 3 — Создание задачи (Priority: P1) 🎯 MVP

**Goal**: Пользователь создаёт задачу через "+" в колонке

**Independent Test**: Можно создать задачу с названием/описанием, она появляется в колонке

### Tests for User Story 3 ⚠️

- [X] T043 [P] [US3] Contract test for `POST /api/tasks` in `backend/tests/TaskTracker.UnitTests/Features/Tasks/CreateTask/CreateTaskContractTests.cs`
- [X] T044 [P] [US3] Validation tests for CreateTaskCommand in `backend/tests/TaskTracker.UnitTests/Features/Tasks/CreateTask/CreateTaskValidationTests.cs`
- [X] T045 [P] [US3] Handler unit test for successful creation in `backend/tests/TaskTracker.UnitTests/Features/Tasks/CreateTask/CreateTaskHandlerTests.cs`
- [X] T046 [P] [US3] Frontend integration test for task creation in `frontend/tests/integration/CreateTask.test.tsx`

### Implementation for User Story 3

- [X] T047 [P] [US3] Create `CreateTaskCommand` in `backend/src/TaskTracker.Application/Features/Tasks/CreateTask/CreateTaskCommand.cs`
- [X] T048 [P] [US3] Create `CreateTaskValidator` in `backend/src/TaskTracker.Infrastructure/Validators/CreateTaskValidator.cs`
- [X] T049 [P] [US3] Create `CreateTaskHandler` in `backend/src/TaskTracker.Application/Features/Tasks/CreateTask/CreateTaskHandler.cs`
- [X] T050 [US3] Add `TasksController.CreateTask` endpoint in `backend/src/TaskTracker.Api/Controllers/TasksController.cs`
- [X] T051 [P] [US3] Create `CreateTaskModal` component in `frontend/src/components/TaskModal/CreateTaskModal.tsx`
- [X] T052 [US3] Add "create task" button (+) to `Column.tsx`
- [X] T053 [US3] Wire up form submission in `CreateTaskModal.tsx`
- [X] T054 [US3] Add optimistic update in `taskStore.ts` after create

**Checkpoint**: User Story 3 complete — tasks can be created

---

## Phase 6: User Story 4 — Редактирование задачи (Priority: P2)

**Goal**: Пользователь редактирует задачу через иконку карандаша

**Independent Test**: Можно открыть редактирование, изменить поля, сохранить

### Tests for User Story 4 ⚠️

- [X] T055 [P] [US4] Contract test for `PUT /api/tasks/{id}` in `backend/tests/TaskTracker.UnitTests/Features/Tasks/UpdateTask/UpdateTaskContractTests.cs`
- [X] T056 [P] [US4] Handler unit test for UpdateTask in `backend/tests/TaskTracker.UnitTests/Features/Tasks/UpdateTask/UpdateTaskHandlerTests.cs`
- [X] T057 [P] [US4] Frontend integration test for edit flow in `frontend/tests/integration/EditTask.test.tsx`

### Implementation for User Story 4

- [X] T058 [P] [US4] Create `UpdateTaskCommand` in `backend/src/TaskTracker.Application/Features/Tasks/UpdateTask/UpdateTaskCommand.cs`
- [X] T059 [P] [US4] Create `UpdateTaskValidator` in `backend/src/TaskTracker.Infrastructure/Validators/UpdateTaskValidator.cs`
- [X] T060 [P] [US4] Create `UpdateTaskHandler` in `backend/src/TaskTracker.Application/Features/Tasks/UpdateTask/UpdateTaskHandler.cs`
- [X] T061 [US4] Add `TasksController.UpdateTask` endpoint in `backend/src/TaskTracker.Api/Controllers/TasksController.cs`
- [X] T062 [P] [US4] Create `EditTaskModal` component in `frontend/src/components/TaskModal/EditTaskModal.tsx`
- [X] T063 [US4] Add pencil icon to `TaskCard.tsx`
- [X] T064 [US4] Wire up edit form with pre-populated data
- [X] T065 [P] [US4] Create `UpdateTaskDateCommand` in `backend/src/TaskTracker.Application/Features/Tasks/UpdateTaskDate/UpdateTaskDateCommand.cs`
- [X] T066 [P] [US4] Create `UpdateTaskDateHandler` in `backend/src/TaskTracker.Application/Features/Tasks/UpdateTaskDate/UpdateTaskDateHandler.cs`
- [X] T067 [US4] Add date change handling (triggers re-fetch) in `EditTaskModal.tsx`

**Checkpoint**: User Story 4 complete — tasks can be edited

---

## Phase 7: User Story 5 — Удаление задачи (Priority: P2)

**Goal**: Пользователь удаляет задачу через иконку урны с подтверждением

**Independent Test**: Клик на урну → диалог подтверждения → задача удаляется

### Tests for User Story 5 ⚠️

- [X] T068 [P] [US5] Contract test for `DELETE /api/tasks/{id}` in `backend/tests/TaskTracker.UnitTests/Features/Tasks/DeleteTask/DeleteTaskContractTests.cs`
- [X] T069 [P] [US5] Handler unit test for DeleteTask in `backend/tests/TaskTracker.UnitTests/Features/Tasks/DeleteTask/DeleteTaskHandlerTests.cs`
- [X] T070 [P] [US5] Frontend integration test for delete confirmation in `frontend/tests/integration/DeleteTask.test.tsx`

### Implementation for User Story 5

- [X] T071 [P] [US5] Create `DeleteTaskCommand` in `backend/src/TaskTracker.Application/Features/Tasks/DeleteTask/DeleteTaskCommand.cs`
- [X] T072 [P] [US5] Create `DeleteTaskHandler` in `backend/src/TaskTracker.Application/Features/Tasks/DeleteTask/DeleteTaskHandler.cs`
- [X] T073 [US5] Add `TasksController.DeleteTask` endpoint in `backend/src/TaskTracker.Api/Controllers/TasksController.cs`
- [X] T074 [P] [US5] Create `DeleteConfirmModal` component in `frontend/src/components/TaskModal/DeleteConfirmModal.tsx`
- [X] T075 [US5] Add trash icon to `TaskCard.tsx`
- [X] T076 [US5] Wire up delete confirmation flow
- [X] T077 [US5] Add optimistic removal in `taskStore.ts`

**Checkpoint**: User Story 5 complete — tasks can be deleted

---

## Phase 8: User Story 6 — Перетаскивание задач (Priority: P2)

**Goal**: Drag-n-drop задач между колонками и внутри колонки

**Independent Test**: Можно перетащить задачу в другую колонку, статус меняется

### Tests for User Story 6 ⚠️

- [X] T078 [P] [US6] Contract test for `PATCH /api/tasks/{id}/status` in `backend/tests/TaskTracker.UnitTests/Features/Tasks/UpdateTaskStatus/UpdateTaskStatusContractTests.cs`
- [X] T079 [P] [US6] Frontend integration test for drag-n-drop in `frontend/tests/integration/DragDrop.test.tsx`

### Implementation for User Story 6

- [X] T080 [P] [US6] Add `@dnd-kit/core` and `@dnd-kit/sortable` to `frontend/package.json`
- [X] T081 [P] [US6] Create `useDragDrop` hook in `frontend/src/hooks/useDragDrop.ts`
- [X] T082 [US6] Wrap `Board` with `DndContext`
- [X] T083 [US6] Make `TaskCard` draggable with `useSortable`
- [X] T084 [US6] Implement `handleDragEnd` for status change
- [X] T085 [P] [US6] Create `UpdateTaskStatusCommand` in `backend/src/TaskTracker.Application/Features/Tasks/UpdateTaskStatus/UpdateTaskStatusCommand.cs`
- [X] T086 [P] [US6] Create `UpdateTaskStatusHandler` in `backend/src/TaskTracker.Application/Features/Tasks/UpdateTaskStatus/UpdateTaskStatusHandler.cs`
- [X] T087 [US6] Add `TasksController.UpdateTaskStatus` endpoint

**Checkpoint**: User Story 6 complete — drag-n-drop functional

---

## Phase 9: User Story 7 — Массовые операции (Priority: P3)

**Goal**: Выбор задач галочками и массовые операции

**Independent Test**: Можно выбрать несколько задач, выполнить массовое удаление/перемещение

### Tests for User Story 7 ⚠️

- [ ] T088 [P] [US7] Contract test for `POST /api/tasks/bulk/delete` in `backend/tests/TaskTracker.UnitTests/Features/Tasks/BulkDelete/BulkDeleteContractTests.cs`
- [ ] T089 [P] [US7] Contract test for `POST /api/tasks/bulk/move` in `backend/tests/TaskTracker.UnitTests/Features/Tasks/BulkMove/BulkMoveContractTests.cs`
- [ ] T090 [P] [US7] Frontend integration test for bulk selection in `frontend/tests/integration/BulkActions.test.tsx`

### Implementation for User Story 7

- [ ] T091 [P] [US7] Create `BulkDeleteCommand` in `backend/src/TaskTracker.Application/Features/Tasks/BulkDelete/BulkDeleteCommand.cs`
- [ ] T092 [P] [US7] Create `BulkDeleteHandler` in `backend/src/TaskTracker.Application/Features/Tasks/BulkDelete/BulkDeleteHandler.cs`
- [ ] T093 [P] [US7] Create `BulkMoveCommand` in `backend/src/TaskTracker.Application/Features/Tasks/BulkMove/BulkMoveCommand.cs`
- [ ] T094 [P] [US7] Create `BulkMoveHandler` in `backend/src/TaskTracker.Application/Features/Tasks/BulkMove/BulkMoveHandler.cs`
- [ ] T095 [US7] Add `TasksController.BulkDelete` endpoint
- [ ] T096 [US7] Add `TasksController.BulkMove` endpoint
- [ ] T097 [P] [US7] Create checkbox component in `frontend/src/components/Board/TaskCheckbox.tsx`
- [ ] T098 [US7] Add selection state to `taskStore.ts`
- [ ] T099 [US7] Create `BulkActionsPanel` component in `frontend/src/components/BulkActions/BulkActionsPanel.tsx`
- [ ] T100 [US7] Wire up bulk delete action
- [ ] T101 [US7] Wire up bulk move action

**Checkpoint**: User Story 7 complete — bulk operations functional

---

## Phase 10: CLI & Observability (Constitution Requirements)

**Purpose**: Constitution I. CLI-First, IV. Observability

- [ ] T102 [P] Create `CreateTaskCommand` CLI in `backend/src/TaskTracker.Cli/Commands/Tasks/CreateTaskCommand.cs`
- [ ] T103 [P] Create `ListTasksCommand` CLI in `backend/src/TaskTracker.Cli/Commands/Tasks/ListTasksCommand.cs`
- [ ] T104 [P] Create `DeleteTaskCommand` CLI in `backend/src/TaskTracker.Cli/Commands/Tasks/DeleteTaskCommand.cs`
- [ ] T105 [P] Create `UpdateTaskCommand` CLI in `backend/src/TaskTracker.Cli/Commands/Tasks/UpdateTaskCommand.cs`
- [ ] T106 [P] Create `UpdateTaskStatusCommand` CLI in `backend/src/TaskTracker.Cli/Commands/Tasks/UpdateTaskStatusCommand.cs`
- [ ] T107 [P] Create `BulkDeleteCommand` CLI in `backend/src/TaskTracker.Cli/Commands/Tasks/BulkDeleteCommand.cs`
- [ ] T108 Setup Serilog in `backend/src/TaskTracker.Api/Program.cs`
- [ ] T109 Configure structured logging for CQRS handlers
- [ ] T110 [P] Create CLI tests in `backend/tests/TaskTracker.Cli.Tests/`

---

## Phase 11: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] T111 [P] Add loading states to `Board.tsx` and `Column.tsx`
- [ ] T112 [P] Add error handling middleware in `backend/src/TaskTracker.Api/Middleware/ExceptionMiddleware.cs`
- [ ] T113 [P] Create Dockerfile for backend
- [ ] T114 [P] Create Dockerfile for frontend
- [ ] T115 [P] Create `docker-compose.yml`
- [ ] T116 Documentation updates in `README.md`
- [ ] T117 Run UX checklist from `checklists/ux.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Setup — BLOCKS all user stories
- **User Stories (Phase 3-9)**: All depend on Foundational completion
  - Stories can run in parallel after Phase 2
  - Or sequentially in priority order (P1 → P2 → P3)
- **CLI & Observability (Phase 10)**: Depends on any P1 story complete
- **Polish (Phase 11)**: Depends on all desired user stories complete

### User Story Dependencies

- **US1 (P1)**: After Foundational — No story dependencies
- **US2 (P1)**: After Foundational — Independent
- **US3 (P1)**: After Foundational — Independent (MVP complete with US1+US2+US3)
- **US4 (P2)**: After Foundational — Independent
- **US5 (P2)**: After Foundational — Independent
- **US6 (P2)**: After Foundational — Independent
- **US7 (P3)**: After Foundational — Independent

### Within Each User Story

1. Tests (if TDD) MUST be written and FAIL before implementation
2. Commands/Queries before Handlers
3. Handlers before Controllers
4. Backend before Frontend components
5. Components before store wiring

### Parallel Opportunities

- All Setup tasks marked [P] can run in parallel (T003-T005)
- All Foundational tasks marked [P] can run in parallel (T006-T020)
- After Phase 2, all user stories can start in parallel (if team capacity)
- All tests for a story marked [P] can run in parallel
- All models/commands marked [P] within a story can run in parallel

---

## Parallel Example: User Story 1

```bash
# Launch all tests for User Story 1 together:
Task: "T019 [P] [US1] Contract test for GET /api/tasks?date=YYYY-MM-DD"
Task: "T020 [P] [US1] Validation test for invalid date format"
Task: "T021 [P] [US1] Frontend integration test for board rendering"

# Launch all backend commands/queries for User Story 1:
Task: "T022 [P] [US1] Create GetTasksByDateQuery"

# Launch all frontend components for User Story 1:
Task: "T025 [P] [US1] Create Column component"
Task: "T026 [P] [US1] Create TaskCard component"
Task: "T028 [P] [US1] Create DateNavigator component"
Task: "T031 [P] [US1] Add aria-labels to icon buttons"
Task: "T032 [P] [US1] Add keyboard tab navigation"
Task: "T033 [P] [US1] Add focus indicators"
```

---

## Implementation Strategy

### MVP First (User Stories 1-3 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL — blocks all stories)
3. Complete Phase 3: US1 (Просмотр задач)
4. Complete Phase 4: US2 (Навигация по датам)
5. Complete Phase 5: US3 (Создание задач)
6. **STOP and VALIDATE**: Test MVP independently
7. Deploy/demo if ready

### Incremental Delivery

1. Setup + Foundational → Foundation ready
2. Add US1 + US2 + US3 → Test independently → Deploy/Demo (MVP!)
3. Add US4 (Редактирование) → Test independently
4. Add US5 (Удаление) → Test independently
5. Add US6 (Drag-n-drop) → Test independently
6. Add US7 (Массовые операции) → Test independently
7. Each story adds value without breaking previous stories

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: User Story 1 (Просмотр)
   - Developer B: User Story 2 (Навигация)
   - Developer C: User Story 3 (Создание)
3. Stories complete and integrate independently

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- Verify tests fail before implementing (TDD)
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- Avoid: vague tasks, same file conflicts, cross-story dependencies that break independence
