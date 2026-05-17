import { useState } from 'react';
import { useTaskStore } from '../../stores/taskStore';

export function BulkActionsPanel() {
  const { selectedTaskIds, clearSelection, bulkDelete, bulkMove, selectedDate } = useTaskStore();
  const [showMoveForm, setShowMoveForm] = useState(false);
  const [moveDate, setMoveDate] = useState(selectedDate.toISOString().split('T')[0]);

  if (selectedTaskIds.length === 0) {
    return null;
  }

  const handleBulkDelete = async () => {
    if (window.confirm(`Удалить ${selectedTaskIds.length} выбранных задач?`)) {
      await bulkDelete();
    }
  };

  const handleBulkMove = async () => {
    const targetDate = new Date(moveDate);
    await bulkMove(targetDate);
    setShowMoveForm(false);
  };

  const handleCancel = () => {
    clearSelection();
  };

  return (
    <div className="bulk-actions-panel" role="region" aria-label="Массовые операции">
      <div className="bulk-actions-info">
        <span>Выбрано: {selectedTaskIds.length}</span>
      </div>

      <div className="bulk-actions-controls">
        {!showMoveForm ? (
          <>
            <button
              onClick={handleBulkDelete}
              className="btn btn-danger"
              aria-label={`Удалить ${selectedTaskIds.length} выбранных задач`}
            >
              Удалить ({selectedTaskIds.length})
            </button>
            <button
              onClick={() => setShowMoveForm(true)}
              className="btn btn-primary"
              aria-label={`Переместить ${selectedTaskIds.length} выбранных задач на другую дату`}
            >
              Переместить ({selectedTaskIds.length})
            </button>
            <button
              onClick={handleCancel}
              className="btn btn-secondary"
              aria-label="Отменить выбор"
            >
              Отмена
            </button>
          </>
        ) : (
          <>
            <div className="move-form">
              <label htmlFor="move-date">Целевая дата:</label>
              <input
                id="move-date"
                type="date"
                value={moveDate}
                onChange={(e) => setMoveDate(e.target.value)}
                aria-label="Выбрать целевую дату для перемещения"
              />
              <button
                onClick={handleBulkMove}
                className="btn btn-success"
                aria-label={`Переместить ${selectedTaskIds.length} задач на ${moveDate}`}
              >
                Подтвердить
              </button>
              <button
                onClick={() => setShowMoveForm(false)}
                className="btn btn-secondary"
                aria-label="Отменить перемещение"
              >
                Отмена
              </button>
            </div>
          </>
        )}
      </div>
    </div>
  );
}
