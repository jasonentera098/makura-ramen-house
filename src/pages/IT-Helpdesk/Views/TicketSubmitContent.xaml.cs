using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using IT_Helpdesk.ViewModels;

namespace IT_Helpdesk.Views
{
    /// <summary>
    /// Interaction logic for TicketSubmitContent.xaml
    /// </summary>
    public partial class TicketSubmitContent : UserControl
    {
        private TicketSubmitViewModel? ViewModel => DataContext as TicketSubmitViewModel;

        public TicketSubmitContent()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select Attachment",
                Filter = "All Files (*.*)|*.*|" +
                         "Images (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp|" +
                         "Documents (*.pdf;*.doc;*.docx;*.txt;*.xls;*.xlsx)|*.pdf;*.doc;*.docx;*.txt;*.xls;*.xlsx|" +
                         "Archives (*.zip;*.rar;*.7z)|*.zip;*.rar;*.7z",
                FilterIndex = 1,
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Check file size (10MB limit)
                var fileInfo = new System.IO.FileInfo(openFileDialog.FileName);
                if (fileInfo.Length > 10 * 1024 * 1024) // 10MB in bytes
                {
                    MessageBox.Show(
                        "The selected file exceeds the 10MB size limit. Please choose a smaller file.",
                        "File Too Large",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (ViewModel != null)
                {
                    ViewModel.AttachmentPath = openFileDialog.FileName;
                }
            }
        }

        private void btnRemoveAttachment_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.AttachmentPath = string.Empty;
            }
        }
    }
}
