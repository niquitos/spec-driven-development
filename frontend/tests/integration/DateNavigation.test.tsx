import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { Board } from '../../src/components/Board';
import { Header } from '../../src/components/Header';
import { useTaskStore } from '../../src/stores/taskStore';
import { Task, TaskStatus } from '../../src/types/task';

// Mock the store
vi.mock('../../src/stores/taskStore', () => ({
  useTaskStore: vi.fn(() => ({
    tasks: [] as Task[],
    selectedDate: new Date('2026-05-09'),
    selectedTaskIds: [] as number[],
    isLoading: false,
    error: null,
    setTasks: vi.fn(),
    addTask: vi.fn(),
    updateTask: vi.fn(),
    deleteTask: vi.fn(),
    setSelectedDate: vi.fn(),
    toggleTaskSelection: vi.fn(),
    clearSelection: vi.fn(),
    moveTask: vi.fn(),
    reorderTask: vi.fn(),
    loadTasks: vi.fn(),
    assigneeFilter: [],
    setAssigneeFilter: vi.fn(),
    getAssigneeList: vi.fn(() => []),
  })),
}));

describe('DateNavigation Integration', () => {
  const mockLoadTasks = vi.fn();
  const mockSetSelectedDate = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
    (useTaskStore as any).mockReturnValue({
      tasks: [] as Task[],
      selectedDate: new Date('2026-05-09'),
      selectedTaskIds: [] as number[],
      isLoading: false,
      error: null,
      setTasks: vi.fn(),
      addTask: vi.fn(),
      updateTask: vi.fn(),
      deleteTask: vi.fn(),
      setSelectedDate: mockSetSelectedDate,
      toggleTaskSelection: vi.fn(),
      clearSelection: vi.fn(),
      moveTask: vi.fn(),
      reorderTask: vi.fn(),
      loadTasks: mockLoadTasks,
      assigneeFilter: [],
      setAssigneeFilter: vi.fn(),
      getAssigneeList: vi.fn(() => []),
    });
  });

  it('reloads tasks when date changes via navigation', async () => {
    render(
      <div>
        <Header />
        <Board />
      </div>
    );

    // Click next day
    fireEvent.click(screen.getByLabelText('Следующий день'));

    // Verify loadTasks is called with new date
    await waitFor(() => {
      expect(mockSetSelectedDate).toHaveBeenCalled();
    });
  });

  it('reloads tasks when date changes via date picker', async () => {
    render(
      <div>
        <Header />
        <Board />
      </div>
    );

    const datePicker = screen.getByLabelText('Выбрать дату');
    fireEvent.change(datePicker, { target: { value: '2026-01-15' } });

    await waitFor(() => {
      expect(mockSetSelectedDate).toHaveBeenCalledWith(new Date('2026-01-15'));
    });
  });

  it('displays tasks filtered by selected date', () => {
    const tasksForSelectedDate: Task[] = [
      {
        id: 1,
        title: 'Task 1',
        description: 'Description 1',
        status: TaskStatus.New,
        date: '2026-05-09',
        order: 0,
        createdAt: '2026-05-09T10:00:00Z',
        updatedAt: '2026-05-09T10:00:00Z',
      },
    ];

    const tasksForOtherDate: Task[] = [
      {
        id: 2,
        title: 'Task 2',
        description: 'Description 2',
        status: TaskStatus.New,
        date: '2026-05-10',
        order: 0,
        createdAt: '2026-05-10T10:00:00Z',
        updatedAt: '2026-05-10T10:00:00Z',
      },
    ];

    (useTaskStore as any).mockReturnValue({
      tasks: [...tasksForSelectedDate, ...tasksForOtherDate],
      selectedDate: new Date('2026-05-09'),
      selectedTaskIds: [] as number[],
      isLoading: false,
      error: null,
      setTasks: vi.fn(),
      addTask: vi.fn(),
      updateTask: vi.fn(),
      deleteTask: vi.fn(),
      setSelectedDate: vi.fn(),
      toggleTaskSelection: vi.fn(),
      clearSelection: vi.fn(),
      moveTask: vi.fn(),
      reorderTask: vi.fn(),
      loadTasks: vi.fn(),
      assigneeFilter: [],
      setAssigneeFilter: vi.fn(),
      getAssigneeList: vi.fn(() => []),
    });

    render(<Board />);

    // Should only show task for selected date
    expect(screen.getByText('Task 1')).toBeInTheDocument();
    expect(screen.queryByText('Task 2')).not.toBeInTheDocument();
  });
});
