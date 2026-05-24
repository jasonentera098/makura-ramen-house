# Validation Testing Guide

## Quick Testing Reference

This guide helps you quickly test all the validation features implemented in the IT Helpdesk system.

---

## 1. Testing Strong Password Validation

### Where to Test
- **Add User** (Admin → Users → Add User button)
- **Edit User** (Admin → Users → Select user → Edit button)
- **Change Password** (Any user → Change Password menu)

### Test Cases

#### ❌ Should FAIL - Too Short
```
Password: Test123
Expected: "Password must be at least 8 characters."
```

#### ❌ Should FAIL - No Uppercase
```
Password: test123!@#
Expected: "Password must contain at least one uppercase letter (A–Z)."
```

#### ❌ Should FAIL - No Lowercase
```
Password: TEST123!@#
Expected: "Password must contain at least one lowercase letter (a–z)."
```

#### ❌ Should FAIL - No Digit
```
Password: TestTest!@#
Expected: "Password must contain at least one number (0–9)."
```

#### ❌ Should FAIL - No Special Character
```
Password: Test1234
Expected: "Password must contain at least one special character (e.g. @, #, !, $)."
```

#### ✅ Should PASS - Valid Passwords
```
Password: Test123!
Password: Admin@2024
Password: Secure#Pass1
Password: MyP@ssw0rd
Password: Welcome123!
```

### Visual Feedback to Check
- **Weak** (Red): Less than 3 criteria met
- **Fair** (Orange): 3 criteria met
- **Strong** (Green): 4-5 criteria met
- **Very Strong** (Dark Green): All 6 criteria met

---

## 2. Testing Username Validation

### Where to Test
- **Add User** (Admin → Users → Add User button)
- **Edit User** (Admin → Users → Select user → Edit button)

### Test Cases

#### ❌ Should FAIL - Too Short
```
Username: abc
Expected: "Username must be at least 4 characters."
```

#### ❌ Should FAIL - Too Long
```
Username: thisusernameiswaytoolongandexceedsthirtychars
Expected: "Username must not exceed 30 characters."
```

#### ❌ Should FAIL - Starts with Number
```
Username: 123user
Expected: "Username must start with a letter."
```

#### ❌ Should FAIL - Contains Spaces
```
Username: john doe
Expected: "Username may only contain letters, digits, underscores (_), and hyphens (-)."
```

#### ❌ Should FAIL - Special Characters
```
Username: john@doe
Username: user!name
Username: test#user
Expected: "Username may only contain letters, digits, underscores (_), and hyphens (-)."
```

#### ✅ Should PASS - Valid Usernames
```
Username: john_doe
Username: user123
Username: admin-user
Username: employee_01
Username: tech_support
Username: JohnDoe2024
```

### Visual Feedback to Check
- **Green border**: Username is valid
- **Red border**: Username is invalid
- **Hint text**: Shows format requirements

---

## 3. Testing Philippine Contact Number Validation

### Where to Test
- **Add User** (Admin → Users → Add User button)
- **Edit User** (Admin → Users → Select user → Edit button)

### Test Cases

#### ❌ Should FAIL - Too Short
```
Contact: 0975606146
Expected: "Contact number must be exactly 11 digits."
```

#### ❌ Should FAIL - Too Long
```
Contact: 097560614600
Expected: "Contact number must be exactly 11 digits."
```

#### ❌ Should FAIL - Doesn't Start with 09
```
Contact: 12345678901
Contact: 63975606146
Expected: "Contact number must start with 09."
```

#### ❌ Should FAIL - Contains Non-Digits
```
Contact: 0975-606-146
Contact: 09756061460a
Contact: +639756061460
Expected: "Contact number must contain only digits."
```

#### ✅ Should PASS - Valid Contact Numbers
```
Contact: 09756061460
Contact: 09123456789
Contact: 09987654321
Contact: 09171234567
Contact: (empty/blank - contact is optional)
```

### Visual Feedback to Check
- **Green border**: Contact number is valid
- **Red border**: Contact number is invalid
- **Error message**: Shows specific validation error

---

## 4. Complete User Creation Test Flow

### Step-by-Step Test
1. **Login as Admin**
   - Username: `admin`
   - Password: (your admin password)

2. **Navigate to Users**
   - Click "Users" in navigation bar

3. **Click Add User**
   - Click "Add User" button

4. **Test Invalid Inputs First**
   - Full Name: `Test User`
   - Username: `test` (too short - should show error)
   - Password: `test` (too weak - should show error)
   - Contact: `12345` (invalid format - should show error)
   - Role: `Employee`
   - Department: Select any

