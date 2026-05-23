import axios from 'axios';
import { Task, CreateTaskDto, UpdateTaskDto } from '../types/task';

const api = axios.create({
  baseURL: '/api',
});

export const taskApi = {
  async getTasks(date: string, assignees?: string[]): Promise<Task[]> {
    const params: Record<string, string> = { date };
    if (assignees && assignees.length > 0) {
      params.assignees = assignees.join(',');
    }
    const response = await api.get<Task[]>('/tasks', { params });
    return response.data;
  },

  async createTask(dto: CreateTaskDto): Promise<Task> {
    const response = await api.post<Task>('/tasks', dto);
    return response.data;
  },

  async updateTask(id: number, dto: UpdateTaskDto): Promise<void> {
    await api.put(`/tasks/${id}`, dto);
  },

  async deleteTask(id: number): Promise<void> {
    await api.delete(`/tasks/${id}`);
  },

  async bulkDelete(taskIds: number[]): Promise<{ deleted: number }> {
    const response = await api.post<{ deleted: number }>('/tasks/bulk/delete', { taskIds });
    return response.data;
  },

  async bulkMove(taskIds: number[], targetDate: string): Promise<{ moved: number; targetDate: string }> {
    const response = await api.post<{ moved: number; targetDate: string }>('/tasks/bulk/move', { taskIds, targetDate });
    return response.data;
  },

  async getAssignees(): Promise<string[]> {
    const response = await api.get<string[]>('/tasks/assignees');
    return response.data;
  },

  async moveIncompleteToTomorrow(): Promise<{ moved: number; targetDate: string }> {
    const response = await api.post<{ moved: number; targetDate: string }>('/tasks/bulk/move-incomplete-to-tomorrow');
    return response.data;
  },
};
