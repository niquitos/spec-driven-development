import axios from 'axios';
import { Task, CreateTaskDto, UpdateTaskDto } from '../types/task';

const api = axios.create({
  baseURL: (import.meta as any).env?.VITE_API_URL || 'http://localhost:5000/api',
});

export const taskApi = {
  async getTasks(date: string): Promise<Task[]> {
    const response = await api.get<Task[]>('/tasks', { params: { date } });
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
};
