import { TaskCard } from './TaskCard';
import { Task, TaskStatus } from '../types/task';
import { useTaskStore } from '../stores/taskStore';

interface ColumnProps {
  status: TaskStatus;
  title: string;
  tasks: Task[];
}

export function Column({ status, title, tasks }: ColumnProps) {
  const { addTask } = useTaskStore();

  const handleCreateTask = () => {
    const title = prompt('Enter task title:');
    if (title) {
      const newTask: Task = {
        id: Date.now(),
        title,
        description: null,
        status,
        date: new Date().toISOString(),
        order: tasks.length,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      };
      addTask(newTask);
    }
  };

  return (
    <div className="column" data-status={status}>
      <div className="column-header">
        <h2>{title}</h2>
        <button onClick={handleCreateTask} aria-label={`Add task to ${title}`}>
          +
        </button>
      </div>
      <div className="column-tasks">
        {tasks.map((task) => (
          <TaskCard key={task.id} task={task} />
        ))}
      </div>
    </div>
  );
}
