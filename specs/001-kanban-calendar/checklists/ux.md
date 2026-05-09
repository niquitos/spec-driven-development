# UX Requirements Checklist: Kanban Calendar Board

**Purpose**: Validate UX requirements quality — visual hierarchy, drag-n-drop, accessibility, keyboard navigation
**Created**: 2026-05-09
**Feature**: [spec.md](../spec.md)
**Focus Areas**: Visual layout, Drag-n-drop behavior, Accessibility (keyboard + aria), Interaction states
**Depth**: Standard (~25 items)
**Audience**: PR Reviewer

---

## Visual Layout & Hierarchy

- [ ] CHK001 Are the exact number and positioning of columns (3) explicitly specified? [Completeness, Spec §FR-001]
- [ ] CHK002 Are column titles ("Новые", "В процессе", "Сделаны") defined with consistent naming convention? [Clarity, Spec §FR-001]
- [ ] CHK003 Is the visual hierarchy between header, date navigator, and board content specified? [Clarity, Spec §FR-002]
- [ ] CHK004 Are task card visual properties (size, padding, border) defined with measurable criteria? [Gap]
- [ ] CHK005 Is empty state appearance specified for columns with no tasks? [Coverage, Spec §US1-SA3]
- [ ] CHK006 Are typography requirements defined for task title vs description distinction? [Gap]

---

## Date Navigation UX

- [ ] CHK007 Are button placements for "назад/вперёд" navigation explicitly positioned relative to date display? [Gap, Spec §FR-004]
- [ ] CHK008 Is the date format specified for header display (e.g., "DD MMM YYYY")? [Clarity, Spec §FR-002]
- [ ] CHK009 Are date-picker interaction requirements defined (click behavior, calendar popup)? [Gap, Spec §FR-005]
- [ ] CHK010 Is navigation button disabled state defined for boundary dates (if any)? [Gap, Spec §FR-004]
- [ ] CHK011 Are loading states specified when switching between dates? [Coverage, Exception Flow]

---

## Drag-n-Drop Behavior

- [ ] CHK012 Are drag visual feedback requirements defined (ghost image, highlight on drop zones)? [Gap, Spec §FR-012]
- [ ] CHK013 Are drop zone boundaries specified for column-to-column dragging? [Clarity, Spec §FR-012]
- [ ] CHK014 Is the reordering animation behavior defined for vertical drag within column? [Gap, Spec §FR-013]
- [ ] CHK015 Are invalid drop target visual indicators specified? [Coverage, Edge Case]
- [ ] CHK016 Is drag cancellation behavior defined (escape key, drop outside zones)? [Gap, Exception Flow]
- [ ] CHK017 Are touch device requirements specified for tablet drag-n-drop support? [Gap, Spec §Constraints]

---

## Accessibility (Keyboard + ARIA)

- [ ] CHK018 Are keyboard tab order requirements defined across all interactive elements? [Clarity, Spec §FR-015]
- [ ] CHK019 Are aria-label requirements specified for all icon-only buttons (pencil, trash, checkbox)? [Completeness, Spec §FR-016]
- [ ] CHK020 Are focus indicator visual properties defined (color, thickness, outline)? [Clarity, Spec §FR-017]
- [ ] CHK021 Is keyboard shortcut specification included for common actions (create, edit, delete)? [Gap]
- [ ] CHK022 Are screen reader announcement requirements defined for dynamic content updates? [Gap, Spec §FR-016]
- [ ] CHK023 Is skip navigation requirement defined for keyboard users to bypass repetitive elements? [Gap, Spec §FR-015]

---

## Interaction States

- [ ] CHK024 Are hover state requirements defined for all clickable elements? [Consistency]
- [ ] CHK025 Are active/pressed state requirements defined for buttons? [Consistency]
- [ ] CHK026 Are disabled state visual requirements specified across all interactive elements? [Consistency]
- [ ] CHK027 Is checkbox selection state (checked/unchecked/indeterminate) visually defined? [Gap, Spec §FR-011]

---

## Modal & Dialog UX

- [ ] CHK028 Are create task modal field layouts specified (label positions, input types)? [Gap, Spec §FR-008]
- [ ] CHK029 Are edit modal pre-population requirements defined for existing task data? [Clarity, Spec §FR-009]
- [ ] CHK030 Are delete confirmation dialog text and button labels specified? [Gap, Spec §FR-010]
- [ ] CHK031 Are modal close behaviors defined (escape key, click outside, cancel button)? [Coverage, Spec §US4-SA4]
- [ ] CHK032 Is form validation error display positioning and timing specified? [Gap, Spec §FR-008]

---

## Bulk Operations UX

- [ ] CHK033 Is bulk action panel trigger threshold defined (when does it appear)? [Gap, Spec §FR-011]
- [ ] CHK034 Are bulk action available operations explicitly listed? [Completeness, Spec §FR-011]
- [ ] CHK035 Is checkbox selection behavior defined for "select all" scenario? [Gap, Spec §FR-011]

---

## Edge Cases & Error States

- [ ] CHK036 Is offline disconnection handling specified for task save operations? [Coverage, Edge Case, Spec §Edge-2]
- [ ] CHK037 Are task title length limits defined with truncation behavior? [Gap, Spec §FR-008]
- [ ] CHK038 Is empty description display behavior specified (placeholder, hidden, collapsed)? [Coverage, Spec §Edge-4]
- [ ] CHK039 Are concurrent edit conflict requirements defined (same task edited twice)? [Gap, Exception Flow]
- [ ] CHK040 Is bulk operation cancellation behavior defined mid-execution? [Gap, Spec §FR-011]

---

## Notes

- Checklist focuses on UX requirements quality for reviewer PR validation
- Items marked [Gap] indicate missing requirements that should be added to spec
- Items marked [Coverage] verify edge case and exception flow documentation
- Accessibility items (CHK018-CHK023) validate FR-015/FR-016/FR-017 implementation requirements
