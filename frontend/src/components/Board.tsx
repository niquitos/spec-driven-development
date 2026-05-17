import { useEffect, useCallback } from 'react';
import { DragDropContext, DropResult } from '@hello-pangea/dnd';
import { useTaskStore } from '../stores/taskStore';
import { Column } from './Column';
import { BulkActionsPanel } from './BulkActions/BulkActionsPanel';
import { TaskStatus } from '../types/task';

const columns: { status: TaskStatus; title: string }[] = [
  { status: TaskStatus.New, title: 'Новые' },
  { status: TaskStatus.InProgress, title: 'В процессе' },
  { status: TaskStatus.Done, title: 'Сделаны' },
];

export function Board() {
  const { tasks, selectedDate, loadTasks, setSelectedDate, moveTask, isLoading, error, assigneeFilter } = useTaskStore();

  // Загружаем задачи при изменении даты
  useEffect(() => {
    loadTasks(selectedDate);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [selectedDate]);

  const handleKeyDown = useCallback((event: KeyboardEvent) => {
    if (event.target instanceof HTMLInputElement || event.target instanceof HTMLButtonElement) {
      return;
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

  const handleDragEnd = (result: DropResult) => {
    if (!result.destination) return;

    const taskId = Number(result.draggableId);
    const newStatus = Number(result.destination.droppableId) as TaskStatus;
    const newOrder = result.destination.index;

    moveTask(taskId, newStatus, newOrder);
  };

  const dateTasks = tasks.filter(
    (task) => new Date(task.date).toDateString() === selectedDate.toDateString()
  );

  if (isLoading) {
    return (
      <div className="board-container">
        <div className="loading-state">
          <div className="loading-spinner"></div>
          <p>Загрузка задач...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="board-container">
        <div className="error-state">
          <p className="error-message">{error}</p>
          <button onClick={() => loadTasks(selectedDate)}>Попробовать снова</button>
        </div>
      </div>
    );
  }

  return (
    <DragDropContext onDragEnd={handleDragEnd}>
      <div className="board-container">
        <BulkActionsPanel />
        {assigneeFilter.length > 0 && dateTasks.length === 0 && (
          <div className="empty-filter-state">
            <p>Нет задач, соответствующих фильтру</p>
          </div>
        )}
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
      </div>
    </DragDropContext>
  );
}
