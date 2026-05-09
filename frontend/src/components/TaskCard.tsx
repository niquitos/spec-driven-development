import { Task } from '../types/task';
import { useTaskStore } from '../stores/taskStore';

interface TaskCardProps {
  task: Task;
}

export function TaskCard({ task }: TaskCardProps) {
  const { toggleTaskSelection, deleteTask } = useTaskStore();
  const { selectedTaskIds } = useTaskStore();
  const isSelected = selectedTaskIds.includes(task.id);

  const handleEdit = () => {
    const newTitle = prompt('Edit title:', task.title);
    if (newTitle !== null) {
      // Update logic here
    }
  };

  const handleDelete = () => {
    if (confirm(`Are you sure you want to delete task "${task.title}"?`)) {
      deleteTask(task.id);
    }
  };

  const handleSelectionChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    toggleTaskSelection(task.id);
  };

  return (
    <div className="task-card" draggable>
      <div className="task-card-header">
        <input
          type="checkbox"
          checked={isSelected}
          onChange={handleSelectionChange}
          aria-label={`Select task ${task.title}`}
        />
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
  );
}
