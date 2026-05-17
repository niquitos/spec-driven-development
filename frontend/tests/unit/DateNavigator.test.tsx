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
    });
  });

  it('renders navigation buttons', () => {
    render(<Header />);

    expect(screen.getByLabelText('Previous day')).toBeInTheDocument();
    expect(screen.getByLabelText('Today')).toBeInTheDocument();
    expect(screen.getByLabelText('Next day')).toBeInTheDocument();
    expect(screen.getByLabelText('Select date')).toBeInTheDocument();
  });

  it('calls setSelectedDate with previous day when clicking previous button', () => {
    render(<Header />);

    fireEvent.click(screen.getByLabelText('Previous day'));

    expect(mockSetSelectedDate).toHaveBeenCalledWith(new Date('2026-05-08'));
  });

  it('calls setSelectedDate with next day when clicking next button', () => {
    render(<Header />);

    fireEvent.click(screen.getByLabelText('Next day'));

    expect(mockSetSelectedDate).toHaveBeenCalledWith(new Date('2026-05-10'));
  });

  it('calls setSelectedDate with today when clicking today button', () => {
    const today = new Date();
    render(<Header />);

    fireEvent.click(screen.getByLabelText('Today'));

    expect(mockSetSelectedDate).toHaveBeenCalledWith(today);
  });

  it('updates date when date picker value changes', () => {
    render(<Header />);

    const datePicker = screen.getByLabelText('Select date');
    fireEvent.change(datePicker, { target: { value: '2026-01-15' } });

    expect(mockSetSelectedDate).toHaveBeenCalledWith(new Date('2026-01-15'));
  });
});
