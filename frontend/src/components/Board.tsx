import { useEffect, useCallback, useMemo } from 'react';
import { DragDropContext, DropResult } from '@hello-pangea/dnd';
import { useTaskStore } from '../stores/taskStore';
import { SwimlaneRow } from './SwimlaneRow';
import { BulkActionsPanel } from './BulkActions/BulkActionsPanel';
import { TaskStatus } from '../types/task';
import { groupBySwimlane, normalizeSwimlaneKey, DEFAULT_SWIMLANE_KEY } from '../utils/swimlane';

const columns: { status: TaskStatus; title: string }[] = [
  { status: TaskStatus.New, title: 'Новые' },
  { status: TaskStatus.InProgress, title: 'В процессе' },
  { status: TaskStatus.Done, title: 'Сделаны' },
];

export function Board() {
  const {
    tasks,
    selectedDate,
    loadTasks,
    setSelectedDate,
    moveTask,
    updateTask,
    isLoading,
    error,
    assigneeFilter,
    collapsedSwimlanes,
    toggleSwimlaneCollapse,
  } = useTaskStore();

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
    const destDroppableId = result.destination.droppableId;

    // Parse composite droppableId: "{swimlaneKey}:{TaskStatus}"
    const [destSwimlaneKey, destStatusStr] = destDroppableId.split(':');
    const newStatus = Number(destStatusStr) as TaskStatus;
    const newOrder = result.destination.index;

    // Source droppableId
    const sourceDroppableId = result.source.droppableId;
    const [sourceSwimlaneKey] = sourceDroppableId.split(':');

    // Determine if swimlane changed
    if (sourceSwimlaneKey !== destSwimlaneKey) {
      // Vertical move: update swimlane + status + order in single call
      const newSwimlane = destSwimlaneKey === DEFAULT_SWIMLANE_KEY ? null : destSwimlaneKey;
      updateTask(taskId, { swimlane: newSwimlane, status: newStatus, order: newOrder });
    } else {
      // Only status/order changed
      moveTask(taskId, newStatus, newOrder);
    }
  };

  const dateTasks = useMemo(
    () => tasks.filter((task) => new Date(task.date).toDateString() === selectedDate.toDateString()),
    [tasks, selectedDate]
  );

  const swimlaneGroups = useMemo(() => groupBySwimlane(dateTasks), [dateTasks]);

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
        <div className="board board--swimlanes">
          {/* Фон столбцов — непрерывные полосы во всю высоту */}
          <div className="board-columns-bg" aria-hidden="true">
            {columns.map((column) => (
              <div key={column.status} className="board-column-bg">
                <div className="board-column-bg-header">{column.title}</div>
              </div>
            ))}
          </div>
          {/* Swimlanes поверх столбцов */}
          <div className="board-swimlanes-content">
            {swimlaneGroups.map((group) => {
              const groupKey = normalizeSwimlaneKey(group.key);
              const isCollapsed = collapsedSwimlanes.has(groupKey);

              return (
                <SwimlaneRow
                  key={groupKey}
                  swimlaneKey={groupKey}
                  displayName={group.displayName}
                  tasks={group.tasks}
                  columns={columns}
                  isCollapsed={isCollapsed}
                  onToggleCollapse={() => toggleSwimlaneCollapse(groupKey)}
                />
              );
            })}
          </div>
        </div>
      </div>
    </DragDropContext>
  );
}