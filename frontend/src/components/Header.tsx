import { useTaskStore } from '../stores/taskStore';
import { formatDate } from '../utils/date';

interface HeaderProps {
  onNavigate?: (date: Date) => void;
}

export function Header({ onNavigate }: HeaderProps) {
  const { selectedDate, setSelectedDate } = useTaskStore();

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

  return (
    <header className="header">
      <button onClick={handlePrevDay} aria-label="Previous day">
        ←
      </button>
      <button onClick={handleToday} aria-label="Today">
        Today
      </button>
      <div className="date-display">
        <input
          type="date"
          value={formatDate(selectedDate, 'yyyy-MM-dd')}
          onChange={handleDateChange}
          aria-label="Select date"
        />
        <span className="date-label">{formatDate(selectedDate, 'EEEE, MMMM d')}</span>
      </div>
      <button onClick={handleNextDay} aria-label="Next day">
        →
      </button>
    </header>
  );
}
