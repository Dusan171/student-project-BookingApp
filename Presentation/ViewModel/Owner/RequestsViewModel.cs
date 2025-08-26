using BookingApp.Domain;
using BookingApp.Utilities; 
using BookingApp.Services.DTO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using System.Linq;
using System;

namespace BookingApp.Presentation.ViewModel.Owner
{
    public class RequestsViewModel : INotifyPropertyChanged
    {
        private readonly IRescheduleRequestService _rescheduleRequestService;
        private readonly IAccommodationService _accommodationService;
        private readonly IReservationService _reservationService;

        private ObservableCollection<RescheduleRequestDTO> _requests;
        public ReservationDTO Reservation{ get; set; }
        public AccommodationDTO Accommodation { get; set; }
        public RescheduleRequestDTO Request { get; set; }

        public ObservableCollection<RescheduleRequestDTO> Requests
        {
            get => _requests;
            set
            {
                _requests = value;
                OnPropertyChanged();
            }
        }

        private RescheduleRequestDTO _selectedRequest;
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

        private bool _isRejecting;
        public bool IsRejecting
        {
            get => _isRejecting;
            set
            {
                _isRejecting = value;
                OnPropertyChanged();
            }
        }

        private string _rejectReason;
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

        public RequestsViewModel(IRescheduleRequestService rescheduleRequestService, IAccommodationService accommodationService, IReservationService reservationService)
        {
            _rescheduleRequestService = rescheduleRequestService;
            _accommodationService = accommodationService;
            _reservationService = reservationService;

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
                var reservation = _reservationService.GetById(request.ReservationId);

                if (reservation != null)
                {
                  
                    request.OriginalStartDate = reservation.StartDate;
                    request.OriginalEndDate = reservation.EndDate;

                    var accommodation = _accommodationService.GetAccommodationById(reservation.AccommodationId);

                    if (accommodation != null)
                    {
                      
                        request.AccommodationName = accommodation.Name;
                        bool isAvailable = _reservationService.IsAccommodationAvailable(accommodation.Id, request.NewStartDate, request.NewEndDate);
                        request.AvailabilityStatus = isAvailable ? "Available" : "Not available";
                    }
                }
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

            var reservationToUpdate = _reservationService.GetById(SelectedRequest.ReservationId);


            if (reservationToUpdate != null)
            {
                reservationToUpdate.StartDate = SelectedRequest.NewStartDate;
                reservationToUpdate.EndDate = SelectedRequest.NewEndDate;
                _reservationService.Update(reservationToUpdate); 
            }

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