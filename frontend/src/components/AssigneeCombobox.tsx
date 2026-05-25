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
    setTimeout(() => setIsOpen(false), 150);
  };

  return (
    <div className="assignee-combobox">
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
        className="assignee-combobox-input"
      />
      {isOpen && filteredOptions.length > 0 && (
        <ul
          id="assignee-listbox"
          role="listbox"
          aria-label="Предлагаемые исполнители"
          className="assignee-combobox-list"
        >
          {filteredOptions.map((option, index) => (
            <li
              key={option}
              id={`assignee-option-${index}`}
              role="option"
              aria-selected={index === activeIndex}
              onMouseDown={() => commitOption(option)}
              className={`assignee-combobox-option ${index === activeIndex ? 'assignee-combobox-option--active' : ''}`}
            >
              {option}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};