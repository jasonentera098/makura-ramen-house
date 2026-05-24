using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using IT_Helpdesk.Helpers;
using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;

namespace IT_Helpdesk
{
    /// <summary>
    /// Interaction logic for AddUser.xaml — supports both Add and Edit modes.
    /// Pass a User to the constructor to enter Edit mode.
    /// Implements Task 8 requirements: Account Status, Department visibility, Admin restriction, Reset Password.
    /// </summary>
    public partial class AddUser : Window
    {
        private readonly User? _editUser;
        private readonly IUserRepository _userRepo;
        private readonly IDepartmentRepository _deptRepo;
        private bool _isEditMode;
        private bool _isPasswordVisible = false;

        /// <summary>Add mode constructor.</summary>
        public AddUser()
        {
            InitializeComponent();
            _userRepo = new UserRepository(DatabaseConfig.ConnectionString);
            _deptRepo = new DepartmentRepository(DatabaseConfig.ConnectionString);
            _isEditMode = false;
            Loaded += async (_, _) => await LoadDepartmentsAsync();
        }

        /// <summary>Edit mode constructor — pre-populates fields with existing user data.</summary>
        public AddUser(User userToEdit) : this()
        {
            _editUser = userToEdit ?? throw new ArgumentNullException(nameof(userToEdit));
            _isEditMode = true;
        }

        private async Task LoadDepartmentsAsync()
        {
            try
            {
                List<Department> departments = await _deptRepo.GetAllAsync();
                
                // Filter departments for Employee role only (Task 8.2)
                var employeeDepartments = departments.Where(d => 
                    d.DepartmentName != "IT Department").ToList();
                
                cmbDepartment.ItemsSource = employeeDepartments;

                if (_editUser != null)
                    PopulateForEdit();
                else
                    UpdateDepartmentVisibility();
            }
            catch (Exception ex)
            {
                ShowError($"Failed to load departments: {ex.Message}");
            }
        }

        private void PopulateForEdit()
        {
            if (_editUser == null) return;

            Title = "Edit User";
            txtTitle.Text = "Edit User";
            lblPassword.Text = "Password (leave blank to keep current)";
            
            // Show Reset Password button and Account Status in edit mode (Task 8.8, 8.9)
            btnResetPassword.Visibility = Visibility.Visible;
            lblAccountStatus.Visibility = Visibility.Visible;
            cmbAccountStatus.Visibility = Visibility.Visible;

            txtFullName.Text = _editUser.FullName;
            txtUsername.Text = _editUser.Username;
            txtContact.Text = _editUser.ContactNumber;

            // Select matching role in ComboBox
            foreach (ComboBoxItem item in cmbRole.Items)
            {
                if (item.Content?.ToString() == _editUser.Role)
                {
                    cmbRole.SelectedItem = item;
                    break;
                }
            }

            // Select matching department
            cmbDepartment.SelectedValue = _editUser.DepartmentID;
            
            // Select matching account status
            foreach (ComboBoxItem item in cmbAccountStatus.Items)
            {
                if (item.Content?.ToString() == _editUser.AccountStatus)
                {
                    cmbAccountStatus.SelectedItem = item;
                    break;
                }
            }
            
            UpdateDepartmentVisibility();
        }

        /// <summary>
        /// Updates Department field visibility based on selected role (Task 8.2, 8.3).
        /// Department is only shown for Employee role.
        /// </summary>
        private void UpdateDepartmentVisibility()
        {
            string selectedRole = (cmbRole.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
            
            if (selectedRole == "Employee")
            {
                lblDepartment.Visibility = Visibility.Visible;
                cmbDepartment.Visibility = Visibility.Visible;
            }
            else
            {
                lblDepartment.Visibility = Visibility.Collapsed;
                cmbDepartment.Visibility = Visibility.Collapsed;
                
                // Set IT Department as default for Technician/Admin
                var itDept = (cmbDepartment.ItemsSource as List<Department>)?.FirstOrDefault(d => d.DepartmentName == "IT Department");
                if (itDept != null)
                    cmbDepartment.SelectedValue = itDept.DepartmentID;
            }
        }

        private void cmbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDepartmentVisibility();
        }

