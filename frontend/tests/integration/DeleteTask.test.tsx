import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { DeleteConfirmModal } from '../../src/components/TaskModal/DeleteConfirmModal';
import { Task, TaskStatus } from '../../src/types/task';

describe('DeleteConfirmModal Integration', () => {
  const mockOnClose = vi.fn();
  const mockOnConfirm = vi.fn();

  const mockTask: Task = {
    id: 1,
    title: 'Task to Delete',
    description: 'Description',
    status: TaskStatus.New,
    date: '2026-05-09',
    order: 0,
    createdAt: '2026-05-09T10:00:00Z',
    updatedAt: '2026-05-09T10:00:00Z',
  };

  beforeEach(() => {
    vi.clearAllMocks();
  });

  const renderModal = (task: Task | null = mockTask) => {
    return render(
      <DeleteConfirmModal
        isOpen={true}
        onClose={mockOnClose}
        onConfirm={mockOnConfirm}
        task={task}
      />
    );
  };

  const getButtons = (container: HTMLElement) => {
    return {
      cancelButton: container.querySelector('.btn-secondary') as HTMLButtonElement,
      deleteButton: container.querySelector('.btn-danger') as HTMLButtonElement,
    };
  };

  it('renders delete confirmation modal with task title', () => {
    const { container } = renderModal();

    expect(container.querySelector('.modal-content')).toBeTruthy();
    expect(container.querySelector('.modal-title')?.textContent).toBe('Удаление задачи');
    expect(container.querySelector('.delete-message')).toBeTruthy();
  });

  it('shows warning message', () => {
    const { container } = renderModal();

    expect(container.querySelector('.delete-warning')).toBeTruthy();
  });

  it('calls onClose when clicking cancel button', () => {
    const { container } = renderModal();
    const { cancelButton } = getButtons(container);

    fireEvent.click(cancelButton);
    expect(mockOnClose).toHaveBeenCalled();
  });

  it('calls onConfirm and onClose when clicking delete button', () => {
    const { container } = renderModal();
    const { deleteButton } = getButtons(container);

    fireEvent.click(deleteButton);
    expect(mockOnConfirm).toHaveBeenCalled();
    expect(mockOnClose).toHaveBeenCalled();
  });

  it('does not render when not open', () => {
    const { container } = render(
      <DeleteConfirmModal
        isOpen={false}
        onClose={mockOnClose}
        onConfirm={mockOnConfirm}
        task={mockTask}
      />
    );

    expect(container.querySelector('.modal-backdrop')).toBeFalsy();
  });

  it('does not render when task is null', () => {
    const { container } = render(
      <DeleteConfirmModal
        isOpen={true}
        onClose={mockOnClose}
        onConfirm={mockOnConfirm}
        task={null}
      />
    );

    expect(container.querySelector('.modal-content')).toBeFalsy();
  });

  it('closes when clicking backdrop', () => {
    const { container } = renderModal();
    const backdrop = container.querySelector('.modal-backdrop');

    if (backdrop) {
      fireEvent.click(backdrop);
    }

    expect(mockOnClose).toHaveBeenCalled();
  });

  it('has proper aria attributes for accessibility', () => {
    const { container } = renderModal();
    const dialog = container.querySelector('[role="alertdialog"]');

    expect(dialog).toBeTruthy();
    expect(dialog?.getAttribute('aria-modal')).toBe('true');
    expect(dialog?.getAttribute('aria-labelledby')).toBe('delete-modal-title');
  });
});
