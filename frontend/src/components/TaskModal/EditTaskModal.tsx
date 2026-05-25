import { useState, useEffect } from 'react';
import { useTaskStore } from '../../stores/taskStore';
import { Task, TaskStatus, UpdateTaskDto } from '../../types/task';
import { AssigneeCombobox } from '../AssigneeCombobox';
import { SwimlaneCombobox } from '../SwimlaneCombobox';

interface EditTaskModalProps {
  isOpen: boolean;
  onClose: () => void;
  task: Task | null;
}

export function EditTaskModal({ isOpen, onClose, task }: EditTaskModalProps) {
  const { updateTask, getAssigneeList, swimlaneList } = useTaskStore();
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [date, setDate] = useState('');
  const [status, setStatus] = useState<TaskStatus>(TaskStatus.New);
  const [assignee, setAssignee] = useState('');
  const [swimlane, setSwimlane] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (task && isOpen) {
      setTitle(task.title);
      setDescription(task.description || '');
      setDate(new Date(task.date).toISOString().split('T')[0]);
      setStatus(task.status);
      setAssignee(task.assignee || '');
      setSwimlane(task.swimlane || '');
      setError(null);
      setIsSubmitting(false);
    }
  }, [task, isOpen]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setIsSubmitting(true);

    if (!task) return;

    try {
      const updates: UpdateTaskDto = {
        title: title.trim(),
        description: description || undefined,
        date,
        status,
        assignee: assignee || undefined,
        swimlane: swimlane.trim() || null,
      };

      await updateTask(task.id, updates);
      onClose();
    } catch (err) {
      setError('Не удалось обновить задачу. Попробуйте снова.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleBackdropClick = (e: React.MouseEvent) => {
    if (e.target === e.currentTarget) {
      onClose();
    }
  };

  if (!isOpen || !task) return null;

  return (
    <div className="modal-backdrop" onClick={handleBackdropClick}>
      <div className="modal-content" role="dialog" aria-labelledby="edit-modal-title" aria-modal="true">
        <h2 id="edit-modal-title" className="modal-title">Редактировать задачу</h2>

        <form onSubmit={handleSubmit} className="task-form">
          <div className="form-group">
            <label htmlFor="edit-task-title" className="form-label">Название *</label>
            <input
              id="edit-task-title"
              type="text"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              className="form-input"
              placeholder="Введите название задачи"
              required
              maxLength={200}
              autoFocus
            />
          </div>

          <div className="form-group">
            <label htmlFor="edit-task-description" className="form-label">Описание</label>
            <textarea
              id="edit-task-description"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              className="form-textarea"
              placeholder="Введите описание (необязательно)"
              maxLength={2000}
              rows={4}
            />
          </div>

          <div className="form-group">
            <label htmlFor="edit-task-date" className="form-label">Дата *</label>
            <input
              id="edit-task-date"
              type="date"
              value={date}
              onChange={(e) => setDate(e.target.value)}
              className="form-input"
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="edit-task-status" className="form-label">Статус</label>
            <select
              id="edit-task-status"
              value={status}
              onChange={(e) => setStatus(Number(e.target.value) as TaskStatus)}
              className="form-input"
            >
              <option value={TaskStatus.New}>Новые</option>
              <option value={TaskStatus.InProgress}>В процессе</option>
              <option value={TaskStatus.Done}>Сделаны</option>
            </select>
          </div>

          <div className="form-group">
            <label htmlFor="edit-task-assignee" className="form-label">Исполнитель</label>
            <AssigneeCombobox
              value={assignee}
              options={getAssigneeList()}
              onChange={setAssignee}
              placeholder="Введите имя исполнителя"
            />
          </div>

          <div className="form-group">
            <label htmlFor="edit-task-swimlane" className="form-label">Swimlane</label>
            <SwimlaneCombobox
              value={swimlane}
              options={swimlaneList}
              onChange={setSwimlane}
              placeholder="Выберите swimlane..."
            />
          </div>

          {error && (
            <div className="form-error" role="alert">
              {error}
            </div>
          )}

          <div className="form-actions">
            <button
              type="button"
              onClick={onClose}
              className="btn btn-secondary"
              disabled={isSubmitting}
            >
              Отмена
            </button>
            <button
              type="submit"
              className="btn btn-primary"
              disabled={isSubmitting || !title.trim()}
            >
              {isSubmitting ? 'Сохранение...' : 'Сохранить'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}