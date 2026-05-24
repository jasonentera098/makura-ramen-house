using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using IT_Helpdesk.Helpers;
using IT_Helpdesk.ViewModels;

namespace IT_Helpdesk.Views
{
    public partial class ChangePasswordContent : UserControl
    {
        private bool _isCurrentPasswordVisible = false;
        private bool _isNewPasswordVisible = false;
        private bool _isConfirmPasswordVisible = false;

        public ChangePasswordContent()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is ChangePasswordViewModel oldVm)
                oldVm.PasswordChangedSuccessfully -= ClearPasswordFields;

            if (e.NewValue is ChangePasswordViewModel newVm)
                newVm.PasswordChangedSuccessfully += ClearPasswordFields;
        }

        private void ClearPasswordFields()
        {
            txtCurrentPassword.Clear();
            txtCurrentPasswordVisible.Clear();
            txtNewPassword.Clear();
            txtNewPasswordVisible.Clear();
            txtConfirmPassword.Clear();
            txtConfirmPasswordVisible.Clear();
            pnlPasswordStrength.Visibility = Visibility.Collapsed;
            pnlStrengthLabel.Visibility = Visibility.Collapsed;
        }

        private void txtNewPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_isNewPasswordVisible) return;
            
            string value = txtNewPassword.Password;
            txtNewPasswordVisible.Text = value;
            
            if (string.IsNullOrEmpty(value))
            {
                pnlPasswordStrength.Visibility = Visibility.Collapsed;
                pnlStrengthLabel.Visibility = Visibility.Collapsed;
                return;
            }

            pnlPasswordStrength.Visibility = Visibility.Visible;
            pnlStrengthLabel.Visibility = Visibility.Visible;
            UpdateStrengthBar(ValidationHelper.GetPasswordStrength(value));
        }

        private void txtNewPasswordVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isNewPasswordVisible) return;
            
            string value = txtNewPasswordVisible.Text;
            txtNewPassword.Password = value;
            
            if (string.IsNullOrEmpty(value))
            {
                pnlPasswordStrength.Visibility = Visibility.Collapsed;
                pnlStrengthLabel.Visibility = Visibility.Collapsed;
                return;
            }

            pnlPasswordStrength.Visibility = Visibility.Visible;
            pnlStrengthLabel.Visibility = Visibility.Visible;
            UpdateStrengthBar(ValidationHelper.GetPasswordStrength(value));
        }

        private void UpdateStrengthBar(string strength)
        {
            var gray    = Color.FromRgb(224, 224, 224);
            var red     = Color.FromRgb(220, 53, 69);
            var orange  = Color.FromRgb(255, 152, 0);
            var green   = Color.FromRgb(0, 211, 153);
            var dkGreen = Color.FromRgb(0, 150, 100);

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
                bars[i].Background = new SolidColorBrush(i < filled ? color : gray);

            txtPasswordStrengthLabel.Text = label;
            txtPasswordStrengthLabel.Foreground = new SolidColorBrush(labelColor);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ChangePasswordViewModel vm)
            {
                string currentPwd = _isCurrentPasswordVisible ? txtCurrentPasswordVisible.Text : txtCurrentPassword.Password;
                string newPwd = _isNewPasswordVisible ? txtNewPasswordVisible.Text : txtNewPassword.Password;
                string confirmPwd = _isConfirmPasswordVisible ? txtConfirmPasswordVisible.Text : txtConfirmPassword.Password;
                
                vm.ExecuteSave(currentPwd, newPwd, confirmPwd);
            }
        }

        private void btnToggleCurrentPassword_Click(object sender, RoutedEventArgs e)
        {
            _isCurrentPasswordVisible = !_isCurrentPasswordVisible;

            if (_isCurrentPasswordVisible)
            {
                txtCurrentPasswordVisible.Text = txtCurrentPassword.Password;
                txtCurrentPasswordVisible.Visibility = Visibility.Visible;
                txtCurrentPassword.Visibility = Visibility.Collapsed;
                txtCurrentEyeIcon.Text = "👁‍🗨";
            }
            else
            {
                txtCurrentPassword.Password = txtCurrentPasswordVisible.Text;
                txtCurrentPassword.Visibility = Visibility.Visible;
                txtCurrentPasswordVisible.Visibility = Visibility.Collapsed;
                txtCurrentEyeIcon.Text = "👁";
            }
        }

        private void btnToggleNewPassword_Click(object sender, RoutedEventArgs e)
        {
            _isNewPasswordVisible = !_isNewPasswordVisible;

            if (_isNewPasswordVisible)
            {
                txtNewPasswordVisible.Text = txtNewPassword.Password;
                txtNewPasswordVisible.Visibility = Visibility.Visible;
                txtNewPassword.Visibility = Visibility.Collapsed;
                txtNewEyeIcon.Text = "👁‍🗨";
            }
            else
            {
                txtNewPassword.Password = txtNewPasswordVisible.Text;
                txtNewPassword.Visibility = Visibility.Visible;
                txtNewPasswordVisible.Visibility = Visibility.Collapsed;
                txtNewEyeIcon.Text = "👁";
            }
        }

        private void btnToggleConfirmPassword_Click(object sender, RoutedEventArgs e)
        {
            _isConfirmPasswordVisible = !_isConfirmPasswordVisible;

            if (_isConfirmPasswordVisible)
            {
                txtConfirmPasswordVisible.Text = txtConfirmPassword.Password;
                txtConfirmPasswordVisible.Visibility = Visibility.Visible;
                txtConfirmPassword.Visibility = Visibility.Collapsed;
                txtConfirmEyeIcon.Text = "👁‍🗨";
            }
            else
            {
                txtConfirmPassword.Password = txtConfirmPasswordVisible.Text;
                txtConfirmPassword.Visibility = Visibility.Visible;
                txtConfirmPasswordVisible.Visibility = Visibility.Collapsed;
                txtConfirmEyeIcon.Text = "👁";
            }
        }
    }
}
