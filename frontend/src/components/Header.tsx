import { useRef } from 'react';
import { useTaskStore } from '../stores/taskStore';
import { formatDate } from '../utils/date';
import { AssigneeFilter } from './AssigneeFilter';

interface HeaderProps {
  onNavigate?: (date: Date) => void;
}

export function Header({ onNavigate }: HeaderProps) {
  const { selectedDate, setSelectedDate, isMovingIncomplete, moveIncompleteToDate } = useTaskStore();
  const hiddenDateInputRef = useRef<HTMLInputElement>(null);

  const handlePrevDay = () => {
    const newDate = new Date(selectedDate);
    newDate.setDate(newDate.getDate() - 1);
    setSelectedDate(newDate);
    onNavigate?.(newDate);
  };

  const handleNextDay = () => {
    const newDate = new Date(selectedDate);
    newDate.setDate(newDate.getDate() + 1);
    setSelectedDate(newDate);
    onNavigate?.(newDate);
  };

  const handleToday = () => {
    const today = new Date();
    setSelectedDate(today);
    onNavigate?.(today);
  };

  const handleDateChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newDate = new Date(e.target.value);
    setSelectedDate(newDate);
    onNavigate?.(newDate);
  };

  const handleMoveButtonClick = () => {
    hiddenDateInputRef.current?.showPicker?.() ?? hiddenDateInputRef.current?.click();
  };

  const handleMoveDateChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (!e.target.value) return;
    const targetDate = new Date(e.target.value);
    moveIncompleteToDate(targetDate);
    e.target.value = '';
  };

  return (
    <header className="header">
      <button onClick={handlePrevDay} aria-label="Предыдущий день">
        &larr;
      </button>
      <button onClick={handleToday} aria-label="Сегодня">
        Сегодня
      </button>
      <div className="date-display">
        <input
          type="date"
          value={formatDate(selectedDate, 'yyyy-MM-dd')}
          onChange={handleDateChange}
          aria-label="Выбрать дату"
        />
        <span className="date-label">{formatDate(selectedDate, 'EEEE, MMMM d')}</span>
      </div>
      <button onClick={handleNextDay} aria-label="Следующий день">
        &rarr;
      </button>

      <button
        onClick={handleMoveButtonClick}
        className="btn btn-move-tomorrow"
        disabled={isMovingIncomplete}
        aria-label="Переместить несделанные задачи на другую дату"
      >
        {isMovingIncomplete ? 'Перенос...' : 'Переместить несделанные'}
      </button>

      <input
        ref={hiddenDateInputRef}
        type="date"
        onChange={handleMoveDateChange}
        aria-hidden="true"
        style={{
          position: 'absolute',
          opacity: 0,
          pointerEvents: 'none',
          width: 0,
          height: 0,
          border: 'none',
          padding: 0,
          margin: 0,
        }}
      />

      <AssigneeFilter />
    </header>
  );
}
