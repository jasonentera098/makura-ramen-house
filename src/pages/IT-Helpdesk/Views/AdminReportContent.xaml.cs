using System.Windows;
using System.Windows.Controls;
using IT_Helpdesk.ViewModels;

namespace IT_Helpdesk.Views
{
    public partial class AdminReportContent : UserControl
    {
        public AdminReportContent()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is AdminReportViewModel vm)
            {
                vm.PrintRequested += (s, _) =>
                {
                    var pd = new PrintDialog();
                    if (pd.ShowDialog() == true)
                        pd.PrintVisual(ReportDataGrid, "IT Helpdesk - All Tickets Report");
                };
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
