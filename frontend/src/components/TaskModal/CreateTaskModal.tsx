import { useState, useEffect } from 'react';
import { useTaskStore } from '../../stores/taskStore';
import { TaskStatus } from '../../types/task';
import { AssigneeCombobox } from '../AssigneeCombobox';
import { SwimlaneCombobox } from '../SwimlaneCombobox';

interface CreateTaskModalProps {
  isOpen: boolean;
  onClose: () => void;
  defaultDate: Date;
  defaultStatus: TaskStatus;
  defaultSwimlane?: string;
}

export function CreateTaskModal({ isOpen, onClose, defaultDate, defaultStatus, defaultSwimlane }: CreateTaskModalProps) {
  const { createTask, setIsCreateModalOpen, getAssigneeList, swimlaneList } = useTaskStore();
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [date, setDate] = useState('');
  const [assignee, setAssignee] = useState('');
  const [swimlane, setSwimlane] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (isOpen) {
      setDate(defaultDate.toISOString().split('T')[0]);
      setTitle('');
      setDescription('');
      setAssignee('');
      setSwimlane(defaultSwimlane ?? '');
      setError(null);
      setIsSubmitting(false);
    }
  }, [isOpen, defaultDate, defaultSwimlane]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setIsSubmitting(true);

    try {
      await createTask({
        title,
        description: description || undefined,
        date,
        status: defaultStatus,
        order: 0,
        assignee: assignee || undefined,
        swimlane: swimlane.trim() || undefined,
      });
      setIsCreateModalOpen(false);
      onClose();
    } catch (err) {
      setError('Не удалось создать задачу. Попробуйте снова.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleBackdropClick = (e: React.MouseEvent) => {
    if (e.target === e.currentTarget) {
      onClose();
    }
  };

  if (!isOpen) return null;

  return (
    <div className="modal-backdrop" onClick={handleBackdropClick}>
      <div className="modal-content" role="dialog" aria-labelledby="modal-title" aria-modal="true">
        <h2 id="modal-title" className="modal-title">Создать задачу</h2>

        <form onSubmit={handleSubmit} className="task-form">
          <div className="form-group">
            <label htmlFor="task-title" className="form-label">Название *</label>
            <input
              id="task-title"
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
            <label htmlFor="task-description" className="form-label">Описание</label>
            <textarea
              id="task-description"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              className="form-textarea"
              placeholder="Введите описание (необязательно)"
              maxLength={2000}
              rows={4}
            />
          </div>

          <div className="form-group">
            <label htmlFor="task-date" className="form-label">Дата *</label>
            <input
              id="task-date"
              type="date"
              value={date}
              onChange={(e) => setDate(e.target.value)}
              className="form-input"
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="task-assignee" className="form-label">Исполнитель</label>
            <AssigneeCombobox
              value={assignee}
              options={getAssigneeList()}
              onChange={setAssignee}
              placeholder="Введите имя исполнителя"
            />
          </div>

          <div className="form-group">
            <label htmlFor="task-swimlane" className="form-label">Swimlane</label>
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
              {isSubmitting ? 'Создание...' : 'Создать'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}