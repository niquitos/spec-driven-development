import { useEffect, useCallback } from 'react';
import { useTaskStore } from '../stores/taskStore';
import { Column } from './Column';
import { TaskStatus } from '../types/task';

const columns: { status: TaskStatus; title: string }[] = [
  { status: TaskStatus.New, title: 'Новые' },
  { status: TaskStatus.InProgress, title: 'В процессе' },
  { status: TaskStatus.Done, title: 'Сделаны' },
];

export function Board() {
  const { tasks, selectedDate, loadTasks, setSelectedDate } = useTaskStore();

  useEffect(() => {
    loadTasks(selectedDate);
  }, [selectedDate, loadTasks]);

  // Keyboard navigation for dates (ArrowLeft/ArrowRight)
  const handleKeyDown = useCallback((event: KeyboardEvent) => {
    if (event.target instanceof HTMLInputElement || event.target instanceof HTMLButton) {
      return; // Don't interfere with input focus
    }

    if (event.key === 'ArrowLeft') {
      const newDate = new Date(selectedDate);
      newDate.setDate(newDate.getDate() - 1);
      setSelectedDate(newDate);
    } else if (event.key === 'ArrowRight') {
      const newDate = new Date(selectedDate);
      newDate.setDate(newDate.getDate() + 1);
      setSelectedDate(newDate);
    }
  }, [selectedDate, setSelectedDate]);

  useEffect(() => {
    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [handleKeyDown]);

  const dateTasks = tasks.filter(
    (task) => new Date(task.date).toDateString() === selectedDate.toDateString()
  );

  return (
    <div className="board">
      {columns.map((column) => (
        <Column
          key={column.status}
          status={column.status}
          title={column.title}
          tasks={dateTasks.filter((t) => t.status === column.status)}
        />
      ))}
    </div>
  );
}
