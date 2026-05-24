using System.Windows.Controls;
using IT_Helpdesk.ViewModels;

namespace IT_Helpdesk.Views
{
    /// <summary>
    /// Interaction logic for EmployeeMyTicketsContent.xaml
    /// Displays all tickets submitted by the current employee with search and filter functionality.
    /// </summary>
    public partial class EmployeeMyTicketsContent : UserControl
    {
        public EmployeeMyTicketsContent()
        {
            InitializeComponent();
            DataContext = new EmployeeMyTicketsViewModel();
        }
    }
}
