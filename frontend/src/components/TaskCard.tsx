import { useState } from 'react';
import { Draggable } from '@hello-pangea/dnd';
import { Task } from '../types/task';
import { useTaskStore } from '../stores/taskStore';
import { TaskCheckbox } from './Board/TaskCheckbox';
import { EditTaskModal } from './TaskModal/EditTaskModal';
import { DeleteConfirmModal } from './TaskModal/DeleteConfirmModal';

interface TaskCardProps {
  task: Task;
  index: number;
}

export function TaskCard({ task, index }: TaskCardProps) {
  const { deleteTask } = useTaskStore();
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);

  const handleEdit = () => {
    setIsEditModalOpen(true);
  };

  const handleDelete = () => {
    setIsDeleteModalOpen(true);
  };

  const handleConfirmDelete = () => {
    deleteTask(task.id);
  };

  return (
    <>
      <Draggable draggableId={String(task.id)} index={index}>
        {(provided, snapshot) => (
          <div
            ref={provided.innerRef}
            {...provided.draggableProps}
            {...provided.dragHandleProps}
            className={`task-card ${snapshot.isDragging ? 'task-card-dragging' : ''}`}
            style={{
              ...provided.draggableProps.style,
            }}
          >
            <div className="task-card-header">
              <TaskCheckbox taskId={task.id} />
              <div className="task-card-actions">
                <button onClick={handleEdit} aria-label={`Редактировать задачу "${task.title}"`}>
                  ✏️
                </button>
                <button onClick={handleDelete} aria-label={`Удалить задачу "${task.title}"`}>
                  🗑️
                </button>
              </div>
            </div>
            <h3 className="task-card-title">{task.title}</h3>
            {task.description && (
              <p className="task-card-description">{task.description}</p>
            )}
            {task.assignee && (
              <div className="task-card-assignee">
                <span className="task-card-assignee-label">{task.assignee}</span>
              </div>
            )}
          </div>
        )}
      </Draggable>
      <EditTaskModal
        isOpen={isEditModalOpen}
        onClose={() => setIsEditModalOpen(false)}
        task={isEditModalOpen ? task : null}
      />
      <DeleteConfirmModal
        isOpen={isDeleteModalOpen}
        onClose={() => setIsDeleteModalOpen(false)}
        onConfirm={handleConfirmDelete}
        task={isDeleteModalOpen ? task : null}
      />
    </>
  );
}