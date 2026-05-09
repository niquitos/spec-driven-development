import { useState } from 'react';
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
  const { selectedDate, isCreateModalOpen, setIsCreateModalOpen } = useTaskStore();
  const [isModalOpen, setIsModalOpen] = useState(false);

  const openModal = () => setIsModalOpen(true);
  const closeModal = () => setIsModalOpen(false);

  return (
    <div className="column" data-status={status}>
      <div className="column-header">
        <h2>{title}</h2>
        <button onClick={openModal} aria-label={`Add task to ${title}`}>
          +
        </button>
      </div>
      <div className="column-tasks">
        {tasks.map((task) => (
          <TaskCard key={task.id} task={task} />
        ))}
      </div>
      <CreateTaskModal
        isOpen={isModalOpen}
        onClose={closeModal}
        defaultDate={selectedDate}
        defaultStatus={status}
      />
    </div>
  );
}
