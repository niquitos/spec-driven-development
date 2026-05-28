import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen } from '@testing-library/react';
import { DragDropContext } from '@hello-pangea/dnd';
import { Column } from '../../src/components/Column';
import { TaskStatus } from '../../src/types/task';
import { useTaskStore } from '../../src/stores/taskStore';

vi.mock('../../src/stores/taskStore', () => ({
  useTaskStore: vi.fn(),
}));

const renderWithDnD = (ui: React.ReactElement) => {
  return render(
    <DragDropContext onDragEnd={() => {}}>
      {ui}
    </DragDropContext>
  );
};

describe('DragDrop Integration', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    (useTaskStore as any).mockReturnValue({
      tasks: [],
      selectedDate: new Date('2026-05-09'),
      selectedTaskIds: [],
      isLoading: false,
      error: null,
      isCreateModalOpen: false,
      isDeleteModalOpen: false,
      editingTask: null,
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
      createTask: vi.fn(),
      setIsCreateModalOpen: vi.fn(),
      setEditingTask: vi.fn(),
      assigneeFilter: [],
      setAssigneeFilter: vi.fn(),
      getAssigneeList: vi.fn(() => []),
      swimlaneList: [],
      collapsedSwimlanes: new Set(),
      toggleSwimlaneCollapse: vi.fn(),
    });
  });

  it('renders column with droppable area', () => {
    renderWithDnD(<Column status={TaskStatus.New} title="Новые" tasks={[]} />);

    const column = document.querySelector('.column');
    expect(column).toBeTruthy();
  });

  it('renders column with add button', () => {
    renderWithDnD(<Column status={TaskStatus.New} title="Новые" tasks={[]} />);

    const addButton = screen.getByRole('button', { name: /Добавить задачу/ });
    expect(addButton).toBeTruthy();
  });
});
