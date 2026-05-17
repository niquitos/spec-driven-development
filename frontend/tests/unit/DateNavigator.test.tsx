import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { Header } from '../../src/components/Header';
import { useTaskStore } from '../../src/stores/taskStore';

// Mock the store
vi.mock('../../src/stores/taskStore', () => ({
  useTaskStore: vi.fn(),
}));

describe('DateNavigator', () => {
  const mockSetSelectedDate = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
    (useTaskStore as any).mockReturnValue({
      selectedDate: new Date('2026-05-09'),
      setSelectedDate: mockSetSelectedDate,
      assigneeFilter: [],
      setAssigneeFilter: vi.fn(),
      getAssigneeList: vi.fn(() => []),
    });
  });

  it('renders navigation buttons', () => {
    render(<Header />);

    expect(screen.getByLabelText('Предыдущий день')).toBeInTheDocument();
    expect(screen.getByLabelText('Сегодня')).toBeInTheDocument();
    expect(screen.getByLabelText('Следующий день')).toBeInTheDocument();
    expect(screen.getByLabelText('Выбрать дату')).toBeInTheDocument();
  });

  it('calls setSelectedDate with previous day when clicking previous button', () => {
    render(<Header />);

    fireEvent.click(screen.getByLabelText('Предыдущий день'));

    expect(mockSetSelectedDate).toHaveBeenCalledWith(new Date('2026-05-08'));
  });

  it('calls setSelectedDate with next day when clicking next button', () => {
    render(<Header />);

    fireEvent.click(screen.getByLabelText('Следующий день'));

    expect(mockSetSelectedDate).toHaveBeenCalledWith(new Date('2026-05-10'));
  });

  it('calls setSelectedDate with today when clicking today button', () => {
    const before = Date.now();
    render(<Header />);

    fireEvent.click(screen.getByLabelText('Сегодня'));

    const after = Date.now();
    expect(mockSetSelectedDate).toHaveBeenCalled();
    const actualCall = mockSetSelectedDate.mock.calls[0][0];
    expect(actualCall.getTime()).toBeGreaterThanOrEqual(before - 1);
    expect(actualCall.getTime()).toBeLessThanOrEqual(after + 1);
  });

  it('updates date when date picker value changes', () => {
    render(<Header />);

    const datePicker = screen.getByLabelText('Выбрать дату');
    fireEvent.change(datePicker, { target: { value: '2026-01-15' } });

    expect(mockSetSelectedDate).toHaveBeenCalledWith(new Date('2026-01-15'));
  });
});
