import { describe, it, expect, vi } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { AssigneeCombobox } from '../../src/components/AssigneeCombobox';

describe('AssigneeCombobox', () => {
  const defaultOptions = ['Иван', 'Петр', 'Мария'];

  it('renders input with placeholder', () => {
    render(
      <AssigneeCombobox
        value=""
        options={defaultOptions}
        onChange={vi.fn()}
        placeholder="Назначить исполнителя"
      />
    );

    expect(screen.getByPlaceholderText('Назначить исполнителя')).toBeInTheDocument();
  });

  it('renders with given value', () => {
    render(
      <AssigneeCombobox
        value="Иван"
        options={defaultOptions}
        onChange={vi.fn()}
      />
    );

    const input = screen.getByPlaceholderText('Назначить исполнителя') as HTMLInputElement;
    expect(input.value).toBe('Иван');
  });

  it('renders input element with combobox role', () => {
    render(
      <AssigneeCombobox
        value=""
        options={defaultOptions}
        onChange={vi.fn()}
      />
    );

    const input = screen.getByPlaceholderText('Назначить исполнителя');
    expect(input).toHaveAttribute('role', 'combobox');
    expect(input).toHaveAttribute('aria-haspopup', 'listbox');
  });

  it('calls onChange when input value changes', () => {
    const handleChange = vi.fn();
    render(
      <AssigneeCombobox
        value=""
        options={defaultOptions}
        onChange={handleChange}
      />
    );

    const input = screen.getByPlaceholderText('Назначить исполнителя');
    fireEvent.change(input, { target: { value: 'Иван' } });

    expect(handleChange).toHaveBeenCalledWith('Иван');
  });

  it('has aria-autocomplete attribute', () => {
    render(
      <AssigneeCombobox
        value=""
        options={defaultOptions}
        onChange={vi.fn()}
      />
    );

    const input = screen.getByPlaceholderText('Назначить исполнителя');
    expect(input).toHaveAttribute('aria-autocomplete', 'list');
  });

  it('has aria-label on input', () => {
    render(
      <AssigneeCombobox
        value=""
        options={defaultOptions}
        onChange={vi.fn()}
        placeholder="Введите имя исполнителя"
      />
    );

    expect(screen.getByLabelText('Введите имя исполнителя')).toBeInTheDocument();
  });
});
