import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { Board } from '../../src/components/Board';
import { BulkActionsPanel } from '../../src/components/BulkActions/BulkActionsPanel';
import { useTaskStore } from '../../src/stores/taskStore';
import { TaskStatus } from '../../src/types/task';

vi.mock('../../src/stores/taskStore', () => ({
  useTaskStore: vi.fn(),
}));

vi.mock('../../src/services/taskApi', () => ({
  default: {
    getTasks: vi.fn().mockResolvedValue([]),
    createTask: vi.fn(),
    updateTask: vi.fn(),
    deleteTask: vi.fn(),
    bulkDelete: vi.fn(),
    bulkMove: vi.fn(),
  },
}));

describe('BulkActions Integration', () => {
  const mockBulkDelete = vi.fn();
  const mockBulkMove = vi.fn();
  const mockToggleTaskSelection = vi.fn();
  const mockClearSelection = vi.fn();

  const mockTasks = [
    {
      id: '1',
      title: 'Task 1',
      description: 'Description 1',
      status: TaskStatus.New,
      date: '2026-05-09',
      order: 0,
      createdAt: '2026-05-09T10:00:00Z',
      updatedAt: null,
    },
    {
      id: '2',
      title: 'Task 2',
      description: 'Description 2',
      status: TaskStatus.New,
      date: '2026-05-09',
      order: 1,
      createdAt: '2026-05-09T10:00:00Z',
      updatedAt: null,
    },
    {
      id: '3',
      title: 'Task 3',
      description: 'Description 3',
      status: TaskStatus.InProgress,
      date: '2026-05-09',
      order: 0,
      createdAt: '2026-05-09T10:00:00Z',
      updatedAt: null,
    },
  ];

  beforeEach(() => {
    vi.clearAllMocks();
    window.confirm = vi.fn(() => true);
    (useTaskStore as any).mockReturnValue({
      tasks: mockTasks,
      selectedDate: new Date('2026-05-09'),
      selectedTaskIds: ['1', '2'],
      isLoading: false,
      error: null,
      isCreateModalOpen: false,
      setTasks: vi.fn(),
      addTask: vi.fn(),
      updateTask: vi.fn(),
      deleteTask: vi.fn(),
      setSelectedDate: vi.fn(),
      toggleTaskSelection: mockToggleTaskSelection,
      clearSelection: mockClearSelection,
      moveTask: vi.fn(),
      reorderTask: vi.fn(),
      loadTasks: vi.fn(),
      createTask: vi.fn(),
      setIsCreateModalOpen: vi.fn(),
      bulkDelete: mockBulkDelete,
      bulkMove: mockBulkMove,
      assigneeFilter: [],
      setAssigneeFilter: vi.fn(),
      getAssigneeList: vi.fn(() => []),
    });
  });

  it('should display BulkActionsPanel when tasks are selected', () => {
    render(<BulkActionsPanel />);

    expect(screen.getByText(/Выбрано:/)).toBeInTheDocument();
  });

  it('should show delete button in bulk actions panel', () => {
    render(<BulkActionsPanel />);

    expect(screen.getByRole('button', { name: /Удалить/i })).toBeInTheDocument();
  });

  it('should show move button in bulk actions panel', () => {
    render(<BulkActionsPanel />);

    expect(screen.getByRole('button', { name: /Переместить/i })).toBeInTheDocument();
  });

  it('should call bulkDelete when delete button is clicked', async () => {
    render(<BulkActionsPanel />);

    const deleteButton = screen.getByRole('button', { name: /Удалить/i });
    fireEvent.click(deleteButton);

    await waitFor(() => {
      expect(mockBulkDelete).toHaveBeenCalled();
    });
  });

  it('should call bulkMove when move confirm button is clicked', async () => {
    render(<BulkActionsPanel />);

    const moveButton = screen.getByRole('button', { name: /Переместить/i });
    fireEvent.click(moveButton);

    const confirmButton = screen.getByRole('button', { name: /Переместить.*задач/i });
    fireEvent.click(confirmButton);

    await waitFor(() => {
      expect(mockBulkMove).toHaveBeenCalled();
    });
  });

  it('should call clearSelection when cancel button is clicked', async () => {
    render(<BulkActionsPanel />);

    const cancelButton = screen.getByRole('button', { name: /Отменить выбор/i });
    fireEvent.click(cancelButton);

    await waitFor(() => {
      expect(mockClearSelection).toHaveBeenCalled();
    });
  });

  it('should not display BulkActionsPanel when no tasks are selected', () => {
    (useTaskStore as any).mockReturnValue({
      tasks: mockTasks,
      selectedDate: new Date('2026-05-09'),
      selectedTaskIds: [],
      isLoading: false,
      error: null,
      isCreateModalOpen: false,
      setTasks: vi.fn(),
      addTask: vi.fn(),
      updateTask: vi.fn(),
      deleteTask: vi.fn(),
      setSelectedDate: vi.fn(),
      toggleTaskSelection: mockToggleTaskSelection,
      clearSelection: mockClearSelection,
      moveTask: vi.fn(),
      reorderTask: vi.fn(),
      loadTasks: vi.fn(),
      createTask: vi.fn(),
      setIsCreateModalOpen: vi.fn(),
      bulkDelete: mockBulkDelete,
      bulkMove: mockBulkMove,
      assigneeFilter: [],
      setAssigneeFilter: vi.fn(),
      getAssigneeList: vi.fn(() => []),
    });

    const { container } = render(<BulkActionsPanel />);

    expect(container.firstChild).toBeNull();
  });

  it('should select multiple tasks via checkboxes', async () => {
    render(
      <div>
        <Board />
      </div>
    );

    // Simulate clicking checkboxes for tasks
    const checkboxes = screen.getAllByRole('checkbox');
    fireEvent.click(checkboxes[0]);
    fireEvent.click(checkboxes[1]);

    await waitFor(() => {
      expect(mockToggleTaskSelection).toHaveBeenCalledTimes(2);
    });
  });

  it('should display count of selected tasks', () => {
    render(<BulkActionsPanel />);

    expect(screen.getByText(/Выбрано:/)).toBeInTheDocument();
  });
});
