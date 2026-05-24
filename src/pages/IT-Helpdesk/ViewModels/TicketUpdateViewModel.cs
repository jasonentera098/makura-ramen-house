using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;
using IT_Helpdesk.Services;

namespace IT_Helpdesk.ViewModels
{
    /// <summary>
    /// Display-friendly wrapper for a TicketUpdate that includes the technician's FullName.
    /// </summary>
    public class TicketUpdateDisplayItem
    {
        public DateTime UpdateDate { get; set; }
        public string UpdatedByName { get; set; } = string.Empty;
        public string StatusChange { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }

    /// <summary>
    /// Display-friendly wrapper for an Attachment that indicates whether the file is available on disk.
    /// </summary>
    public class AttachmentDisplayItem
    {
        public int AttachmentID { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }

        /// <summary>
        /// True when the file exists on disk; false when it has been moved or deleted.
        /// </summary>
        public bool FileAvailable { get; set; }

        /// <summary>
        /// Human-readable availability label shown in the UI.
        /// </summary>
        public string AvailabilityLabel => FileAvailable ? string.Empty : "(File unavailable)";
    }

    public class TicketUpdateViewModel : ViewModelBase
    {
        private readonly ITicketService _ticketService;
        private readonly IFileService _fileService;
        private readonly ITicketUpdateRepository? _ticketUpdateRepository;
        private readonly IUserRepository? _userRepository;
        private Ticket _currentTicket;

        private string _title = string.Empty;
        private string _description = string.Empty;
        private string _selectedStatus = string.Empty;
        private string _remarks = string.Empty;
        private string _attachmentPath = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isUpdating;

        private ObservableCollection<string> _availableStatuses = new();
        private ObservableCollection<TicketUpdateDisplayItem> _updateHistory = new();
        private ObservableCollection<AttachmentDisplayItem> _attachments = new();

        /// <summary>
        /// Event raised when the update is successful and the dialog should close.
        /// </summary>
        public event EventHandler? CloseRequested;

        /// <summary>
        /// Convenience constructor that takes a ticketId and creates all dependencies internally.
        /// Used by dialogs that need to instantiate the ViewModel with minimal setup.
        /// </summary>
        public TicketUpdateViewModel(int ticketId)
        {
            var connStr = DatabaseConfig.ConnectionString;
            var ticketRepo = new TicketRepository(connStr);
            var ticketUpdateRepo = new TicketUpdateRepository(connStr);
            var userRepo = new UserRepository(connStr);
            var attachmentRepo = new AttachmentRepository(connStr);

            _ticketService = new TicketService(ticketRepo, ticketUpdateRepo, userRepo);
            _fileService = new FileService(attachmentRepo);
            _ticketUpdateRepository = ticketUpdateRepo;
            _userRepository = userRepo;

            // Load the ticket synchronously (required for initialization)
            _currentTicket = _ticketService.GetTicketByIdAsync(ticketId).GetAwaiter().GetResult()
                ?? throw new InvalidOperationException($"Ticket with ID {ticketId} not found.");

            InitializeViewModel();
        }

        public TicketUpdateViewModel(ITicketService ticketService, IFileService fileService, Ticket ticket,
            ITicketUpdateRepository? ticketUpdateRepository = null, IUserRepository? userRepository = null)
        {
            _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _currentTicket = ticket ?? throw new ArgumentNullException(nameof(ticket));
            _ticketUpdateRepository = ticketUpdateRepository;
            _userRepository = userRepository;

            InitializeViewModel();
        }

        /// <summary>
        /// Common initialization logic shared by all constructors.
        /// </summary>
        private void InitializeViewModel()
        {
            // Initialize commands FIRST so property setters that call RaiseCanExecuteChanged don't NullRef
            UpdateCommand = new RelayCommand(async () => await UpdateTicketAsync(), CanUpdate);
            OpenAttachmentCommand = new RelayCommand<AttachmentDisplayItem>(async item => await OpenAttachmentItemAsync(item));

            // Initialize properties from ticket (after commands are ready)
            Title = _currentTicket.Title;
            Description = _currentTicket.Description;
            SelectedStatus = _currentTicket.Status;

            // Populate available statuses based on current status
            PopulateAvailableStatuses(_currentTicket.Status);

            // Load existing update history
            _ = LoadUpdateHistoryAsync();

            // Load existing attachments for this ticket
            _ = LoadAttachmentsAsync();
        }

