using BookingApp.Utilities;
using BookingApp.Services.DTO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using System.Linq;
using BookingApp.Domain.Model;
using BookingApp.Services;

namespace BookingApp.Presentation.ViewModel.Owner
{
    public class RequestsViewModel : INotifyPropertyChanged
    {
        private readonly RequestsDisplayService _requestsDisplayService;
        private readonly IRescheduleRequestService _rescheduleRequestService;

        private ObservableCollection<RescheduleRequestDTO> _requests;
        private RescheduleRequestDTO _selectedRequest;
        private bool _isRejecting;
        private string _rejectReason;
        public ObservableCollection<RescheduleRequestDTO> Requests
        {
            get => _requests;
            set
            {
                _requests = value;
                OnPropertyChanged();
            }
        }
        public RescheduleRequestDTO SelectedRequest
        {
            get => _selectedRequest;
            set
            {
                _selectedRequest = value;
                OnPropertyChanged();
                (RejectCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (ApproveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
        public bool IsRejecting
        {
            get => _isRejecting;
            set
            {
                _isRejecting = value;
                OnPropertyChanged();
            }
        }
        public string RejectReason
        {
            get => _rejectReason;
            set
            {
                _rejectReason = value;
                OnPropertyChanged();
            }
        }
        public ICommand RejectCommand { get; }
        public ICommand ApproveCommand { get; }
        public ICommand SubmitRejectCommand { get; }
        public RequestsViewModel(IRescheduleRequestService rescheduleRequestService, RequestsDisplayService requestsDisplayService)
        {
            _rescheduleRequestService = rescheduleRequestService;
            _requestsDisplayService = requestsDisplayService;

            LoadRequests();

            RejectCommand = new RelayCommand(ExecuteReject, CanExecuteReject);
            ApproveCommand = new RelayCommand(ExecuteApprove, CanExecuteApprove);
            SubmitRejectCommand = new RelayCommand(ExecuteSubmitReject, CanExecuteSubmitReject);
        }
        private void LoadRequests()
        {
            var allRequests = _rescheduleRequestService.GetAll();
            var pendingRequests = allRequests.Where(r => r.Status == RequestStatus.Pending).ToList();

            foreach (var request in pendingRequests)
            {
                _requestsDisplayService.ProcessAndSetRequestData(request);
            }

            Requests = new ObservableCollection<RescheduleRequestDTO>(pendingRequests);
        }
        private void ExecuteApprove(object parameter)
        {
            if (SelectedRequest == null)
            {
                MessageBox.Show("Select Request!");
                return;
            }

            SelectedRequest.Status = RequestStatus.Approved;
            _rescheduleRequestService.Update(SelectedRequest);

            MessageBox.Show("Request Approved.");
            LoadRequests();
        }
        private bool CanExecuteApprove(object parameter)
        {
            return SelectedRequest != null && SelectedRequest.Status == RequestStatus.Pending;
        }

        private void ExecuteReject(object parameter)
        {
            if (SelectedRequest == null)
            {
                MessageBox.Show("Select Request.");
                return;
            }
            IsRejecting = true;
        }
        private bool CanExecuteReject(object parameter)
        {
            return SelectedRequest != null && SelectedRequest.Status == RequestStatus.Pending;
        }
        private void ExecuteSubmitReject(object parameter)
        {
            if (SelectedRequest == null)
            {
                MessageBox.Show("Select Request.");
                return;
            }
            SelectedRequest.Status = RequestStatus.Rejected;
            SelectedRequest.OwnerComment = RejectReason;
            _rescheduleRequestService.Update(SelectedRequest);

            MessageBox.Show("Request Rejected.");
            IsRejecting = false;
            RejectReason = string.Empty;
            LoadRequests();
        }
        private bool CanExecuteSubmitReject(object parameter)
        {
            return SelectedRequest != null && IsRejecting;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}