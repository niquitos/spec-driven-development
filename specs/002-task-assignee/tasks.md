---

description: "Task list for assignee feature implementation"

# Tasks: Назначение исполнителей задач

**Input**: Design documents from `/specs/002-task-assignee/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/api.md

**Tests**: Включены согласно Test-First принципу конституции проекта. Тесты пишутся до реализации.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths

## Phase 1: Foundational (Blocking Prerequisites)

**Purpose**: Backend model + frontend types — блокирует все user stories

- [ ] T001 Add Assignee field to TaskEntity in backend/src/TaskTracker.Domain/TaskEntity.cs
- [ ] T002 [P] Add Assignee field to CreateTaskCommand record in backend/src/TaskTracker.Application/Tasks/CreateTaskCommand.cs
- [ ] T003 [P] Add Assignee field to UpdateTaskCommand record in backend/src/TaskTracker.Application/Tasks/UpdateTaskCommand.cs
- [ ] T004 Add Assignee validation to CreateTaskCommandValidator in backend/src/TaskTracker.Application/Tasks/CreateTaskCommand.cs
- [ ] T005 Add Assignee to TaskEntity mapping in CreateTaskCommandHandler (assign from request)
- [ ] T006 Add Assignee to TaskEntity mapping in UpdateTaskCommandHandler (assign from request)
- [ ] T007 [P] Add assignee field to Task interface in frontend/src/types/task.ts
- [ ] T008 [P] Add assignee field to CreateTaskDto interface in frontend/src/types/task.ts
- [ ] T009 [P] Add assignee field to UpdateTaskDto interface in frontend/src/types/task.ts
- [ ] T010 Generate EF Core migration for new Assignee column in backend/src/TaskTracker.Infrastructure/Persistence/Migrations/

**Checkpoint**: Backend and frontend types updated — assignee field flows through the stack

---

## Phase 2: User Stories 1+2 — Назначение и просмотр исполнителя (Priority: P1) 🎯 MVP

**Goal**: Пользователь может назначить исполнителя при создании/редактировании задачи и видеть его на карточке

**Independent Test**: Создать задачу с новым исполнителем, проверить что имя отображается на карточке. Создать ещё одну задачу — проверить что исполнитель появился в выпадающем списке.

### Tests for User Story 1+2 ⚠️

- [ ] T011 [P] [US1] Unit test for CreateTaskCommandValidator — assignee validation (empty, whitespace, max length) in backend/tests/TaskTracker.Application.Tests/Tasks/CreateTaskCommandValidatorTests.cs
- [ ] T012 [P] [US1] Unit test for UpdateTaskCommandHandler — assignee update in backend/tests/TaskTracker.Application.Tests/Tasks/UpdateTaskCommandHandlerTests.cs
- [ ] T013 [US1] Integration test — create task with assignee via API, verify assignee in response in backend/tests/TaskTracker.Integration.Tests/Tasks/CreateTaskTests.cs
- [ ] T014 [US1] Frontend test — AssigneeCombobox renders existing assignees in frontend/tests/components/AssigneeCombobox.test.tsx

### Implementation for User Stories 1+2

- [ ] T015 [P] [US1] Create AssigneeCombobox component in frontend/src/components/AssigneeCombobox.tsx
- [ ] T016 [US1] Add assignee field to CreateTaskModal in frontend/src/components/TaskModal/CreateTaskModal.tsx
- [ ] T017 [US1] Add assignee field to EditTaskModal in frontend/src/components/TaskModal/EditTaskModal.tsx
- [ ] T018 [US1] Render assignee name on TaskCard in frontend/src/components/TaskCard.tsx
- [ ] T019 [US1] Add assigneeList derived state to taskStore in frontend/src/stores/taskStore.ts

**Checkpoint**: MVP ready — assignee can be assigned and viewed on the card

---

## Phase 3: User Story 3 — Фильтрация по исполнителям (Priority: P2)

**Goal**: Пользователь может фильтровать задачи по одному или нескольким исполнителям. Фильтр хранится в URL query-параметрах.

**Independent Test**: Выбрать двух исполнителей в фильтре, переключиться на другую дату и обратно — фильтр сохраняется.

### Tests for User Story 3 ⚠️

- [ ] T020 [P] [US2] Unit test for GetTasksQueryHandler — filter by assignees in backend/tests/TaskTracker.Application.Tests/Tasks/GetTasksQueryHandlerTests.cs
- [ ] T021 [P] [US2] Integration test — GET /api/tasks?assignees=... filter in backend/tests/TaskTracker.Integration.Tests/Tasks/GetTasksTests.cs
- [ ] T022 [US2] Frontend test — AssigneeFilter persists selection in URL in frontend/tests/components/AssigneeFilter.test.tsx

### Implementation for User Story 3

- [ ] T023 [US2] Add assignees filter param to GetTasksQuery record in backend/src/TaskTracker.Application/Tasks/GetTasksQuery.cs
- [ ] T024 [US2] Update GetTasksQueryHandler to filter by assignees list in backend/src/TaskTracker.Application/Tasks/GetTasksQuery.cs
- [ ] T025 [US2] Update ITaskRepository.GetByDateAsync to support assignee filter in backend/src/TaskTracker.Application/Tasks/ITaskRepository.cs
- [ ] T026 [US2] Update repository implementation for assignee filtering in backend/src/TaskTracker.Infrastructure/Persistence/TaskRepository.cs
- [ ] T027 [US2] Add assignees param to GET /api/tasks in backend/src/TaskTracker.Api/Controllers/TasksController.cs
- [ ] T028 [US2] Create useAssigneeFilter hook in frontend/src/hooks/useAssigneeFilter.ts
- [ ] T029 [P] [US2] Create AssigneeFilter component in frontend/src/components/AssigneeFilter.tsx
- [ ] T030 [US2] Add AssigneeFilter to Header component in frontend/src/components/Header.tsx
- [ ] T031 [US2] Add assignees param to taskApi.getTasks in frontend/src/services/taskApi.ts
- [ ] T032 [US2] Add assigneeFilter state + filtered tasks logic to taskStore in frontend/src/stores/taskStore.ts

**Checkpoint**: Filter works — tasks are filtered by assignee, filter persists in URL across date changes

---

## Phase 4: Polish & Cross-Cutting Concerns

**Purpose**: Завершающие улучшения

- [ ] T033 [P] Add ARIA labels and keyboard accessibility to AssigneeCombobox in frontend/src/components/AssigneeCombobox.tsx
- [ ] T034 [P] Add ARIA labels to AssigneeFilter in frontend/src/components/AssigneeFilter.tsx
- [ ] T035 Add structured logging for assignee operations in backend handlers
- [ ] T036 Run quickstart.md validation (migration, build, test)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Foundational (Phase 1)**: No dependencies — can start immediately
- **US1+2 (Phase 2)**: Depends on Phase 1 — BLOCKS all user stories
- **US3 (Phase 3)**: Depends on Phase 2 (needs assignee data to filter)
- **Polish (Phase 4)**: Depends on Phase 2, Phase 3

### User Story Dependencies

- **User Story 1+2 (P1)**: Can start after Phase 1 — No dependencies on other stories
- **User Story 3 (P2)**: Depends on US1+2 — needs assignee field on tasks to filter

### Parallel Opportunities

- All foundational backend tasks (T001-T006) and frontend type tasks (T007-T009) can run in parallel
- All tests for a user story marked [P] can run in parallel
- US3 filter backend (T023-T027) and frontend (T028-T032) can be parallelized within the phase

---

## Parallel Example: Phase 1 Foundational

```bash
# Backend domain + frontend types in parallel:
Task: "T001 Add Assignee to TaskEntity"
Task: "T002 Add Assignee to CreateTaskCommand"
Task: "T007 Add assignee to Task type"
```

## Parallel Example: Phase 2 US1+2

```bash
# Tests in parallel:
Task: "T011 CreateTaskCommandValidator tests"
Task: "T012 UpdateTaskCommandHandler tests"

# Components in parallel:
Task: "T015 AssigneeCombobox component"
Task: "T018 TaskCard assignee display"
```

## Parallel Example: Phase 3 US3

```bash
# Backend + frontend in parallel:
Task: "T023 GetTasksQuery assignee filter"
Task: "T028 useAssigneeFilter hook"
```

---

## Implementation Strategy

### MVP First (Phase 1 + Phase 2)

1. Complete Phase 1: Foundational (backend + types)
2. Complete Phase 2: US1+2 (assign + view)
3. **STOP and VALIDATE**: Test assignee creation + display independently
4. Deploy/demo if ready

### Incremental Delivery

1. Complete Phase 1 → Foundation ready
2. Add Phase 2 (US1+2) → Test independently → Deploy/Demo (MVP!)
3. Add Phase 3 (US3) → Test independently → Deploy/Demo
4. Add Phase 4 (Polish) → Final validation

---

## Notes

- [P] tasks = different files, no dependencies
- [US1] = User Stories 1+2 (assign/view), [US2] = User Story 3 (filter)
- Tests MUST fail before implementation (TDD)
- Assignee is nullable string — no separate entity
- Filter in URL query params (?assignees=Иван,Петр)
- Commit after each task or logical group
