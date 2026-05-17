import { useCallback } from 'react';
import { useTaskStore } from '../stores/taskStore';

export function useAssigneeFilter() {
  const { assigneeFilter, assigneeList, setAssigneeFilter } = useTaskStore();

  const isFilterActive = assigneeFilter.length > 0;

  const toggleAssignee = useCallback((assignee: string) => {
    if (assigneeFilter.includes(assignee)) {
      setAssigneeFilter(assigneeFilter.filter((a) => a !== assignee));
    } else {
      setAssigneeFilter([...assigneeFilter, assignee]);
    }
  }, [assigneeFilter, setAssigneeFilter]);

  const clearFilter = useCallback(() => {
    setAssigneeFilter([]);
  }, [setAssigneeFilter]);

  return {
    assigneeFilter,
    assigneeList,
    isFilterActive,
    toggleAssignee,
    clearFilter,
  };
}
