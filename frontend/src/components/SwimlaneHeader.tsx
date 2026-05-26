import { normalizeSwimlaneKey } from '../utils/swimlane';

interface SwimlaneHeaderProps {
  swimlaneKey: string;
  displayName: string;
  taskCount: number;
  isCollapsed: boolean;
  onToggle: () => void;
}

export function SwimlaneHeader({ swimlaneKey, displayName, taskCount, isCollapsed, onToggle }: SwimlaneHeaderProps) {
  const collapseId = `swimlane-content-${normalizeSwimlaneKey(swimlaneKey)}`;

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' || e.key === ' ') {
      e.preventDefault();
      onToggle();
    }
  };

  return (
    <div
      className="swimlane-header"
      role="button"
      tabIndex={0}
      aria-expanded={!isCollapsed}
      aria-controls={collapseId}
      aria-label={isCollapsed ? `Развернуть swimlane ${displayName}` : `Свернуть swimlane ${displayName}`}
      onClick={onToggle}
      onKeyDown={handleKeyDown}
    >
      <span className="swimlane-header-icon">{isCollapsed ? '▸' : '▾'}</span>
      <h3 className="swimlane-header-title">{displayName}</h3>
      <span className="swimlane-header-count">({taskCount})</span>
    </div>
  );
}