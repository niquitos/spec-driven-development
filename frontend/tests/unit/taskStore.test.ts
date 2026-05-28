import { describe, it, expect, vi, beforeEach } from 'vitest';
import { useTaskStore } from '../../src/stores/taskStore';
import { taskApi } from '../../src/services/taskApi';
import { TaskStatus } from '../../src/types/task';

vi.mock('../../src/services/taskApi', () => ({
  taskApi: {
    getTasks: vi.fn().mockResolvedValue([]),
    getAssignees: vi.fn().mockResolvedValue([]),
    getSwimlanes: vi.fn().mockResolvedValue([]),
    deleteTask: vi.fn().mockResolvedValue(undefined),
    createTask: vi.fn(),
    updateTask: vi.fn(),
    patchTask: vi.fn().mockResolvedValue(undefined),
    bulkDelete: vi.fn(),
    bulkMove: vi.fn(),
  },
}));

vi.mock('../../src/components/Toast', () => ({
  addToast: vi.fn(),
}));

describe('taskStore assignee filter', () => {
  beforeEach(() => {
    useTaskStore.setState({
      tasks: [],
      assigneeFilter: [],
      assigneeList: [],
      swimlaneList: [],
      selectedDate: new Date('2026-05-17'),
    });
  });

  it('keeps filter active after deleting the last matching task', async () => {
    const store = useTaskStore.getState();

    // Set filter
    store.setAssigneeFilter(['Иван']);

    // Verify filter is active
    expect(useTaskStore.getState().assigneeFilter).toEqual(['Иван']);

    // Mock delete
    await store.deleteTask(1);

    // Filter should still be active
    expect(useTaskStore.getState().assigneeFilter).toEqual(['Иван']);
  });

  it('keeps filter active after bulk deleting all tasks', async () => {
    const store = useTaskStore.getState();

    // Select tasks and set filter
    store.setAssigneeFilter(['Иван']);

    expect(useTaskStore.getState().assigneeFilter).toEqual(['Иван']);

    // Filter remains active after bulkDelete
    expect(useTaskStore.getState().assigneeFilter).toEqual(['Иван']);
  });
});

describe('moveTask uses PATCH and preserves swimlane', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    useTaskStore.setState({
      tasks: [
        {
          id: 1,
          title: 'Task 1',
          description: null,
          status: TaskStatus.New,
          date: '2026-05-17',
          order: 0,
          createdAt: '2026-05-17T00:00:00Z',
          updatedAt: '2026-05-17T00:00:00Z',
          assignee: 'Анна',
          swimlane: 'Фронтенд',
        },
      ],
      selectedDate: new Date('2026-05-17'),
      assigneeFilter: [],
      assigneeList: [],
      swimlaneList: [],
    });
  });

  it('calls patchTask with swimlane when moving a task', async () => {
    const store = useTaskStore.getState();
    (taskApi.patchTask as ReturnType<typeof vi.fn>).mockResolvedValue(undefined);

    await store.moveTask(1, TaskStatus.InProgress, 0);

    expect(taskApi.patchTask).toHaveBeenCalledWith(1, {
      status: TaskStatus.InProgress,
      order: 0,
      swimlane: 'Фронтенд',
    });
  });

  it('calls patchTask with null swimlane when task has no swimlane', async () => {
    useTaskStore.setState({
      tasks: [
        {
          id: 2,
          title: 'Task 2',
          description: null,
          status: TaskStatus.New,
          date: '2026-05-17',
          order: 0,
          createdAt: '2026-05-17T00:00:00Z',
          updatedAt: '2026-05-17T00:00:00Z',
          assignee: null,
          swimlane: null,
        },
      ],
    });

    const store = useTaskStore.getState();
    (taskApi.patchTask as ReturnType<typeof vi.fn>).mockResolvedValue(undefined);

    await store.moveTask(2, TaskStatus.Done, 0);

    expect(taskApi.patchTask).toHaveBeenCalledWith(2, {
      status: TaskStatus.Done,
      order: 0,
      swimlane: null,
    });
  });

  it('rolls back tasks on network error', async () => {
    const originalTasks = [...useTaskStore.getState().tasks];
    (taskApi.patchTask as ReturnType<typeof vi.fn>).mockRejectedValue(new Error('Network error'));

    await useTaskStore.getState().moveTask(1, TaskStatus.InProgress, 0);

    const state = useTaskStore.getState();
    expect(state.tasks).toEqual(originalTasks);
  });

  it('does not call updateTask when moving (uses patchTask instead)', async () => {
    const store = useTaskStore.getState();
    (taskApi.patchTask as ReturnType<typeof vi.fn>).mockResolvedValue(undefined);

    await store.moveTask(1, TaskStatus.InProgress, 0);

    expect(taskApi.updateTask).not.toHaveBeenCalled();
    expect(taskApi.patchTask).toHaveBeenCalled();
  });
});
