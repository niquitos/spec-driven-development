import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { EditTaskModal } from '../../src/components/TaskModal/EditTaskModal';
import { useTaskStore } from '../../src/stores/taskStore';
import { Task, TaskStatus } from '../../src/types/task';

vi.mock('../../src/stores/taskStore', () => ({
  useTaskStore: vi.fn(),
}));

describe('EditTask Integration', () => {
  const mockUpdateTask = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
    (useTaskStore as any).mockReturnValue({
      tasks: [],
      selectedDate: new Date('2026-05-09'),
      selectedTaskIds: [],
      isLoading: false,
      error: null,
      isCreateModalOpen: false,
      editingTask: null,
      updateTask: mockUpdateTask,
      setTasks: vi.fn(),
      addTask: vi.fn(),
      deleteTask: vi.fn(),
      setSelectedDate: vi.fn(),
      toggleTaskSelection: vi.fn(),
      clearSelection: vi.fn(),
      moveTask: vi.fn(),
      reorderTask: vi.fn(),
      loadTasks: vi.fn(),
      createTask: vi.fn(),
      setIsCreateModalOpen: vi.fn(),
      setEditingTask: vi.fn(),
      assigneeFilter: [],
      setAssigneeFilter: vi.fn(),
      getAssigneeList: vi.fn(() => []),
    });
  });

  const mockTask: Task = {
    id: 1,
    title: 'Original Title',
    description: 'Original Description',
    status: TaskStatus.New,
    date: '2026-05-09',
    order: 0,
    createdAt: '2026-05-09T10:00:00Z',
    updatedAt: '2026-05-09T10:00:00Z',
    assignee: null,
    swimlane: null,
  };

  const renderModal = (task: Task | null = mockTask) => {
    return render(
      <EditTaskModal
        isOpen={true}
        onClose={vi.fn()}
        task={task}
      />
    );
  };

  const getFormFields = (container: HTMLElement) => {
    return {
      title: container.querySelector('#edit-task-title') as HTMLInputElement,
      description: container.querySelector('#edit-task-description') as HTMLTextAreaElement,
      date: container.querySelector('#edit-task-date') as HTMLInputElement,
      submitButton: container.querySelector('.btn-primary') as HTMLButtonElement,
      cancelButton: container.querySelector('.btn-secondary') as HTMLButtonElement,
    };
  };

  it('renders edit modal with task data', () => {
    const { container } = renderModal();

    const { title, description, date } = getFormFields(container);
    expect(title.value).toBe('Original Title');
    expect(description.value).toBe('Original Description');
    expect(date.value).toBe('2026-05-09');
  });

  it('updates task when form is submitted', async () => {
    const { container } = renderModal();
    const { title, description, date, submitButton } = getFormFields(container);

    fireEvent.change(title, { target: { value: 'Updated Title' } });
    fireEvent.change(description, { target: { value: 'Updated Description' } });
    fireEvent.change(date, { target: { value: '2026-05-10' } });

    fireEvent.click(submitButton);

    await waitFor(() => {
      expect(mockUpdateTask).toHaveBeenCalledWith(1, {
        title: 'Updated Title',
        description: 'Updated Description',
        date: '2026-05-10',
        status: 0,
        assignee: undefined,
        swimlane: null,
      });
    });
  });

  it('calls onClose after successful update', async () => {
    const mockOnClose = vi.fn();
    mockUpdateTask.mockResolvedValueOnce(undefined);

    const { container } = render(
      <EditTaskModal
        isOpen={true}
        onClose={mockOnClose}
        task={mockTask}
      />
    );

    const { submitButton } = getFormFields(container);
    fireEvent.click(submitButton);

    await waitFor(() => {
      expect(mockOnClose).toHaveBeenCalled();
    });
  });

  it('calls onClose when clicking cancel button', () => {
    const mockOnClose = vi.fn();

    const { container } = render(
      <EditTaskModal
        isOpen={true}
        onClose={mockOnClose}
        task={mockTask}
      />
    );

    const { cancelButton } = getFormFields(container);
    fireEvent.click(cancelButton);

    expect(mockOnClose).toHaveBeenCalled();
  });

  it('does not render when not open', () => {
    const { container } = render(
      <EditTaskModal
        isOpen={false}
        onClose={vi.fn()}
        task={mockTask}
      />
    );

    expect(container.querySelector('.modal-backdrop')).toBeFalsy();
  });

  it('does not render when task is null', () => {
    const { container } = render(
      <EditTaskModal
        isOpen={true}
        onClose={vi.fn()}
        task={null}
      />
    );

    expect(container.querySelector('.modal-content')).toBeFalsy();
  });

  it('disables submit button when title is empty', () => {
    const { container } = renderModal();
    const { title, submitButton } = getFormFields(container);

    fireEvent.change(title, { target: { value: '' } });
    expect(submitButton.disabled).toBe(true);
  });

  it('enables submit button when title has value', () => {
    const { container } = renderModal();
    const { title, submitButton } = getFormFields(container);

    fireEvent.change(title, { target: { value: 'New Title' } });
    expect(submitButton.disabled).toBe(false);
  });
});
