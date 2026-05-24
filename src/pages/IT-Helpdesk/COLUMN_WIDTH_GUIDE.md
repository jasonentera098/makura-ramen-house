# DataGrid Column Width Guide

## Quick Reference: Column Width Patterns

### User Management Grid
```
┌────────┬──────────────────┬──────────────┬────────────┬──────────────┬─────────┐
│   ID   │   Full Name      │  Username    │    Role    │  Contact No. │ Status  │
│  0.5*  │      2*          │    1.5*      │    1.2*    │     1.5*     │   1*    │
│  50px  │     150px        │   120px      │   120px    │    130px     │  90px   │
└────────┴──────────────────┴──────────────┴────────────┴──────────────┴─────────┘
```

### Ticket Management Grid
```
┌────────┬────────────────────────────┬────────────┬──────────┬──────────────┐
│   ID   │          Title             │   Status   │ Priority │ Assigned To  │
│  0.5*  │            3*              │    1.3*    │    1*    │     1.5*     │
│  50px  │          200px             │   120px    │  100px   │    140px     │
└────────┴────────────────────────────┴────────────┴──────────┴──────────────┘
```

### Dashboard Recent Tickets Grid
```
┌────────┬────────────────────────────┬────────────┬──────────┐
│   ID   │          Title             │   Status   │ Priority │
│  0.5*  │            3*              │    1.3*    │    1*    │
│  50px  │          200px             │   120px    │  100px   │
└────────┴────────────────────────────┴────────────┴──────────┘
```

## Column Width Ratios Explained

### Small Columns (0.5* - 1*)
**Use for:**
- ID numbers
- Status badges
- Priority badges
- Action icons

**Examples:**
- ID: `0.5*` - Very compact, just enough for numbers
- Status: `1*` - Standard badge size
- Priority: `1*` - Standard badge size

### Medium Columns (1.2* - 1.5*)
**Use for:**
- Usernames
- Contact numbers
- Assigned technician names
- Role badges (slightly wider)

**Examples:**
- Username: `1.5*` - Comfortable reading space
- Contact: `1.5*` - Fits phone numbers well
- Role: `1.2*` - Slightly wider for badge text

### Large Columns (2* - 3*)
**Use for:**
- Full names
- Ticket titles
- Descriptions
- Main content fields

**Examples:**
- Full Name: `2*` - Plenty of space for names
- Title: `3*` - Maximum space for ticket titles

## Responsive Behavior

### On Normal Window (1024px wide)
```
ID: 50px (min)
Title: 400px
Status: 150px
Priority: 120px
Assigned: 180px
```

### On Maximized Window (1920px wide)
```
ID: 90px (scaled)
Title: 750px (scaled)
Status: 280px (scaled)
Priority: 220px (scaled)
Assigned: 340px (scaled)
```

### On Small Window (800px wide)
```
ID: 50px (min enforced)
Title: 200px (min enforced)
Status: 120px (min enforced)
Priority: 100px (min enforced)
Assigned: 140px (min enforced)
```

## Best Practices

### ✅ DO:
- Use star-based widths for responsive scaling
- Set MinWidth to prevent columns from becoming too narrow
- Give more space to content-heavy columns (titles, names)
- Keep badge columns consistent (1* - 1.3*)
- Use 0.5* for very compact columns (IDs)

### ❌ DON'T:
- Use fixed pixel widths (except for MinWidth)
- Make ID columns too wide
- Make title columns too narrow
- Forget to set MinWidth values
- Use different ratios for similar columns across views

## Column Type Guidelines

### ID Columns
```xml
<DataGridTextColumn Header="ID"
                    Binding="{Binding TicketID}"
                    Width="0.5*"
                    MinWidth="50"/>
```

### Title/Name Columns
```xml
<DataGridTextColumn Header="Title"
                    Binding="{Binding Title}"
                    Width="3*"
                    MinWidth="200"/>
```

### Badge Columns (Status/Priority/Role)
```xml
<DataGridTemplateColumn Header="Status" 
                        Width="1.3*" 
                        MinWidth="120"/>
```

### Contact/Username Columns
```xml
<DataGridTextColumn Header="Username"
                    Binding="{Binding Username}"
                    Width="1.5*"
                    MinWidth="120"/>
```

## Testing Checklist

- [ ] Columns scale proportionally when maximizing window
- [ ] No large gaps between columns
- [ ] Minimum widths prevent columns from being too narrow
- [ ] Text truncates properly with ellipsis when needed
- [ ] Badges display correctly at all window sizes
- [ ] Horizontal scrollbar appears only when necessary
- [ ] All columns visible without horizontal scrolling at 1024px+

## Maintenance Notes

When adding new DataGrid columns:
1. Determine column type (ID, content, badge, etc.)
2. Choose appropriate star width from this guide
3. Set MinWidth based on content type
4. Test at multiple window sizes
5. Ensure consistency with existing columns

---

**Last Updated**: May 16, 2026
**Applies To**: All DataGrid controls in the application
