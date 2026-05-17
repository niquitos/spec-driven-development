import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { AssigneeFilter } from '../../src/components/AssigneeFilter';
import { useTaskStore } from '../../src/stores/taskStore';

vi.mock('../../src/stores/taskStore', () => ({
  useTaskStore: vi.fn(),
}));

describe('AssigneeFilter', () => {
  const mockSetAssigneeFilter = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
    (useTaskStore as any).mockReturnValue({
      assigneeFilter: [],
      assigneeList: ['Иван', 'Петр', 'Мария'],
      setAssigneeFilter: mockSetAssigneeFilter,
    });
  });

  it('renders filter button', () => {
    render(<AssigneeFilter />);

    expect(screen.getByLabelText('Фильтр по исполнителям')).toBeInTheDocument();
    expect(screen.getByText('Исполнители')).toBeInTheDocument();
  });

  it('shows dropdown when button is clicked', () => {
    render(<AssigneeFilter />);

    const button = screen.getByLabelText('Фильтр по исполнителям');
    fireEvent.click(button);

    expect(screen.getByLabelText('Список исполнителей')).toBeInTheDocument();
  });

  it('displays assignee list in dropdown', () => {
    render(<AssigneeFilter />);

    fireEvent.click(screen.getByLabelText('Фильтр по исполнителям'));

    expect(screen.getByLabelText('Фильтр: Иван')).toBeInTheDocument();
    expect(screen.getByLabelText('Фильтр: Петр')).toBeInTheDocument();
    expect(screen.getByLabelText('Фильтр: Мария')).toBeInTheDocument();
  });

  it('shows "Нет исполнителей" when list is empty', () => {
    (useTaskStore as any).mockReturnValue({
      assigneeFilter: [],
      assigneeList: [],
      setAssigneeFilter: mockSetAssigneeFilter,
    });
    render(<AssigneeFilter />);

    fireEvent.click(screen.getByLabelText('Фильтр по исполнителям'));

    expect(screen.getByText('Нет исполнителей')).toBeInTheDocument();
  });

  it('calls toggleAssignee when checkbox is clicked', () => {
    render(<AssigneeFilter />);

    fireEvent.click(screen.getByLabelText('Фильтр по исполнителям'));
    fireEvent.click(screen.getByLabelText('Фильтр: Иван'));

    expect(mockSetAssigneeFilter).toHaveBeenCalledWith(['Иван']);
  });

  it('removes assignee from filter when already selected', () => {
    (useTaskStore as any).mockReturnValue({
      assigneeFilter: ['Иван'],
      setAssigneeFilter: mockSetAssigneeFilter,
      assigneeList: ['Иван', 'Петр', 'Мария'],
    });
    render(<AssigneeFilter />);

    fireEvent.click(screen.getByLabelText('Фильтр по исполнителям'));
    fireEvent.click(screen.getByLabelText('Фильтр: Иван'));

    expect(mockSetAssigneeFilter).toHaveBeenCalledWith([]);
  });

  it('shows active indicator when filter is active', () => {
    (useTaskStore as any).mockReturnValue({
      assigneeFilter: ['Иван'],
      setAssigneeFilter: mockSetAssigneeFilter,
      assigneeList: ['Иван', 'Петр', 'Мария'],
    });
    render(<AssigneeFilter />);

    // The badge showing count should be visible
    expect(screen.getByText('1')).toBeInTheDocument();
  });

  it('shows clear filter button when filter is active', () => {
    (useTaskStore as any).mockReturnValue({
      assigneeFilter: ['Иван'],
      setAssigneeFilter: mockSetAssigneeFilter,
      assigneeList: ['Иван', 'Петр', 'Мария'],
    });
    render(<AssigneeFilter />);

    fireEvent.click(screen.getByLabelText('Фильтр по исполнителям'));

    const clearButton = screen.getByLabelText('Сбросить фильтр');
    expect(clearButton).toBeInTheDocument();
  });

  it('calls clearFilter when reset button is clicked', () => {
    (useTaskStore as any).mockReturnValue({
      assigneeFilter: ['Иван', 'Петр'],
      setAssigneeFilter: mockSetAssigneeFilter,
      assigneeList: ['Иван', 'Петр', 'Мария'],
    });
    render(<AssigneeFilter />);

    fireEvent.click(screen.getByLabelText('Фильтр по исполнителям'));
    fireEvent.click(screen.getByLabelText('Сбросить фильтр'));

    expect(mockSetAssigneeFilter).toHaveBeenCalledWith([]);
  });

  it('closes dropdown when clicking outside', () => {
    render(<AssigneeFilter />);

    fireEvent.click(screen.getByLabelText('Фильтр по исполнителям'));
    expect(screen.getByLabelText('Список исполнителей')).toBeInTheDocument();

    // Click outside
    fireEvent.mouseDown(document.body);

    expect(screen.queryByLabelText('Список исполнителей')).not.toBeInTheDocument();
  });
});
