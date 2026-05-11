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

  return (
    <input
      type="checkbox"
      checked={isSelected}
      onChange={handleChange}
      aria-label={`Select task ${taskId}`}
      className="task-checkbox"
      onClick={(e) => e.stopPropagation()}
    />
  );
}
