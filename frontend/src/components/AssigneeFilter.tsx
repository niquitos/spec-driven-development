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
    <div ref={dropdownRef} style={{ position: 'relative' }}>
      <button
        onClick={() => setIsOpen(!isOpen)}
        onKeyDown={(e) => {
          if (e.key === 'Escape' && isOpen) {
            setIsOpen(false);
          }
        }}
        aria-label="Фильтр по исполнителям"
        aria-haspopup="listbox"
        aria-expanded={isOpen}
        style={{
          padding: '6px 12px',
          border: `1px solid ${isFilterActive ? '#4a90d9' : '#ccc'}`,
          borderRadius: '4px',
          background: isFilterActive ? '#e8f0fe' : '#fff',
          cursor: 'pointer',
          fontSize: '14px',
          display: 'flex',
          alignItems: 'center',
          gap: '4px',
        }}
      >
        <span>👤</span>
        <span>Исполнители</span>
        {isFilterActive && (
          <span style={{
            background: '#4a90d9',
            color: '#fff',
            borderRadius: '10px',
            padding: '1px 6px',
            fontSize: '11px',
            marginLeft: '4px',
          }}>
            {assigneeFilter.length}
          </span>
        )}
      </button>

      {isOpen && (
        <div
          onKeyDown={(e) => {
            if (e.key === 'Escape') setIsOpen(false);
          }}
          style={{
            position: 'absolute',
            top: '100%',
            left: 0,
            zIndex: 1000,
            background: '#fff',
            border: '1px solid #ccc',
            borderRadius: '4px',
            boxShadow: '0 4px 12px rgba(0,0,0,0.15)',
            minWidth: '200px',
            marginTop: '4px',
            padding: '8px 0',
          }}
          role="listbox"
          aria-label="Список исполнителей"
        >
          {assigneeList.length === 0 ? (
            <div style={{ padding: '8px 16px', color: '#999', fontSize: '13px' }}>
              Нет исполнителей
            </div>
          ) : (
            assigneeList.map((assignee) => (
              <label
                key={assignee}
                style={{
                  display: 'flex',
                  alignItems: 'center',
                  gap: '8px',
                  padding: '6px 16px',
                  cursor: 'pointer',
                  fontSize: '14px',
                  whiteSpace: 'nowrap',
                }}
              >
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
            <div style={{ borderTop: '1px solid #eee', marginTop: '4px', padding: '8px 16px 0' }}>
              <button
                onClick={clearFilter}
                style={{
                  background: 'none',
                  border: 'none',
                  color: '#d32f2f',
                  cursor: 'pointer',
                  fontSize: '13px',
                  padding: 0,
                }}
                aria-label="Сбросить фильтр"
              >
                Сбросить фильтр
              </button>
            </div>
          )}
        </div>
      )}
    </div>
  );
};
