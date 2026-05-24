# Professional Button Style Guide

## Overview
A comprehensive, professional button color scheme has been implemented with appropriate colors for different action types. All button styles follow Material Design principles for a clean, modern, and uniform appearance.

---

## Button Styles Available

### 1. 🔵 PRIMARY BUTTON (Blue)
**Color:** #2196F3 (Material Blue)  
**Use For:** Main actions, primary operations

**Examples:**
- Save
- Submit
- Login
- Confirm
- Add
- Create
- Apply

**Usage:**
```xml
<Button Content="Save" Style="{StaticResource PrimaryButton}"/>
```

**Colors:**
- Normal: #2196F3
- Hover: #1976D2
- Pressed: #1565C0
- Disabled: #BDBDBD

---

### 2. ✅ SUCCESS BUTTON (Green)
**Color:** #4CAF50 (Material Green)  
**Use For:** Positive actions, completion

**Examples:**
- Approve
- Complete
- Resolve
- Accept
- Finish
- Done

**Usage:**
```xml
<Button Content="Approve" Style="{StaticResource SuccessButton}"/>
```

**Colors:**
- Normal: #4CAF50
- Hover: #388E3C
- Pressed: #2E7D32
- Disabled: #BDBDBD

---

### 3. 🔴 DANGER BUTTON (Red)
**Color:** #F44336 (Material Red)  
**Use For:** Destructive actions, deletions

**Examples:**
- Delete
- Remove
- Cancel (destructive)
- Reject
- Discard
- Clear

**Usage:**
```xml
<Button Content="Delete" Style="{StaticResource DangerButton}"/>
```

**Colors:**
- Normal: #F44336
- Hover: #D32F2F
- Pressed: #C62828
- Disabled: #BDBDBD

---

### 4. 🟠 WARNING BUTTON (Orange)
**Color:** #FF9800 (Material Orange)  
**Use For:** Caution actions, modifications

**Examples:**
- Update
- Modify
- Edit
- Change
- Reassign
- Transfer

**Usage:**
```xml
<Button Content="Update" Style="{StaticResource WarningButton}"/>
```

**Colors:**
- Normal: #FF9800
- Hover: #F57C00
- Pressed: #EF6C00
- Disabled: #BDBDBD

---

### 5. ⚪ SECONDARY BUTTON (Gray)
**Color:** #607D8B (Material Blue Gray)  
**Use For:** Neutral actions, cancellations

**Examples:**
- Cancel (non-destructive)
- Close
- Back
- Exit
- Skip
- Dismiss

**Usage:**
```xml
<Button Content="Cancel" Style="{StaticResource SecondaryButton}"/>
```

**Colors:**
- Normal: #607D8B
- Hover: #546E7A
- Pressed: #455A64
- Disabled: #BDBDBD

---

### 6. 🔷 INFO BUTTON (Cyan)
**Color:** #00BCD4 (Material Cyan)  
**Use For:** Informational actions, viewing

**Examples:**
- View
- Details
- Info
- Preview
- Show
- Display

**Usage:**
```xml
<Button Content="View Details" Style="{StaticResource InfoButton}"/>
```

**Colors:**
- Normal: #00BCD4
- Hover: #0097A7
- Pressed: #00838F
- Disabled: #BDBDBD

---

### 7. ⬜ OUTLINE BUTTON
**Color:** Transparent with Blue Border  
**Use For:** Subtle secondary actions

**Examples:**
- Secondary options
- Less important actions
- Alternative choices

**Usage:**
```xml
<Button Content="Learn More" Style="{StaticResource OutlineButton}"/>
```

**Colors:**
- Normal: Transparent, Blue border
- Hover: Light blue background
- Pressed: Lighter blue background
- Disabled: Gray border and text

---

## Button Usage Guidelines

### Action Type Matrix

| Action Type | Button Style | Example |
|-------------|--------------|---------|
| **Save/Submit** | Primary (Blue) | Save Changes, Submit Form |
| **Approve/Complete** | Success (Green) | Approve Request, Mark Complete |
| **Delete/Remove** | Danger (Red) | Delete User, Remove Item |
| **Edit/Update** | Warning (Orange) | Edit Profile, Update Status |
| **Cancel/Close** | Secondary (Gray) | Cancel, Close, Back |
| **View/Info** | Info (Cyan) | View Details, Show Info |
| **Optional** | Outline | Learn More, See More |

---

## Dialog Button Patterns

### Confirmation Dialogs
```xml
<!-- Primary action on right, secondary on left -->
<StackPanel Orientation="Horizontal" HorizontalAlignment="Right">
    <Button Content="Cancel" Style="{StaticResource SecondaryButton}" Margin="0,0,10,0"/>
    <Button Content="Confirm" Style="{StaticResource PrimaryButton}"/>
</StackPanel>
```

### Destructive Confirmation
```xml
<StackPanel Orientation="Horizontal" HorizontalAlignment="Right">
    <Button Content="Cancel" Style="{StaticResource SecondaryButton}" Margin="0,0,10,0"/>
    <Button Content="Delete" Style="{StaticResource DangerButton}"/>
</StackPanel>
```

### Form Actions
```xml
<StackPanel Orientation="Horizontal" HorizontalAlignment="Right">
    <Button Content="Cancel" Style="{StaticResource SecondaryButton}" Margin="0,0,10,0"/>
    <Button Content="Save" Style="{StaticResource PrimaryButton}"/>
</StackPanel>
```

---

## Color Psychology

### Why These Colors?

**Blue (Primary)**
- ✅ Trust and reliability
- ✅ Professional and corporate
- ✅ Encourages action
- ✅ Most common primary color

**Green (Success)**
- ✅ Positive and safe
- ✅ Completion and success
- ✅ Go ahead signal
- ✅ Reassuring

