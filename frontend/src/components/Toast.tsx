import { useEffect, useState, useCallback } from 'react';

export interface ToastMessage {
  id: number;
  text: string;
  type: 'success' | 'error' | 'info';
}

let nextId = 0;
const TOAST_DURATION = 3000;

interface ToastState {
  toasts: ToastMessage[];
  addToast: (text: string, type?: ToastMessage['type']) => void;
  removeToast: (id: number) => void;
}

let setToastState: ((state: ToastState) => void) | null = null;

export function addToast(text: string, type: ToastMessage['type'] = 'success') {
  setToastState?.({
    toasts: [],
    addToast: () => {},
    removeToast: () => {},
  });
  // Use external setter
  if (externalAddToast) {
    externalAddToast(text, type);
  }
}

let externalAddToast: ((text: string, type: ToastMessage['type']) => void) | null = null;

export function ToastContainer() {
  const [toasts, setToasts] = useState<ToastMessage[]>([]);

  const removeToast = useCallback((id: number) => {
    setToasts((prev) => prev.filter((t) => t.id !== id));
  }, []);

  useEffect(() => {
    externalAddToast = (text: string, type: ToastMessage['type'] = 'success') => {
      const id = nextId++;
      const toast: ToastMessage = { id, text, type };
      setToasts((prev) => [...prev, toast]);
      setTimeout(() => {
        setToasts((prev) => prev.filter((t) => t.id !== id));
      }, TOAST_DURATION);
    };
    return () => {
      externalAddToast = null;
    };
  }, []);

  if (toasts.length === 0) {
    return null;
  }

  return (
    <div className="toast-container" role="status" aria-live="polite">
      {toasts.map((toast) => (
        <div
          key={toast.id}
          className={`toast toast-${toast.type}`}
          onClick={() => removeToast(toast.id)}
        >
          {toast.text}
        </div>
      ))}
    </div>
  );
}