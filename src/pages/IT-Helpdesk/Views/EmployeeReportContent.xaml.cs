using System.Windows;
using System.Windows.Controls;
using IT_Helpdesk.ViewModels;

namespace IT_Helpdesk.Views
{
    public partial class EmployeeReportContent : UserControl
    {
        public EmployeeReportContent()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is EmployeeReportViewModel vm)
            {
                vm.PrintRequested += (s, _) =>
                {
                    var pd = new PrintDialog();
                    if (pd.ShowDialog() == true)
                        pd.PrintVisual(ReportDataGrid, "IT Helpdesk - My Ticket History");
                };
            }
        }
    }
}
