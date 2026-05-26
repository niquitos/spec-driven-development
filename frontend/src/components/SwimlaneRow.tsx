import { Droppable } from '@hello-pangea/dnd';
import { Task, TaskStatus } from '../types/task';
import { SwimlaneHeader } from './SwimlaneHeader';
import { TaskCard } from './TaskCard';
import { useTaskStore } from '../stores/taskStore';
import { useState } from 'react';
import { CreateTaskModal } from './TaskModal/CreateTaskModal';
import { DEFAULT_SWIMLANE_KEY } from '../utils/swimlane';

interface SwimlaneRowProps {
  swimlaneKey: string;
  displayName: string;
  tasks: Task[];
  columns: { status: TaskStatus; title: string }[];
  isCollapsed: boolean;
  onToggleCollapse: () => void;
}

export function SwimlaneRow({ swimlaneKey, displayName, tasks, columns, isCollapsed, onToggleCollapse }: SwimlaneRowProps) {
  const { selectedDate } = useTaskStore();
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [createStatus, setCreateStatus] = useState<TaskStatus>(TaskStatus.New);

  const taskCount = tasks.length;
  const contentId = `swimlane-content-${swimlaneKey}`;

  const tasksByStatus = (status: TaskStatus) =>
    tasks.filter(t => t.status === status).sort((a, b) => a.order - b.order);

  const openCreateModal = (status: TaskStatus) => {
    setCreateStatus(status);
    setIsCreateModalOpen(true);
  };

  const defaultSwimlane = swimlaneKey === DEFAULT_SWIMLANE_KEY ? '' : displayName;

  return (
    <div className="swimlane-row" data-swimlane={swimlaneKey}>
      <SwimlaneHeader
        swimlaneKey={swimlaneKey}
        displayName={displayName}
        taskCount={taskCount}
        isCollapsed={isCollapsed}
        onToggle={onToggleCollapse}
      />
      {!isCollapsed && (
        <div className="swimlane-content" id={contentId}>
          <div className="swimlane-columns">
            {columns.map((column) => {
              const droppableId = `${swimlaneKey}:${column.status}`;
              const columnTasks = tasksByStatus(column.status);

              return (
                <div className="swimlane-cell" key={column.status}>
                  <div className="swimlane-cell-header">
                    <span className="swimlane-cell-count">{columnTasks.length}</span>
                    <button
                      className="swimlane-add-btn"
                      onClick={() => openCreateModal(column.status)}
                      aria-label={`Добавить задачу в колонку "${column.title}"`}
                    >
                      +
                    </button>
                  </div>
                  <Droppable droppableId={droppableId}>
                    {(provided, snapshot) => (
                      <div
                        className={`swimlane-cell-tasks ${snapshot.isDraggingOver ? 'column-tasks-dragging-over' : ''}`}
                        ref={provided.innerRef}
                        {...provided.droppableProps}
                      >
                        {columnTasks.map((task, index) => (
                          <TaskCard key={task.id} task={task} index={index} />
                        ))}
                        {provided.placeholder}
                      </div>
                    )}
                  </Droppable>
                </div>
              );
            })}
          </div>
        </div>
      )}
      {isCollapsed && (
        <div
          className="swimlane-collapsed-drop-zone"
          onClick={() => openCreateModal(TaskStatus.New)}
        >
          <Droppable droppableId={`${swimlaneKey}:${TaskStatus.New}`}>
            {(provided) => (
              <div
                ref={provided.innerRef}
                {...provided.droppableProps}
                style={{ minHeight: '4px' }}
              >
                {provided.placeholder}
              </div>
            )}
          </Droppable>
        </div>
      )}
      <CreateTaskModal
        isOpen={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
        defaultDate={selectedDate}
        defaultStatus={createStatus}
        defaultSwimlane={defaultSwimlane}
      />
    </div>
  );
}