import React, { useState, useRef, useEffect } from 'react';
import { useAssigneeFilter } from '../hooks/useAssigneeFilter';

export const AssigneeFilter: React.FC = () => {
  const { assigneeFilter, assigneeList, isFilterActive, toggleAssignee, clearFilter } = useAssigneeFilter();
  const [isOpen, setIsOpen] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(e.target as Node)) {
        setIsOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  return (
    <div ref={dropdownRef} className="assignee-filter">
      <button
        onClick={() => setIsOpen(!isOpen)}
        onKeyDown={(e) => {
          if (e.key === 'Escape' && isOpen) {
            setIsOpen(false);
          }
        }}
        className={`assignee-filter-trigger ${isFilterActive ? 'assignee-filter-trigger--active' : ''}`}
        aria-label="Фильтр по исполнителям"
        aria-haspopup="listbox"
        aria-expanded={isOpen}
      >
        <span>👤</span>
        <span>Исполнители</span>
        {isFilterActive && (
          <span className="assignee-filter-badge">
            {assigneeFilter.length}
          </span>
        )}
      </button>

      {isOpen && (
        <div
          className="assignee-filter-dropdown"
          onKeyDown={(e) => {
            if (e.key === 'Escape') setIsOpen(false);
          }}
          role="listbox"
          aria-label="Список исполнителей"
        >
          {assigneeList.length === 0 ? (
            <div className="assignee-filter-empty">
              Нет исполнителей
            </div>
          ) : (
            assigneeList.map((assignee) => (
              <label key={assignee} className="assignee-filter-item">
                <input
                  type="checkbox"
                  checked={assigneeFilter.includes(assignee)}
                  onChange={() => toggleAssignee(assignee)}
                  aria-label={`Фильтр: ${assignee}`}
                />
                {assignee}
              </label>
            ))
          )}

          {isFilterActive && (
            <button
              onClick={clearFilter}
              className="assignee-filter-clear"
              aria-label="Сбросить фильтр"
            >
              Сбросить фильтр
            </button>
          )}
        </div>
      )}
    </div>
  );
};