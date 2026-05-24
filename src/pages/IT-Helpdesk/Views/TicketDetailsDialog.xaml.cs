using System.Windows;
using IT_Helpdesk.ViewModels;

namespace IT_Helpdesk.Views
{
    /// <summary>
    /// Interaction logic for TicketDetailsDialog.xaml
    /// Displays comprehensive ticket information including updates and attachments.
    /// </summary>
    public partial class TicketDetailsDialog : Window
    {
        public TicketDetailsDialog(int ticketId)
        {
            InitializeComponent();
            DataContext = new TicketDetailsViewModel(ticketId);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
