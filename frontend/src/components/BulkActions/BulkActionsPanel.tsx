import { useRef } from 'react';
import { useTaskStore } from '../../stores/taskStore';

export function BulkActionsPanel() {
  const { selectedTaskIds, clearSelection, bulkDelete, bulkMove } = useTaskStore();
  const hiddenDateInputRef = useRef<HTMLInputElement>(null);

  if (selectedTaskIds.length === 0) {
    return null;
  }

  const handleBulkDelete = async () => {
    await bulkDelete();
  };

  const handleMoveButtonClick = () => {
    hiddenDateInputRef.current?.showPicker?.() ?? hiddenDateInputRef.current?.click();
  };

  const handleMoveDateChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (!e.target.value) return;
    const targetDate = new Date(e.target.value);
    bulkMove(targetDate);
    e.target.value = '';
  };

  return (
    <div className="bulk-actions-panel" role="region" aria-label="Массовые операции">
      <div className="bulk-actions-info">
        <span>Выбрано: {selectedTaskIds.length}</span>
      </div>

      <div className="bulk-actions-controls">
        <button
          onClick={handleBulkDelete}
          className="btn btn-danger"
          aria-label={`Удалить ${selectedTaskIds.length} выбранных задач`}
        >
          Удалить ({selectedTaskIds.length})
        </button>
        <button
          onClick={handleMoveButtonClick}
          className="btn btn-primary"
          aria-label={`Переместить ${selectedTaskIds.length} выбранных задач на другую дату`}
        >
          Переместить ({selectedTaskIds.length})
        </button>
        <button
          onClick={clearSelection}
          className="btn btn-secondary"
          aria-label="Отменить выбор"
        >
          Отмена
        </button>
        <input
          ref={hiddenDateInputRef}
          type="date"
          onChange={handleMoveDateChange}
          aria-hidden="true"
          style={{
            position: 'absolute',
            opacity: 0,
            pointerEvents: 'none',
            width: 0,
            height: 0,
            border: 'none',
            padding: 0,
            margin: 0,
          }}
        />
      </div>
    </div>
  );
}
