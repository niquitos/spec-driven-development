import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { Column } from '../../src/components/Column';
import { useTaskStore } from '../../src/stores/taskStore';
import { TaskStatus } from '../../src/types/task';
import { CreateTaskModal } from '../../src/components/TaskModal/CreateTaskModal';

vi.mock('../../src/stores/taskStore', () => ({
  useTaskStore: vi.fn(),
}));

describe('CreateTask Integration', () => {
  const mockCreateTask = vi.fn();
  const mockSetIsCreateModalOpen = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
    (useTaskStore as any).mockReturnValue({
      tasks: [],
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
      toggleTaskSelection: vi.fn(),
      clearSelection: vi.fn(),
      moveTask: vi.fn(),
      reorderTask: vi.fn(),
      loadTasks: vi.fn(),
      createTask: mockCreateTask,
      setIsCreateModalOpen: mockSetIsCreateModalOpen,
      assigneeFilter: [],
      setAssigneeFilter: vi.fn(),
      getAssigneeList: vi.fn(() => []),
    });
  });

  const renderModal = () => {
    return render(
      <CreateTaskModal
        isOpen={true}
        onClose={vi.fn()}
        defaultDate={new Date('2026-05-09')}
        defaultStatus={TaskStatus.New}
      />
    );
  };

  const getFormFields = (container: HTMLElement) => {
    const modal = container.querySelector('.modal-content')!;
    return {
      title: modal.querySelector('#task-title') as HTMLInputElement,
      description: modal.querySelector('#task-description') as HTMLTextAreaElement,
      date: modal.querySelector('#task-date') as HTMLInputElement,
      submitButton: modal.querySelector('.btn-primary') as HTMLButtonElement,
      cancelButton: modal.querySelector('.btn-secondary') as HTMLButtonElement,
    };
  };

  it('opens create modal when clicking + button', () => {
    const { container } = render(<Column status={TaskStatus.New} title="Новые" tasks={[]} />);

    const addButton = container.querySelector('button[aria-label="Add task to Новые"]');
    if (addButton) {
      fireEvent.click(addButton);
    }

    expect(container.querySelector('.modal-content')).toBeTruthy();
    expect(container.querySelector('.modal-title')).toBeTruthy();
  });

  it('renders all form fields in create modal', () => {
    const { container } = renderModal();

    expect(container.querySelector('#task-title')).toBeTruthy();
    expect(container.querySelector('#task-description')).toBeTruthy();
    expect(container.querySelector('#task-date')).toBeTruthy();
    expect(container.querySelector('.btn-secondary')).toBeTruthy();
    expect(container.querySelector('.btn-primary')).toBeTruthy();
  });

  it('submits form with valid data', async () => {
    const { container } = renderModal();
    const { title, description, date, submitButton } = getFormFields(container);

    fireEvent.change(title, { target: { value: 'Test Task' } });
    fireEvent.change(description, { target: { value: 'Test Description' } });
    fireEvent.change(date, { target: { value: '2026-05-09' } });

    fireEvent.click(submitButton);

    await waitFor(() => {
      expect(mockCreateTask).toHaveBeenCalledWith({
        title: 'Test Task',
        description: 'Test Description',
        date: '2026-05-09',
        status: TaskStatus.New,
        order: 0,
        assignee: undefined,
      });
    });
  });

  it('closes modal after successful creation', async () => {
    const mockOnClose = vi.fn();
    mockCreateTask.mockResolvedValueOnce(undefined);

    const { container } = render(
      <CreateTaskModal
        isOpen={true}
        onClose={mockOnClose}
        defaultDate={new Date('2026-05-09')}
        defaultStatus={TaskStatus.New}
      />
    );

    const { title, date, submitButton } = getFormFields(container);

    fireEvent.change(title, { target: { value: 'Test Task' } });
    fireEvent.change(date, { target: { value: '2026-05-09' } });

    fireEvent.click(submitButton);

    await waitFor(() => {
      expect(mockOnClose).toHaveBeenCalled();
    });
  });

  it('calls onClose when clicking cancel button', () => {
    const mockOnClose = vi.fn();

    const { container } = render(
      <CreateTaskModal
        isOpen={true}
        onClose={mockOnClose}
        defaultDate={new Date('2026-05-09')}
        defaultStatus={TaskStatus.New}
      />
    );

    const { cancelButton } = getFormFields(container);
    fireEvent.click(cancelButton);

    expect(mockOnClose).toHaveBeenCalledTimes(1);
  });

  it('closes modal when clicking backdrop', () => {
    const mockOnClose = vi.fn();

    const { container } = render(
      <CreateTaskModal
        isOpen={true}
        onClose={mockOnClose}
        defaultDate={new Date('2026-05-09')}
        defaultStatus={TaskStatus.New}
      />
    );

    const backdrop = container.querySelector('.modal-backdrop');
    if (backdrop) {
      fireEvent.click(backdrop);
    }

    expect(mockOnClose).toHaveBeenCalled();
  });

});
