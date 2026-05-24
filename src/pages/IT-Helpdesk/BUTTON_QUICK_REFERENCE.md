# Button Style Quick Reference

## 🎨 Professional Button Colors

### Usage Guide

| Button Style | Color | Use For | Example |
|--------------|-------|---------|---------|
| **PrimaryButton** | 🔵 Blue | Main actions | Save, Submit, Login |
| **SuccessButton** | ✅ Green | Positive actions | Approve, Complete, Resolve |
| **DangerButton** | 🔴 Red | Destructive actions | Delete, Remove, Cancel |
| **WarningButton** | 🟠 Orange | Caution actions | Update, Edit, Modify |
| **SecondaryButton** | ⚪ Gray | Neutral actions | Cancel, Close, Back |
| **InfoButton** | 🔷 Cyan | Informational | View, Details, Info |
| **OutlineButton** | ⬜ Transparent | Subtle actions | Learn More, See More |

---

## 📝 Quick Examples

### Primary Action (Blue)
```xml
<Button Content="Save" Style="{StaticResource PrimaryButton}"/>
```

### Success Action (Green)
```xml
<Button Content="Approve" Style="{StaticResource SuccessButton}"/>
```

### Danger Action (Red)
```xml
<Button Content="Delete" Style="{StaticResource DangerButton}"/>
```

### Warning Action (Orange)
```xml
<Button Content="Update" Style="{StaticResource WarningButton}"/>
```

### Secondary Action (Gray)
```xml
<Button Content="Cancel" Style="{StaticResource SecondaryButton}"/>
```

### Info Action (Cyan)
```xml
<Button Content="View" Style="{StaticResource InfoButton}"/>
```

---

## 🎯 Common Patterns

### Dialog Buttons
```xml
<StackPanel Orientation="Horizontal" HorizontalAlignment="Right">
    <Button Content="Cancel" Style="{StaticResource SecondaryButton}" Margin="0,0,10,0"/>
    <Button Content="Save" Style="{StaticResource PrimaryButton}"/>
</StackPanel>
```

### Delete Confirmation
```xml
<StackPanel Orientation="Horizontal" HorizontalAlignment="Right">
    <Button Content="Cancel" Style="{StaticResource SecondaryButton}" Margin="0,0,10,0"/>
    <Button Content="Delete" Style="{StaticResource DangerButton}"/>
</StackPanel>
```

### Action Bar
```xml
<StackPanel Orientation="Horizontal">
    <Button Content="Add" Style="{StaticResource PrimaryButton}" Margin="0,0,10,0"/>
    <Button Content="Edit" Style="{StaticResource WarningButton}" Margin="0,0,10,0"/>
    <Button Content="Delete" Style="{StaticResource DangerButton}" Margin="0,0,10,0"/>
    <Button Content="View" Style="{StaticResource InfoButton}"/>
</StackPanel>
```

---

## ✅ DO's and ❌ DON'Ts

### DO ✅
- Use Blue for primary actions
- Use Red for destructive actions
- Use Green for positive completions
- Use Gray for cancellations
- Place primary action on the right

### DON'T ❌
- Don't use Red for non-destructive actions
- Don't use multiple primary buttons
- Don't use vague labels
- Don't mix styles inconsistently

---

## 🎨 Color Codes

| Style | Normal | Hover | Pressed |
|-------|--------|-------|---------|
| Primary | #2196F3 | #1976D2 | #1565C0 |
| Success | #4CAF50 | #388E3C | #2E7D32 |
| Danger | #F44336 | #D32F2F | #C62828 |
| Warning | #FF9800 | #F57C00 | #EF6C00 |
| Secondary | #607D8B | #546E7A | #455A64 |
| Info | #00BCD4 | #0097A7 | #00838F |

---

**Location:** `App.xaml` - Application Resources  
**Font:** Roboto, SemiBold, 14px  
**Padding:** 20px horizontal, 10px vertical  
**Border Radius:** 4px
