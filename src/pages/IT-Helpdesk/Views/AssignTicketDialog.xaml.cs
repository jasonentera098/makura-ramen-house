using System.Collections.Generic;
using System.Windows;
using IT_Helpdesk.Models;

namespace IT_Helpdesk.Views
{
    public partial class AssignTicketDialog : Window
    {
        public int SelectedTechnicianId { get; private set; }

        public AssignTicketDialog(int ticketId, string ticketTitle, List<TechnicianWorkloadInfo> technicians)
        {
            InitializeComponent();
            TicketInfoText.Text = $"Assigning Ticket #{ticketId}: {ticketTitle}";
            TechnicianListBox.ItemsSource = technicians;
        }

        private void TechnicianListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            bool hasSelection = TechnicianListBox.SelectedItem != null;
            AssignButton.IsEnabled = hasSelection;
            
            if (hasSelection && TechnicianListBox.SelectedItem is TechnicianWorkloadInfo selected)
            {
                SelectionHintText.Text = $"Ready to assign to: {selected.FullName} ({selected.Status})";
            }
            else
            {
                SelectionHintText.Text = "Please select a technician from the list above";
            }
        }

        private void AssignButton_Click(object sender, RoutedEventArgs e)
        {
            if (TechnicianListBox.SelectedItem is TechnicianWorkloadInfo selected)
            {
                SelectedTechnicianId = selected.UserID;
                DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
