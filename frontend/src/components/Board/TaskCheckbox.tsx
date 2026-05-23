import { useTaskStore } from '../../stores/taskStore';

interface TaskCheckboxProps {
  taskId: number;
}

export function TaskCheckbox({ taskId }: TaskCheckboxProps) {
  const { selectedTaskIds, toggleTaskSelection } = useTaskStore();
  const isSelected = selectedTaskIds.includes(taskId);

  const handleChange = () => {
    toggleTaskSelection(taskId);
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      toggleTaskSelection(taskId);
    }
  };

  return (
    <input
      type="checkbox"
      checked={isSelected}
      onChange={handleChange}
      onKeyDown={handleKeyDown}
      aria-label={`Выделить задачу ${taskId}`}
      className="task-checkbox"
      onClick={(e) => e.stopPropagation()}
    />
  );
}