**Red (Danger)**
- ⚠️ Stop and think
- ⚠️ Destructive action warning
- ⚠️ Requires attention
- ⚠️ Cannot be undone

**Orange (Warning)**
- ⚠️ Proceed with caution
- ⚠️ Important but not critical
- ⚠️ Modification alert
- ⚠️ Review before action

**Gray (Secondary)**
- ✅ Neutral and safe
- ✅ Non-committal
- ✅ Cancellation
- ✅ No consequences

**Cyan (Info)**
- ✅ Informational
- ✅ Non-destructive
- ✅ Read-only actions
- ✅ Helpful

---

## Accessibility Features

### Built-in Accessibility
- ✅ **High Contrast**: All buttons have sufficient contrast ratios
- ✅ **Disabled State**: Clear visual indication when disabled
- ✅ **Hover Feedback**: Visual feedback on mouse hover
- ✅ **Pressed State**: Visual feedback when clicked
- ✅ **Cursor Change**: Hand cursor on hover, arrow when disabled

### WCAG Compliance
- ✅ **Color Contrast**: All text meets WCAG AA standards (4.5:1 minimum)
- ✅ **Focus Indicators**: Keyboard navigation supported
- ✅ **State Changes**: Clear visual state changes

---

## Implementation Examples

### Login Page
```xml
<!-- Primary action -->
<Button Content="LOG IN" Style="{StaticResource PrimaryButton}"/>

<!-- Secondary action -->
<Button Content="EXIT" Style="{StaticResource SecondaryButton}"/>
```

### User Management
```xml
<!-- Add new user -->
<Button Content="Add User" Style="{StaticResource PrimaryButton}"/>

<!-- Edit existing user -->
<Button Content="Edit" Style="{StaticResource WarningButton}"/>

<!-- Delete user -->
<Button Content="Delete" Style="{StaticResource DangerButton}"/>

<!-- View details -->
<Button Content="View" Style="{StaticResource InfoButton}"/>
```

### Ticket Management
```xml
<!-- Assign ticket -->
<Button Content="Assign" Style="{StaticResource PrimaryButton}"/>

<!-- Update ticket -->
<Button Content="Update" Style="{StaticResource WarningButton}"/>

<!-- Resolve ticket -->
<Button Content="Resolve" Style="{StaticResource SuccessButton}"/>

<!-- Close ticket -->
<Button Content="Close Ticket" Style="{StaticResource DangerButton}"/>

<!-- View ticket -->
<Button Content="View" Style="{StaticResource InfoButton}"/>
```

### Form Actions
```xml
<!-- Save form -->
<Button Content="Save Changes" Style="{StaticResource PrimaryButton}"/>

<!-- Cancel form -->
<Button Content="Cancel" Style="{StaticResource SecondaryButton}"/>

<!-- Reset form -->
<Button Content="Reset" Style="{StaticResource DangerButton}"/>
```

---

## Best Practices

### DO ✅
- Use Primary (Blue) for the main action
- Use Danger (Red) for destructive actions
- Use Success (Green) for positive completions
- Use Secondary (Gray) for cancellations
- Place primary action on the right
- Use consistent spacing between buttons
- Provide clear, action-oriented labels

### DON'T ❌
- Don't use multiple primary buttons in one context
- Don't use red for non-destructive actions
- Don't use green for actions that aren't positive
- Don't mix button styles inconsistently
- Don't use vague labels like "OK" or "Yes"
- Don't place destructive actions without confirmation

---

## Button Sizing

### Standard Sizes
```xml
<!-- Default padding -->
Padding="20,10"  <!-- Width: 20px, Height: 10px -->

<!-- Small button -->
Padding="12,6"

<!-- Large button -->
Padding="28,14"

<!-- Full width -->
HorizontalAlignment="Stretch"
```

### Height Guidelines
- **Small**: 32px
- **Medium** (Default): 40px
- **Large**: 48px

---

## Migration Guide

### Updating Existing Buttons

**Before:**
```xml
<Button Content="Save" Background="#007ACC" Foreground="White"/>
```

**After:**
```xml
<Button Content="Save" Style="{StaticResource PrimaryButton}"/>
```

### Common Replacements

| Old Color | New Style | Reason |
|-----------|-----------|--------|
| #007ACC (Blue) | PrimaryButton | Main actions |
| #28A745 (Green) | SuccessButton | Positive actions |
| #DC3545 (Red) | DangerButton | Destructive actions |
| #FFC107 (Yellow) | WarningButton | Caution actions |
| #6C757D (Gray) | SecondaryButton | Neutral actions |

---

## Technical Details

### Style Properties
All button styles include:
- ✅ FontFamily: Roboto
- ✅ FontWeight: SemiBold
- ✅ FontSize: 14px
- ✅ Padding: 20px horizontal, 10px vertical
- ✅ BorderRadius: 4px (rounded corners)
- ✅ Cursor: Hand (pointer)
- ✅ Disabled state styling

### State Management
Each button style handles:
- Normal state
- Hover state (IsMouseOver)
- Pressed state (IsPressed)
- Disabled state (IsEnabled=False)

---

## Summary

✅ **7 professional button styles** defined  
✅ **Material Design colors** for consistency  
✅ **Clear action semantics** (color = meaning)  
✅ **Accessibility compliant** (WCAG AA)  
✅ **Hover and pressed states** for feedback  
✅ **Disabled state styling** for clarity  
✅ **Uniform appearance** across application  

---

**Status:** Complete and ready to use!  
**Location:** `App.xaml` - Application Resources  
**Usage:** Apply `Style="{StaticResource ButtonStyleName}"` to any button

---

**Last Updated:** May 16, 2026  
**Version:** 1.0  
**Design System:** Material Design
