using System.Windows;
using System.Windows.Controls;

namespace IT_Helpdesk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml (Login Screen)
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isPasswordVisible = false;

        public MainWindow()
        {
            InitializeComponent();

            // Wire up PasswordBox — PasswordBox.Password is not bindable, so we pass it
            // to the ViewModel in code-behind whenever it changes.
            txtPassword.PasswordChanged += OnPasswordChanged;
            txtPasswordVisible.TextChanged += OnPasswordVisibleChanged;
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is IHasPassword vm && !_isPasswordVisible)
            {
                vm.Password = txtPassword.Password;
                txtPasswordVisible.Text = txtPassword.Password;
            }
        }

        private void OnPasswordVisibleChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is IHasPassword vm && _isPasswordVisible)
            {
                vm.Password = txtPasswordVisible.Text;
                txtPassword.Password = txtPasswordVisible.Text;
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
                txtEyeIcon.Text = "👁‍🗨"; // Eye with slash
            }
            else
            {
                // Hide password
                txtPassword.Password = txtPasswordVisible.Text;
                txtPassword.Visibility = Visibility.Visible;
                txtPasswordVisible.Visibility = Visibility.Collapsed;
                txtEyeIcon.Text = "👁"; // Regular eye
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            // Show confirmation dialog
            var result = MessageBox.Show(
                "Are you sure you want to exit the application?",
                "Exit Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Properly shutdown the application
                Application.Current.Shutdown();
            }
        }
    }

    /// <summary>
    /// Minimal interface so code-behind can pass the PasswordBox value to any ViewModel
    /// without a direct dependency on LoginViewModel.
    /// </summary>
    public interface IHasPassword
    {
        string Password { get; set; }
    }
}
