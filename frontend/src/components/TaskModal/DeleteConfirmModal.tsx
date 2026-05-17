import { Task } from '../../types/task';

interface DeleteConfirmModalProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  task: Task | null;
}

export function DeleteConfirmModal({ isOpen, onClose, onConfirm, task }: DeleteConfirmModalProps) {
  const handleBackdropClick = (e: React.MouseEvent) => {
    if (e.target === e.currentTarget) {
      onClose();
    }
  };

  if (!isOpen || !task) return null;

  return (
    <div className="modal-backdrop" onClick={handleBackdropClick}>
      <div className="modal-content" role="alertdialog" aria-labelledby="delete-modal-title" aria-modal="true">
        <h2 id="delete-modal-title" className="modal-title">Удаление задачи</h2>

        <p className="delete-message">
          Вы уверены, что хотите удалить задачу <strong>"{task.title}"</strong>?
        </p>
        <p className="delete-warning">
          Это действие нельзя отменить.
        </p>

        <div className="form-actions">
          <button
            type="button"
            onClick={onClose}
            className="btn btn-secondary"
          >
            Отмена
          </button>
          <button
            type="button"
            onClick={() => {
              onConfirm();
              onClose();
            }}
            className="btn btn-danger"
          >
            Удалить
          </button>
        </div>
      </div>
    </div>
  );
}
