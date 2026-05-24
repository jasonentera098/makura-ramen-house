using System.Windows;
using System.Windows.Controls;
using IT_Helpdesk.ViewModels;

namespace IT_Helpdesk.Views
{
    public partial class TechnicianReportContent : UserControl
    {
        public TechnicianReportContent()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is TechnicianReportViewModel vm)
            {
                vm.PrintRequested += (s, _) =>
                {
                    var pd = new PrintDialog();
                    if (pd.ShowDialog() == true)
                        pd.PrintVisual(ReportDataGrid, "IT Helpdesk - Assigned Tickets Report");
                };
            }
        }
    }
}