        // ── Real-time validation handlers ────────────────────────────────────

        private void txtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            string value = txtUsername.Text.Trim();
            if (string.IsNullOrEmpty(value))
            {
                txtUsernameError.Visibility = Visibility.Collapsed;
                txtUsernameHint.Visibility = Visibility.Collapsed;
                txtUsername.BorderBrush = System.Windows.Media.Brushes.Transparent;
                return;
            }

            var result = ValidationHelper.IsValidUsername(value);
            if (result.IsValid)
            {
                txtUsernameError.Visibility = Visibility.Collapsed;
                txtUsernameHint.Visibility = Visibility.Collapsed;
                txtUsername.BorderBrush = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(0, 211, 153));
            }
            else
            {
                txtUsernameError.Text = result.ErrorMessage;
                txtUsernameError.Visibility = Visibility.Visible;
                txtUsernameHint.Visibility = Visibility.Visible;
                txtUsername.BorderBrush = System.Windows.Media.Brushes.Red;
            }
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_isPasswordVisible) return; // Don't update if showing visible text
            
            string value = txtPassword.Password;
            txtPasswordVisible.Text = value; // Keep in sync
            
            if (string.IsNullOrEmpty(value))
            {
                pnlPasswordStrength.Visibility = Visibility.Collapsed;
                txtPasswordStrengthLabel.Visibility = Visibility.Collapsed;
                txtPasswordHint.Visibility = Visibility.Collapsed;
                txtPasswordError.Visibility = Visibility.Collapsed;
                return;
            }

            // Show strength bar and hint
            pnlPasswordStrength.Visibility = Visibility.Visible;
            txtPasswordStrengthLabel.Visibility = Visibility.Visible;
            txtPasswordHint.Visibility = Visibility.Visible;

            string strength = ValidationHelper.GetPasswordStrength(value);
            UpdatePasswordStrengthBar(strength);

            var result = ValidationHelper.IsStrongPassword(value);
            if (result.IsValid)
            {
                txtPasswordError.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtPasswordError.Text = result.ErrorMessage;
                txtPasswordError.Visibility = Visibility.Visible;
            }
        }

        private void txtPasswordVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isPasswordVisible) return; // Don't update if showing password box
            
            string value = txtPasswordVisible.Text;
            txtPassword.Password = value; // Keep in sync
            
            if (string.IsNullOrEmpty(value))
            {
                pnlPasswordStrength.Visibility = Visibility.Collapsed;
                txtPasswordStrengthLabel.Visibility = Visibility.Collapsed;
                txtPasswordHint.Visibility = Visibility.Collapsed;
                txtPasswordError.Visibility = Visibility.Collapsed;
                return;
            }

            // Show strength bar and hint
            pnlPasswordStrength.Visibility = Visibility.Visible;
            txtPasswordStrengthLabel.Visibility = Visibility.Visible;
            txtPasswordHint.Visibility = Visibility.Visible;

            string strength = ValidationHelper.GetPasswordStrength(value);
            UpdatePasswordStrengthBar(strength);

            var result = ValidationHelper.IsStrongPassword(value);
            if (result.IsValid)
            {
                txtPasswordError.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtPasswordError.Text = result.ErrorMessage;
                txtPasswordError.Visibility = Visibility.Visible;
            }
        }

        private void btnTogglePassword_Click(object sender, RoutedEventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;

            if (_isPasswordVisible)
            {
                // Show password as plain text
                txtPasswordVisible.Text = txtPassword.Password;
                txtPasswordVisible.Visibility = Visibility.Visible;
                txtPassword.Visibility = Visibility.Collapsed;
                txtPasswordEyeIcon.Text = "👁‍🗨"; // Eye with slash
            }
            else
            {
                // Hide password
                txtPassword.Password = txtPasswordVisible.Text;
                txtPassword.Visibility = Visibility.Visible;
                txtPasswordVisible.Visibility = Visibility.Collapsed;
                txtPasswordEyeIcon.Text = "👁"; // Regular eye
            }
        }

        private void UpdatePasswordStrengthBar(string strength)
        {
            var gray   = System.Windows.Media.Color.FromRgb(224, 224, 224);
            var red    = System.Windows.Media.Color.FromRgb(220, 53, 69);
            var orange = System.Windows.Media.Color.FromRgb(255, 152, 0);
            var green  = System.Windows.Media.Color.FromRgb(0, 211, 153);
            var dkGreen= System.Windows.Media.Color.FromRgb(0, 150, 100);

            var (color, filled, label, labelColor) = strength switch
            {
                "Weak"        => (red,    1, "Weak",        red),
                "Fair"        => (orange, 2, "Fair",        orange),
                "Strong"      => (green,  3, "Strong",      green),
                "Very Strong" => (dkGreen,4, "Very Strong", dkGreen),
                _             => (gray,   0, "",            gray)
            };

            var bars = new[] { barStrength1, barStrength2, barStrength3, barStrength4 };
            for (int i = 0; i < bars.Length; i++)
                bars[i].Background = new System.Windows.Media.SolidColorBrush(i < filled ? color : gray);

            txtPasswordStrengthLabel.Text = label;
            txtPasswordStrengthLabel.Foreground = new System.Windows.Media.SolidColorBrush(labelColor);
        }

        private void txtContact_TextChanged(object sender, TextChangedEventArgs e)
        {
            string value = txtContact.Text.Trim();
            if (string.IsNullOrEmpty(value))
            {
                txtContactError.Visibility = Visibility.Collapsed;
                txtContact.BorderBrush = System.Windows.Media.Brushes.Transparent;
                return;
            }

            var result = ValidationHelper.IsValidPhilippineContact(value);
            if (result.IsValid)
            {
                txtContactError.Visibility = Visibility.Collapsed;
                txtContact.BorderBrush = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(0, 211, 153));
            }
            else
            {
                txtContactError.Text = result.ErrorMessage;
                txtContactError.Visibility = Visibility.Visible;
                txtContact.BorderBrush = System.Windows.Media.Brushes.Red;
            }
        }

        private async void btnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            if (_editUser == null) return;

            var result = MessageBox.Show(
                "Reset password to temporary password 'Temp123'?\nUser will need to change it on next login.",
                "Reset Password",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                txtPassword.Password = "Temp123";
                MessageBox.Show("Password will be reset to 'Temp123' when you click Save.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>Shows an inline validation error message.</summary>
        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Visibility = Visibility.Visible;
        }

        /// <summary>Clears the inline validation error message.</summary>
        private void ClearError()
        {
            txtError.Text = string.Empty;
            txtError.Visibility = Visibility.Collapsed;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ClearError();

            string fullName = txtFullName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string password = _isPasswordVisible ? txtPasswordVisible.Text : txtPassword.Password;
            string role = (cmbRole.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
            int departmentId = cmbDepartment.SelectedValue is int deptId ? deptId : 0;
            string contact = txtContact.Text.Trim();
            string accountStatus = _isEditMode 
                ? (cmbAccountStatus.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Active"
                : "Active";

            // Validation using ValidationHelper
            var fullNameCheck = ValidationHelper.IsNotEmpty(fullName, "Full Name");
            if (!fullNameCheck.IsValid)
            {
                ShowError(fullNameCheck.ErrorMessage);
                txtFullName.Focus();
                return;
            }

            var usernameCheck = ValidationHelper.IsValidUsername(username);
            if (!usernameCheck.IsValid)
            {
                ShowError(usernameCheck.ErrorMessage);
                txtUsername.Focus();
                return;
            }

            var roleCheck = ValidationHelper.IsNotEmpty(role, "Role");
            if (!roleCheck.IsValid)
            {
                ShowError("Please select a Role.");
                cmbRole.Focus();
                return;
            }

            // Task 8.4: Restrict system to one Admin account only
            if (role == "Administrator" && !_isEditMode)
            {
                int activeAdminCount = await _userRepo.CountActiveAdminsAsync();
                if (activeAdminCount >= 1)
                {
                    ShowError("Cannot add another Administrator. The system is restricted to one Admin account only.");
                    cmbRole.Focus();
                    return;
                }
            }

            // Department validation only for Employee role
            if (role == "Employee" && departmentId == 0)
            {
                ShowError("Please select a Department.");
                cmbDepartment.Focus();
                return;
            }

            if (_editUser == null)
            {
                var passwordCheck = ValidationHelper.IsStrongPassword(password);
                if (!passwordCheck.IsValid)
                {
                    ShowError(passwordCheck.ErrorMessage);
                    txtPassword.Focus();
                    return;
                }
            }
            else if (!string.IsNullOrEmpty(password))
            {
                // In edit mode, only validate if a new password was entered
                var passwordCheck = ValidationHelper.IsStrongPassword(password);
                if (!passwordCheck.IsValid)
                {
                    ShowError(passwordCheck.ErrorMessage);
                    txtPassword.Focus();
                    return;
                }
            }

            if (!string.IsNullOrEmpty(contact))
            {
                var contactCheck = ValidationHelper.IsValidPhilippineContact(contact);
                if (!contactCheck.IsValid)
                {
                    ShowError(contactCheck.ErrorMessage);
                    txtContact.Focus();
                    return;
                }
            }

            try
            {
                btnSave.IsEnabled = false;

                if (_editUser == null)
                {
                    // Add mode — check username uniqueness
                    bool exists = await _userRepo.UsernameExistsAsync(username);
                    if (exists)
                    {
                        ShowError("Username already exists. Please choose a different username.");
                        btnSave.IsEnabled = true;
                        txtUsername.Focus();
                        return;
                    }

                    // For non-Employee roles, use IT Department
                    if (role != "Employee")
                    {
                        var allDepts = await _deptRepo.GetAllAsync();
                        var itDept = allDepts.FirstOrDefault(d => d.DepartmentName == "IT Department");
                        if (itDept != null)
                            departmentId = itDept.DepartmentID;
                    }

                    var newUser = new User
                    {
                        FullName = fullName,
                        Username = username,
                        PasswordHash = PasswordHasher.HashPassword(password),
                        Role = role,
                        DepartmentID = departmentId,
                        ContactNumber = contact,
                        AccountStatus = "Active",
                        CreatedAt = DateTime.Now
                    };

                    await _userRepo.InsertAsync(newUser);
                    MessageBox.Show("User added successfully.", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Edit mode — check username uniqueness (excluding current user)
                    bool exists = await _userRepo.UsernameExistsAsync(username, _editUser.UserID);
                    if (exists)
                    {
                        ShowError("Username already exists. Please choose a different username.");
                        btnSave.IsEnabled = true;
                        txtUsername.Focus();
                        return;
                    }

                    // For non-Employee roles, use IT Department
                    if (role != "Employee")
                    {
                        var allDepts = await _deptRepo.GetAllAsync();
                        var itDept = allDepts.FirstOrDefault(d => d.DepartmentName == "IT Department");
                        if (itDept != null)
                            departmentId = itDept.DepartmentID;
                    }

                    _editUser.FullName = fullName;
                    _editUser.Username = username;
                    _editUser.Role = role;
                    _editUser.DepartmentID = departmentId;
                    _editUser.ContactNumber = contact;
                    _editUser.AccountStatus = accountStatus;

                    // Only re-hash password if it was changed (Task 8.7)
                    if (!string.IsNullOrEmpty(password))
                        _editUser.PasswordHash = PasswordHasher.HashPassword(password);

                    await _userRepo.UpdateAsync(_editUser);
                    MessageBox.Show("User updated successfully.", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ShowError($"Error saving user: {ex.Message}");
                btnSave.IsEnabled = true;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
