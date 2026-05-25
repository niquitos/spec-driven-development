import { useCallback } from 'react';

const STORAGE_KEY = 'tasktracker_collapsed_swimlanes';

function loadCollapsedSwimlanes(): Set<string> {
  try {
    const stored = localStorage.getItem(STORAGE_KEY);
    if (stored) {
      const parsed = JSON.parse(stored) as string[];
      return new Set(parsed.map(key => key.toLowerCase()));
    }
  } catch {
    // Ignore parse errors
  }
  return new Set();
}

function saveCollapsedSwimlanes(collapsed: Set<string>): void {
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify([...collapsed]));
  } catch {
    // Ignore storage errors
  }
}

export function useSwimlaneCollapse() {
  const isCollapsed = useCallback((key: string): boolean => {
    const collapsed = loadCollapsedSwimlanes();
    return collapsed.has(key.toLowerCase());
  }, []);

  const toggle = useCallback((key: string): Set<string> => {
    const collapsed = loadCollapsedSwimlanes();
    const normalizedKey = key.toLowerCase();
    if (collapsed.has(normalizedKey)) {
      collapsed.delete(normalizedKey);
    } else {
      collapsed.add(normalizedKey);
    }
    saveCollapsedSwimlanes(collapsed);
    return collapsed;
  }, []);

  const collapseAll = useCallback((): Set<string> => {
    // Cannot collapse all without knowing all keys
    return loadCollapsedSwimlanes();
  }, []);

  const expandAll = useCallback((): Set<string> => {
    saveCollapsedSwimlanes(new Set());
    return new Set();
  }, []);

  return { isCollapsed, toggle, collapseAll, expandAll, loadCollapsedSwimlanes };
}