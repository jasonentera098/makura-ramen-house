using System.Windows.Controls;
using IT_Helpdesk.ViewModels;

namespace IT_Helpdesk.Views
{
    /// <summary>
    /// Interaction logic for AdminDashboardContent.xaml
    /// </summary>
    public partial class AdminDashboardContent : UserControl
    {
        public AdminDashboardContent()
        {
            InitializeComponent();
            DataContext = new AdminDashboardViewModel();
        }
    }
}