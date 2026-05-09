import { useState } from 'react';
import { Task } from '../types/task';
import { useTaskStore } from '../stores/taskStore';
import { EditTaskModal } from './TaskModal/EditTaskModal';
import { DeleteConfirmModal } from './TaskModal/DeleteConfirmModal';

interface TaskCardProps {
  task: Task;
}

export function TaskCard({ task }: TaskCardProps) {
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
      <div className="task-card" draggable>
        <div className="task-card-header">
          <div className="task-card-actions">
            <button onClick={handleEdit} aria-label={`Edit task ${task.title}`}>
              ✏️
            </button>
            <button onClick={handleDelete} aria-label={`Delete task ${task.title}`}>
              🗑️
            </button>
          </div>
        </div>
        <h3 className="task-card-title">{task.title}</h3>
        {task.description && (
          <p className="task-card-description">{task.description}</p>
        )}
      </div>
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
