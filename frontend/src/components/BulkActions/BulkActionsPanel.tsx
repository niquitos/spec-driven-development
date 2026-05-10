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
    if (window.confirm(`Delete ${selectedTaskIds.length} selected tasks?`)) {
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
    <div className="bulk-actions-panel" role="region" aria-label="Bulk actions">
      <div className="bulk-actions-info">
        <span>{selectedTaskIds.length} selected</span>
      </div>

      <div className="bulk-actions-controls">
        {!showMoveForm ? (
          <>
            <button
              onClick={handleBulkDelete}
              className="btn btn-danger"
              aria-label={`Delete ${selectedTaskIds.length} selected tasks`}
            >
              Delete ({selectedTaskIds.length})
            </button>
            <button
              onClick={() => setShowMoveForm(true)}
              className="btn btn-primary"
              aria-label={`Move ${selectedTaskIds.length} selected tasks to another date`}
            >
              Move ({selectedTaskIds.length})
            </button>
            <button
              onClick={handleCancel}
              className="btn btn-secondary"
              aria-label="Cancel selection"
            >
              Cancel
            </button>
          </>
        ) : (
          <>
            <div className="move-form">
              <label htmlFor="move-date">Target Date:</label>
              <input
                id="move-date"
                type="date"
                value={moveDate}
                onChange={(e) => setMoveDate(e.target.value)}
                aria-label="Select target date for bulk move"
              />
              <button
                onClick={handleBulkMove}
                className="btn btn-success"
                aria-label={`Move ${selectedTaskIds.length} tasks to ${moveDate}`}
              >
                Confirm Move
              </button>
              <button
                onClick={() => setShowMoveForm(false)}
                className="btn btn-secondary"
                aria-label="Cancel move operation"
              >
                Cancel
              </button>
            </div>
          </>
        )}
      </div>
    </div>
  );
}