        #region Properties

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

        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (SetProperty(ref _selectedStatus, value))
                {
                    ((RelayCommand)UpdateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string Remarks
        {
            get => _remarks;
            set
            {
                if (SetProperty(ref _remarks, value))
                {
                    // Clear validation error when user starts typing (Requirement 20.6)
                    if (!string.IsNullOrEmpty(ErrorMessage))
                        ErrorMessage = string.Empty;
                    ((RelayCommand)UpdateCommand).RaiseCanExecuteChanged();
                }
            }
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

        public bool IsUpdating
        {
            get => _isUpdating;
            set => SetProperty(ref _isUpdating, value);
        }

        public ObservableCollection<string> AvailableStatuses
        {
            get => _availableStatuses;
            set => SetProperty(ref _availableStatuses, value);
        }

        public ObservableCollection<TicketUpdateDisplayItem> UpdateHistory
        {
            get => _updateHistory;
            set => SetProperty(ref _updateHistory, value);
        }

        public ObservableCollection<AttachmentDisplayItem> Attachments
        {
            get => _attachments;
            set => SetProperty(ref _attachments, value);
        }

        #endregion

        #region Commands

        public ICommand UpdateCommand { get; private set; }
        public ICommand OpenAttachmentCommand { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the existing attachments for the current ticket and checks whether each file exists on disk.
        /// If a file is missing, the record is still shown but marked as unavailable (Requirement 26.4 / 26.5).
        /// </summary>
        private async Task LoadAttachmentsAsync()
        {
            try
            {
                var attachments = await _fileService.GetAttachmentsByTicketAsync(_currentTicket.TicketID);
                var displayItems = new ObservableCollection<AttachmentDisplayItem>();

                foreach (var attachment in attachments)
                {
                    displayItems.Add(new AttachmentDisplayItem
                    {
                        AttachmentID  = attachment.AttachmentID,
                        FileName      = attachment.FileName,
                        FilePath      = attachment.FilePath,
                        UploadedAt    = attachment.UploadedAt,
                        // Check existence without throwing — missing file is shown but flagged
                        FileAvailable = !string.IsNullOrWhiteSpace(attachment.FilePath)
                                        && File.Exists(attachment.FilePath)
                    });
                }

                Attachments = displayItems;
            }
            catch
            {
                // Attachment load failure is non-critical; leave list empty
            }
        }

        /// <summary>
        /// Opens the selected attachment using the system default application.
        /// Delegates to FileService which handles missing/null paths with a user-friendly MessageBox.
        /// </summary>
        private async Task OpenAttachmentItemAsync(AttachmentDisplayItem? item)
        {
            if (item == null) return;
            await _fileService.OpenAttachmentAsync(item.FilePath);
        }

        /// <summary>
        /// Loads the existing TicketUpdate history for the current ticket in chronological order.
        /// </summary>
        private async Task LoadUpdateHistoryAsync()
        {
            if (_ticketUpdateRepository == null) return;

            try
            {
                var updates = await _ticketUpdateRepository.GetByTicketIdAsync(_currentTicket.TicketID);
                var displayItems = new ObservableCollection<TicketUpdateDisplayItem>();

                foreach (var update in updates)
                {
                    string updatedByName = "Unknown";
                    if (_userRepository != null)
                    {
                        var user = await _userRepository.GetByIdAsync(update.UpdatedBy);
                        if (user != null)
                            updatedByName = user.FullName;
                    }

                    displayItems.Add(new TicketUpdateDisplayItem
                    {
                        UpdateDate    = update.UpdateDate,
                        UpdatedByName = updatedByName,
                        StatusChange  = update.StatusChange,
                        Remarks       = update.Remarks
                    });
                }

                UpdateHistory = displayItems;
            }
            catch
            {
                // History load failure is non-critical; leave list empty
            }
        }

        /// <summary>
        /// Populates the available statuses based on the current ticket status.
        /// Enforces the ticket lifecycle: Pending → In Progress → Resolved → Closed
        /// </summary>
        private void PopulateAvailableStatuses(string currentStatus)
        {
            AvailableStatuses.Clear();

            switch (currentStatus)
            {
                case "Pending":
                    // From Pending, can only move to In Progress
                    AvailableStatuses.Add("Pending"); // Keep current status as option
                    AvailableStatuses.Add("In Progress");
                    break;

                case "In Progress":
                    // From In Progress, can only move to Resolved
                    AvailableStatuses.Add("In Progress"); // Keep current status as option
                    AvailableStatuses.Add("Resolved");
                    break;

                case "Resolved":
                    // From Resolved, technician cannot change (only Admin can close)
                    // Only show current status
                    AvailableStatuses.Add("Resolved");
                    break;

                case "Closed":
                    // Closed tickets cannot be modified
                    AvailableStatuses.Add("Closed");
                    break;

                default:
                    // Fallback: show current status
                    AvailableStatuses.Add(currentStatus);
                    break;
            }
        }

        private bool CanUpdate()
        {
            // Remarks are required
            if (string.IsNullOrWhiteSpace(Remarks))
                return false;

            // Status must be selected
            if (string.IsNullOrWhiteSpace(SelectedStatus))
                return false;

            // Cannot update if already updating
            if (IsUpdating)
                return false;

            return true;
        }

        private async Task UpdateTicketAsync()
        {
            try
            {
                IsUpdating = true;
                ErrorMessage = string.Empty;

                // Validate remarks
                if (string.IsNullOrWhiteSpace(Remarks))
                {
                    ErrorMessage = "Remarks are required. Please describe the work done or progress made.";
                    return;
                }

                bool updateSuccess = false;

                // Check if status is changing
                if (SelectedStatus != _currentTicket.Status)
                {
                    // Use UpdateStatusAsync which handles DateResolved automatically
                    updateSuccess = await _ticketService.UpdateStatusAsync(
                        _currentTicket.TicketID,
                        SelectedStatus,
                        SessionManager.CurrentUser.UserID
                    );

                    if (!updateSuccess)
                    {
                        ErrorMessage = "Failed to update ticket status.";
                        return;
                    }

                    // Update local ticket object to reflect the change
                    _currentTicket.Status = SelectedStatus;
                    if (SelectedStatus == "Resolved")
                    {
                        _currentTicket.DateResolved = DateTime.Now;
                    }
                }

                // Add remark/ticket update (separate from status change)
                var remarkSuccess = await _ticketService.AddRemarkAsync(
                    _currentTicket.TicketID,
                    Remarks,
                    SessionManager.CurrentUser.UserID
                );

                if (!remarkSuccess)
                {
                    ErrorMessage = updateSuccess 
                        ? "Ticket status updated, but failed to add remarks."
                        : "Failed to add remarks.";
                    return;
                }

                // Handle attachment if provided
                if (!string.IsNullOrWhiteSpace(AttachmentPath))
                {
                    try
                    {
                        await _fileService.SaveAttachmentAsync(AttachmentPath, _currentTicket.TicketID);
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = $"Ticket updated, but attachment upload failed: {ex.Message}";
                        return;
                    }
                }

                // Show success message
                MessageBox.Show(
                    $"Ticket #{_currentTicket.TicketID} updated successfully!",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                // Clear form
                Remarks = string.Empty;
                AttachmentPath = string.Empty;

                // Refresh available statuses based on new status
                PopulateAvailableStatuses(SelectedStatus);

                // Reload update history to show the new entry
                await LoadUpdateHistoryAsync();

                // Reload attachments to reflect any newly uploaded file
                await LoadAttachmentsAsync();

                // Raise CloseRequested event to signal the dialog to close
                CloseRequested?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating ticket: {ex.Message}";
            }
            finally
            {
                IsUpdating = false;
            }
        }

        #endregion
    }
}
