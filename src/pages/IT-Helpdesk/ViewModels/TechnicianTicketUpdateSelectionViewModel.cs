using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using IT_Helpdesk.Models;

namespace IT_Helpdesk.ViewModels
{
    public class TechnicianTicketUpdateSelectionViewModel : ViewModelBase
    {
        private readonly TechnicianDashboardViewModel _parentViewModel;
        private Ticket? _selectedTicket;

        public event EventHandler<Ticket>? TicketSelected;

        public TechnicianTicketUpdateSelectionViewModel(TechnicianDashboardViewModel parentViewModel)
        {
            _parentViewModel = parentViewModel ?? throw new ArgumentNullException(nameof(parentViewModel));
            
            // Initialize command
            UpdateTicketCommand = new RelayCommand(OnUpdateTicket, CanUpdateTicket);
        }

        #region Properties

        public ObservableCollection<Ticket> AssignedTickets => _parentViewModel.AssignedTickets;

        public Ticket? SelectedTicket
        {
            get => _selectedTicket;
            set
            {
                if (SetProperty(ref _selectedTicket, value))
                {
                    ((RelayCommand)UpdateTicketCommand).RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region Commands

        public ICommand UpdateTicketCommand { get; }

        #endregion

        #region Methods

        private bool CanUpdateTicket()
        {
            return SelectedTicket != null;
        }

        private void OnUpdateTicket()
        {
            if (SelectedTicket != null)
            {
                TicketSelected?.Invoke(this, SelectedTicket);
            }
        }

        #endregion
    }
}
