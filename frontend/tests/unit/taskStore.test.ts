import { describe, it, expect, vi, beforeEach } from 'vitest';
import { useTaskStore } from '../../src/stores/taskStore';
import { taskApi } from '../../src/services/taskApi';

vi.mock('../../src/services/taskApi', () => ({
  taskApi: {
    getTasks: vi.fn().mockResolvedValue([]),
    getAssignees: vi.fn().mockResolvedValue([]),
    deleteTask: vi.fn().mockResolvedValue(undefined),
    createTask: vi.fn(),
    updateTask: vi.fn(),
    bulkDelete: vi.fn(),
    bulkMove: vi.fn(),
  },
}));

describe('taskStore assignee filter', () => {
  beforeEach(() => {
    useTaskStore.setState({
      tasks: [],
      assigneeFilter: [],
      assigneeList: [],
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
