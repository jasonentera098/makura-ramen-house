using System;
using System.Windows;
using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;
using IT_Helpdesk.Services;

namespace IT_Helpdesk
{
    /// <summary>
    /// Interaction logic for DeleteConfirmation.xaml
    /// Pass a User to the constructor to show their details in the confirmation message.
    /// DialogResult = true means the user confirmed deletion.
    /// </summary>
    public partial class DeleteConfirmation : Window
    {
        private readonly User _userToDelete;
        private readonly IUserService _userService;

        public DeleteConfirmation(User userToDelete)
        {
            InitializeComponent();
            _userToDelete = userToDelete ?? throw new ArgumentNullException(nameof(userToDelete));
            
            // Initialize service with dependency checking
            var connStr = DatabaseConfig.ConnectionString;
            var userRepo = new UserRepository(connStr);
            var ticketRepo = new TicketRepository(connStr);
            _userService = new UserService(userRepo, ticketRepo);

            txtUserInfo.Text = $"{_userToDelete.FullName} (@{_userToDelete.Username})";
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnDelete.IsEnabled = false;
                btnCancel.IsEnabled = false;

                // Block deletion of Administrator accounts
                if (_userToDelete.Role == "Administrator")
                {
                    MessageBox.Show("Administrator accounts cannot be deleted.", "Not Allowed",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    btnDelete.IsEnabled = true;
                    btnCancel.IsEnabled = true;
                    return;
                }

                // Check for dependent tickets and delete user
                bool success = await _userService.DeleteUserAsync(_userToDelete.UserID);

                if (success)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Failed to delete the user. Please try again.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    btnDelete.IsEnabled = true;
                    btnCancel.IsEnabled = true;
                }
            }
            catch (InvalidOperationException ex)
            {
                // Handle dependency errors (user has associated tickets)
                MessageBox.Show(ex.Message, "Cannot Delete User",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                btnDelete.IsEnabled = true;
                btnCancel.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting user: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                btnDelete.IsEnabled = true;
                btnCancel.IsEnabled = true;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
