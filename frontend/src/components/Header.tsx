import { useTaskStore } from '../stores/taskStore';
import { formatDate } from '../utils/date';
import { AssigneeFilter } from './AssigneeFilter';

interface HeaderProps {
  onNavigate?: (date: Date) => void;
}

export function Header({ onNavigate }: HeaderProps) {
  const { selectedDate, setSelectedDate, isMovingToTomorrow, moveIncompleteToTomorrow } = useTaskStore();

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

  const handleMoveToTomorrow = () => {
    moveIncompleteToTomorrow();
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
        onClick={handleMoveToTomorrow}
        className="btn btn-move-tomorrow"
        disabled={isMovingToTomorrow}
        aria-label="Перенести невыполненные задачи на завтра"
      >
        {isMovingToTomorrow ? 'Перенос...' : '→ Завтра'}
      </button>
      <AssigneeFilter />
    </header>
  );
}