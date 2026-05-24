using System.IO;
using System.Windows;
using IT_Helpdesk.Models;
using IT_Helpdesk.Services;

namespace IT_Helpdesk.ViewModels
{
    public class TicketSubmitViewModel : ViewModelBase
    {
        private readonly ITicketService _ticketService;
        private readonly IFileService _fileService;

        private string _title = string.Empty;
        private string _description = string.Empty;
        private string? _selectedCategory;
        private string? _selectedPriority;
        private string _attachmentPath = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isSubmitting;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string? SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        public string? SelectedPriority
        {
            get => _selectedPriority;
            set => SetProperty(ref _selectedPriority, value);
        }

        public string AttachmentPath
        {
            get => _attachmentPath;
            set => SetProperty(ref _attachmentPath, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsSubmitting
        {
            get => _isSubmitting;
            set => SetProperty(ref _isSubmitting, value);
        }

        public RelayCommand SubmitCommand { get; }

        public TicketSubmitViewModel(
            ITicketService ticketService,
            IFileService fileService)
        {
            _ticketService = ticketService;
            _fileService = fileService;

            SubmitCommand = new RelayCommand(async () => await SubmitTicketAsync(), CanSubmit);
        }

        private bool CanSubmit()
        {
            return !IsSubmitting;
        }

        private async Task SubmitTicketAsync()
        {
            // Validate required fields
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = "Title is required. Please enter a brief description of the issue.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Description))
            {
                ErrorMessage = "Description is required. Please describe the issue in detail.";
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedCategory))
            {
                ErrorMessage = "Please select a category for this ticket.";
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedPriority))
            {
                ErrorMessage = "Please select a priority level for this ticket.";
                return;
            }

            // Ensure user is authenticated
            if (SessionManager.CurrentUser == null)
            {
                ErrorMessage = "User session not found. Please log in again.";
                return;
            }

            IsSubmitting = true;

            try
            {
                // Create ticket record with only user-provided data
                var ticket = new Ticket
                {
                    Title = Title.Trim(),
                    Description = Description.Trim(),
                    Category = SelectedCategory,
                    Priority = SelectedPriority
                    // Status, DateSubmitted, SubmittedBy, DepartmentID, AssignedTo are auto-set by the service
                };

                int ticketId = await _ticketService.CreateTicketAsync(
                    ticket,
                    SessionManager.CurrentUser.UserID,
                    SessionManager.CurrentUser.DepartmentID);

                // Save attachment if provided
                if (!string.IsNullOrWhiteSpace(AttachmentPath) && File.Exists(AttachmentPath))
                {
                    try
                    {
                        await _fileService.SaveAttachmentAsync(AttachmentPath, ticketId);
                    }
                    catch (Exception ex)
                    {
                        // Log attachment error but don't fail the ticket submission
                        MessageBox.Show(
                            $"Ticket created successfully, but attachment failed to save: {ex.Message}",
                            "Warning",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                }

                // Show confirmation
                MessageBox.Show(
                    $"Ticket #{ticketId} has been submitted successfully!",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Clear form
                ClearForm();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to submit ticket: {ex.Message}";
            }
            finally
            {
                IsSubmitting = false;
            }
        }

        private void ClearForm()
        {
            Title = string.Empty;
            Description = string.Empty;
            SelectedCategory = null;
            SelectedPriority = null;
            AttachmentPath = string.Empty;
            ErrorMessage = string.Empty;
        }
    }
}
