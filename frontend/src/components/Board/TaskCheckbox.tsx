import { useTaskStore } from '../../stores/taskStore';

interface TaskCheckboxProps {
  taskId: string;
}

export function TaskCheckbox({ taskId }: TaskCheckboxProps) {
  const { selectedTaskIds, toggleTaskSelection } = useTaskStore();
  const isSelected = selectedTaskIds.includes(taskId);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
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
