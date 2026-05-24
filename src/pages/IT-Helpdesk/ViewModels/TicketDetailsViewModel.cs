using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;
using IT_Helpdesk.Services;

namespace IT_Helpdesk.ViewModels
{
    public class TicketDetailsUpdateItem
    {
        public string UpdatedByName { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public DateTime UpdateDate { get; set; }
        public string StatusChange { get; set; } = string.Empty;
        public Visibility HasStatusChange => string.IsNullOrEmpty(StatusChange) ? Visibility.Collapsed : Visibility.Visible;
    }

    public class TicketDetailsAttachmentItem
    {
        public string FileName { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }

    public class TicketDetailsViewModel : ViewModelBase
    {
        private readonly ITicketService _ticketService;
        private readonly IUserService _userService;
        private readonly ITicketUpdateRepository _ticketUpdateRepo;
        private readonly IAttachmentRepository _attachmentRepo;
        private readonly IDepartmentRepository _departmentRepo;

        private int _ticketID;
        private string _ticketTitle = string.Empty;
        private string _description = string.Empty;
        private string _category = string.Empty;
        private string _priority = string.Empty;
        private string _status = string.Empty;
        private DateTime _dateSubmitted;
        private DateTime? _dateResolved;
        private string _submittedByName = string.Empty;
        private string _assignedToName = "Unassigned";
        private string _departmentName = string.Empty;
        private ObservableCollection<TicketDetailsUpdateItem> _updates = new();
        private ObservableCollection<TicketDetailsAttachmentItem> _attachments = new();

        public TicketDetailsViewModel(int ticketId)
        {
            var connStr = DatabaseConfig.ConnectionString;
            var ticketRepo = new TicketRepository(connStr);
            var ticketUpdateRepo = new TicketUpdateRepository(connStr);
            var userRepo = new UserRepository(connStr);
            var attachmentRepo = new AttachmentRepository(connStr);
            var departmentRepo = new DepartmentRepository(connStr);

            _ticketService = new TicketService(ticketRepo, ticketUpdateRepo, userRepo);
            _userService = new UserService(userRepo, ticketRepo);
            _ticketUpdateRepo = ticketUpdateRepo;
            _attachmentRepo = attachmentRepo;
            _departmentRepo = departmentRepo;

            _ = LoadTicketDetailsAsync(ticketId);
        }

        #region Properties

        public int TicketID
        {
            get => _ticketID;
            set => SetProperty(ref _ticketID, value);
        }

        public string TicketTitle
        {
            get => _ticketTitle;
            set => SetProperty(ref _ticketTitle, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        public string Priority
        {
            get => _priority;
            set => SetProperty(ref _priority, value);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public DateTime DateSubmitted
        {
            get => _dateSubmitted;
            set => SetProperty(ref _dateSubmitted, value);
        }

        public DateTime? DateResolved
        {
            get => _dateResolved;
            set => SetProperty(ref _dateResolved, value);
        }

        public string DateResolvedDisplay => DateResolved.HasValue 
            ? DateResolved.Value.ToString("MMM dd, yyyy hh:mm tt") 
            : "Not resolved yet";

        public string SubmittedByName
        {
            get => _submittedByName;
            set => SetProperty(ref _submittedByName, value);
        }

        public string AssignedToName
        {
            get => _assignedToName;
            set => SetProperty(ref _assignedToName, value);
        }

        public string DepartmentName
        {
            get => _departmentName;
            set => SetProperty(ref _departmentName, value);
        }

        public ObservableCollection<TicketDetailsUpdateItem> Updates
        {
            get => _updates;
            set => SetProperty(ref _updates, value);
        }

        public ObservableCollection<TicketDetailsAttachmentItem> Attachments
        {
            get => _attachments;
            set => SetProperty(ref _attachments, value);
        }

        #endregion

        private async Task LoadTicketDetailsAsync(int ticketId)
        {
            try
            {
                // Load ticket
                var ticket = await _ticketService.GetTicketByIdAsync(ticketId);
                if (ticket == null)
                {
                    MessageBox.Show("Ticket not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                TicketID = ticket.TicketID;
                TicketTitle = ticket.Title;
                Description = ticket.Description;
                Category = ticket.Category;
                Priority = ticket.Priority;
                Status = ticket.Status;
                DateSubmitted = ticket.DateSubmitted;
                DateResolved = ticket.DateResolved;

                // Load submitted by user
                var submitter = await _userService.GetUserByIdAsync(ticket.SubmittedBy);
                SubmittedByName = submitter?.FullName ?? "Unknown";

                // Load assigned to user
                if (ticket.AssignedTo.HasValue && ticket.AssignedTo.Value > 0)
                {
                    var assignee = await _userService.GetUserByIdAsync(ticket.AssignedTo.Value);
                    AssignedToName = assignee?.FullName ?? "Unassigned";
                }
                else
                {
                    AssignedToName = "Unassigned";
                }

                // Load department
                var department = await _departmentRepo.GetByIdAsync(ticket.DepartmentID);
                DepartmentName = department?.DepartmentName ?? "Unknown";

                // Load updates
                var updates = await _ticketUpdateRepo.GetByTicketIdAsync(ticketId);
                var updateDisplayItems = new ObservableCollection<TicketDetailsUpdateItem>();
                
                foreach (var update in updates.OrderByDescending(u => u.UpdateDate))
                {
                    var updater = await _userService.GetUserByIdAsync(update.UpdatedBy);
                    updateDisplayItems.Add(new TicketDetailsUpdateItem
                    {
                        UpdatedByName = updater?.FullName ?? "Unknown",
                        Remarks = update.Remarks,
                        UpdateDate = update.UpdateDate,
                        StatusChange = update.StatusChange
                    });
                }
                Updates = updateDisplayItems;

                // Load attachments
                var attachments = await _attachmentRepo.GetByTicketIdAsync(ticketId);
                var attachmentDisplayItems = new ObservableCollection<TicketDetailsAttachmentItem>();
                
                foreach (var attachment in attachments.OrderByDescending(a => a.UploadedAt))
                {
                    attachmentDisplayItems.Add(new TicketDetailsAttachmentItem
                    {
                        FileName = attachment.FileName,
                        UploadedAt = attachment.UploadedAt
                    });
                }
                Attachments = attachmentDisplayItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading ticket details: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