5. **Fix to Valid Inputs**
   - Full Name: `Test User`
   - Username: `test_user`
   - Password: `Test123!`
   - Contact: `09123456789`
   - Role: `Employee`
   - Department: Select any

6. **Click Save**
   - Should succeed with "User added successfully" message

---

## 5. Change Password Test Flow

### Step-by-Step Test
1. **Login as Any User**

2. **Navigate to Change Password**
   - Click "Change Password" in navigation bar

3. **Test Invalid Password**
   - Current Password: (enter correct current password)
   - New Password: `weak` (should show error)
   - Confirm Password: `weak`

4. **Test Valid Password**
   - Current Password: (enter correct current password)
   - New Password: `NewPass123!`
   - Confirm Password: `NewPass123!`

5. **Click Save**
   - Should succeed with "Password changed successfully!" message

---

## 6. Real-Time Validation Testing

### Username Field
1. Start typing in username field
2. Watch for:
   - Border color changes (red/green)
   - Error messages appear/disappear
   - Hint text visibility

### Password Field
1. Start typing in password field
2. Watch for:
   - Strength bar fills up
   - Strength label changes (Weak → Fair → Strong → Very Strong)
   - Colors change (Red → Orange → Green → Dark Green)
   - Error messages for missing requirements

### Contact Field
1. Start typing in contact field
2. Watch for:
   - Border color changes (red/green)
   - Error messages appear/disappear
   - Validation on each keystroke

---

## 7. Edge Cases to Test

### Empty Fields
- ✅ Contact number can be empty (optional field)
- ❌ Username cannot be empty
- ❌ Password cannot be empty (when adding user)
- ✅ Password can be empty (when editing user - keeps current password)

### Special Characters in Password
Test these special characters (all should work):
```
! @ # $ % ^ & * ( ) - _ = + [ ] { } | \ ; : ' " , . < > ? /
```

### Username Edge Cases
```
✅ a123 (starts with letter, minimum 4 chars)
✅ user_name_123 (underscores allowed)
✅ user-name-123 (hyphens allowed)
❌ _username (starts with underscore)
❌ -username (starts with hyphen)
```

### Contact Number Edge Cases
```
✅ 09000000000 (all zeros after 09)
✅ 09999999999 (all nines after 09)
❌ 08123456789 (starts with 08)
❌ 00123456789 (starts with 00)
```

---

## 8. Browser/UI Testing Checklist

- [ ] Error messages are clearly visible
- [ ] Error messages are in red color
- [ ] Success indicators (green borders) are visible
- [ ] Password strength bar animates smoothly
- [ ] Hint text appears when needed
- [ ] Validation happens in real-time (as you type)
- [ ] Save button is disabled during validation errors
- [ ] Success messages appear after successful save
- [ ] Form clears after successful save

---

## 9. Security Testing Checklist

- [ ] Cannot create user with weak password
- [ ] Cannot change password to weak password
- [ ] Username uniqueness is enforced
- [ ] Password is not visible in plain text
- [ ] Validation happens on client-side (immediate feedback)
- [ ] Validation also happens on server-side (security)
- [ ] Error messages don't reveal sensitive information

---

## 10. Common Issues and Solutions

### Issue: Validation not showing
**Solution:** Make sure you're typing in the field. Validation triggers on text change.

### Issue: Can't save even with valid input
**Solution:** Check all fields. One invalid field blocks the entire form.

### Issue: Password strength not updating
**Solution:** Make sure you're typing in the password field, not copy-pasting.

### Issue: Contact validation too strict
**Solution:** Contact must be EXACTLY 11 digits starting with 09. No spaces, dashes, or other characters.

---

## Quick Test Script

Copy and paste these test values for quick testing:

### Valid Test User
```
Full Name: Test Employee
Username: test_employee
Password: Test123!
Contact: 09123456789
Role: Employee
Department: (select any)
```

### Invalid Test Cases
```
Username: 123 (too short, starts with number)
Password: test (too short, no uppercase, no number, no special char)
Contact: 12345 (too short, doesn't start with 09)
```

---

## Success Criteria

All validations are working correctly if:
- ✅ Invalid inputs show clear error messages
- ✅ Valid inputs show green borders/success indicators
- ✅ Real-time validation works as you type
- ✅ Cannot save form with invalid data
- ✅ Can save form with all valid data
- ✅ Password strength indicator works correctly
- ✅ All error messages are helpful and specific

---

**Last Updated:** Based on validation implementation
**Tested On:** IT Helpdesk System v1.0
