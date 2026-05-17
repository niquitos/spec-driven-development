import { useState } from 'react';
import { DragEndEvent } from '@dnd-kit/core';
import { useTaskStore } from '../stores/taskStore';
import { TaskStatus } from '../types/task';

export function useDragDrop() {
  const { moveTask } = useTaskStore();
  const [isDragging, setIsDragging] = useState(false);

  const handleDragStart = () => {
    setIsDragging(true);
  };

  const handleDragEnd = async (event: DragEndEvent) => {
    setIsDragging(false);

    const { active, over } = event;

    if (!over || !active.data.current) return;

    const taskId = active.data.current.taskId as number;
    const newStatus = over.data.current?.status as TaskStatus | undefined;

    if (taskId && newStatus) {
      moveTask(taskId, newStatus, 0);
    }
  };

  return {
    isDragging,
    handleDragStart,
    handleDragEnd,
  };
}
