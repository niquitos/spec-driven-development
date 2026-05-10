import { useState } from 'react';
import { useSortable } from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import { Task } from '../types/task';
import { useTaskStore } from '../stores/taskStore';
import { TaskCheckbox } from './Board/TaskCheckbox';
import { EditTaskModal } from './TaskModal/EditTaskModal';
import { DeleteConfirmModal } from './TaskModal/DeleteConfirmModal';

interface TaskCardProps {
  task: Task;
}

export function TaskCard({ task }: TaskCardProps) {
  const { deleteTask } = useTaskStore();
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);

  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
  } = useSortable({
    id: task.id,
    data: {
      taskId: task.id,
      status: task.status,
    },
  });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
  };

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
      <div
        ref={setNodeRef}
        style={style}
        className="task-card"
        {...attributes}
        {...listeners}
      >
        <div className="task-card-header">
          <TaskCheckbox taskId={task.id} />
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
