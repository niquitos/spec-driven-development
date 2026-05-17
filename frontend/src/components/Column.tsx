import { useState } from 'react';
import { Droppable } from '@hello-pangea/dnd';
import { TaskCard } from './TaskCard';
import { CreateTaskModal } from './TaskModal/CreateTaskModal';
import { Task, TaskStatus } from '../types/task';
import { useTaskStore } from '../stores/taskStore';

interface ColumnProps {
  status: TaskStatus;
  title: string;
  tasks: Task[];
}

export function Column({ status, title, tasks }: ColumnProps) {
  const { selectedDate, isLoading } = useTaskStore();
  const [isModalOpen, setIsModalOpen] = useState(false);

  const openModal = () => setIsModalOpen(true);
  const closeModal = () => setIsModalOpen(false);

  return (
    <div className="column" data-status={status}>
      <div className="column-header">
        <h2>{title}</h2>
        <button onClick={openModal} aria-label={`Добавить задачу в колонку "${title}"`}>
          +
        </button>
      </div>
      <Droppable droppableId={String(status)}>
        {(provided, snapshot) => (
          <div
            className={`column-tasks ${snapshot.isDraggingOver ? 'column-tasks-dragging-over' : ''}`}
            ref={provided.innerRef}
            {...provided.droppableProps}
          >
            {isLoading ? (
              <div className="column-loading">
                <div className="loading-skeleton"></div>
                <div className="loading-skeleton"></div>
                <div className="loading-skeleton"></div>
              </div>
            ) : (
              tasks.map((task, index) => (
                <TaskCard key={task.id} task={task} index={index} />
              ))
            )}
            {provided.placeholder}
          </div>
        )}
      </Droppable>
      <CreateTaskModal
        isOpen={isModalOpen}
        onClose={closeModal}
        defaultDate={selectedDate}
        defaultStatus={status}
      />
    </div>
  );
}
