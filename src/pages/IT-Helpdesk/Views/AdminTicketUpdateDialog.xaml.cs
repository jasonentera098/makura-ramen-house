using System.Windows;
using IT_Helpdesk.ViewModels;

namespace IT_Helpdesk.Views
{
    public partial class AdminTicketUpdateDialog : Window
    {
        public AdminTicketUpdateDialog(int ticketId)
        {
            InitializeComponent();
            
            // Create and set the ViewModel for the TicketUpdateContent
            var viewModel = new TicketUpdateViewModel(ticketId);
            ticketUpdateContent.DataContext = viewModel;
            
            // Subscribe to the close event from the ViewModel
            viewModel.CloseRequested += (s, e) =>
            {
                DialogResult = true;
                Close();
            };
        }

        private void ticketUpdateContent_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
