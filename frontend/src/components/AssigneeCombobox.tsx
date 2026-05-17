import React, { useMemo, useRef, useState, useCallback } from 'react';

interface AssigneeComboboxProps {
  value: string;
  options: string[];
  onChange: (value: string) => void;
  placeholder?: string;
}

export const AssigneeCombobox: React.FC<AssigneeComboboxProps> = ({
  value,
  options,
  onChange,
  placeholder = 'Назначить исполнителя',
}) => {
  const [inputValue, setInputValue] = useState(value);
  const [isOpen, setIsOpen] = useState(false);
  const [activeIndex, setActiveIndex] = useState(-1);
  const inputRef = useRef<HTMLInputElement>(null);

  const filteredOptions = useMemo(() => {
    if (!inputValue) return options;
    return options.filter((opt) =>
      opt.toLowerCase().includes(inputValue.toLowerCase())
    );
  }, [options, inputValue]);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const val = e.target.value;
    setInputValue(val);
    onChange(val);
    setIsOpen(true);
    setActiveIndex(-1);
  };

  const commitOption = useCallback((option: string) => {
    setInputValue(option);
    onChange(option);
    setIsOpen(false);
    setActiveIndex(-1);
  }, [onChange]);

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    switch (e.key) {
      case 'ArrowDown':
        e.preventDefault();
        if (!isOpen) {
          setIsOpen(true);
          setActiveIndex(0);
        } else {
          setActiveIndex((prev) =>
            prev < filteredOptions.length - 1 ? prev + 1 : 0
          );
        }
        break;
      case 'ArrowUp':
        e.preventDefault();
        if (isOpen) {
          setActiveIndex((prev) =>
            prev > 0 ? prev - 1 : filteredOptions.length - 1
          );
        }
        break;
      case 'Enter':
        e.preventDefault();
        if (isOpen && activeIndex >= 0 && activeIndex < filteredOptions.length) {
          commitOption(filteredOptions[activeIndex]);
        }
        break;
      case 'Escape':
        e.preventDefault();
        setIsOpen(false);
        setActiveIndex(-1);
        break;
    }
  };

  const handleFocus = () => {
    if (inputValue) {
      setIsOpen(true);
    }
  };

  const handleBlur = () => {
    // Delay to allow option click to register
    setTimeout(() => setIsOpen(false), 150);
  };

  return (
    <div style={{ position: 'relative' }}>
      <input
        ref={inputRef}
        type="text"
        role="combobox"
        aria-label={placeholder}
        aria-haspopup="listbox"
        aria-expanded={isOpen}
        aria-controls="assignee-listbox"
        aria-activedescendant={activeIndex >= 0 ? `assignee-option-${activeIndex}` : undefined}
        aria-autocomplete="list"
        value={inputValue}
        onChange={handleInputChange}
        onKeyDown={handleKeyDown}
        onFocus={handleFocus}
        onBlur={handleBlur}
        placeholder={placeholder}
        style={{
          width: '100%',
          padding: '6px 8px',
          border: '1px solid #ccc',
          borderRadius: '4px',
          fontSize: '14px',
          boxSizing: 'border-box',
        }}
      />
      {isOpen && filteredOptions.length > 0 && (
        <ul
          id="assignee-listbox"
          role="listbox"
          aria-label="Предлагаемые исполнители"
          style={{
            position: 'absolute',
            top: '100%',
            left: 0,
            right: 0,
            zIndex: 1000,
            background: '#fff',
            border: '1px solid #ccc',
            borderRadius: '4px',
            boxShadow: '0 4px 12px rgba(0,0,0,0.15)',
            maxHeight: '200px',
            overflowY: 'auto',
            margin: '4px 0 0',
            padding: '4px 0',
            listStyle: 'none',
          }}
        >
          {filteredOptions.map((option, index) => (
            <li
              key={option}
              id={`assignee-option-${index}`}
              role="option"
              aria-selected={index === activeIndex}
              onMouseDown={() => commitOption(option)}
              style={{
                padding: '6px 12px',
                cursor: 'pointer',
                fontSize: '14px',
                background: index === activeIndex ? '#e8f0fe' : 'transparent',
              }}
            >
              {option}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};
