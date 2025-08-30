using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Tourist
{
    public class MojeRezervacijeViewModel : ViewModelBase
    {
        private readonly ITourReservationService _reservationService;
        private ObservableCollection<TourReservationDTO> _allReservations = new();
        private ObservableCollection<TourReservationDTO> _filteredReservations = new();
        private string _selectedStatusFilter = "Sve";
        private bool _isLoading = false;
        private string _statusMessage = "";
        private TourReservationDTO? _selectedReservation;

        public event EventHandler<TourReservationDTO>? ReviewRequested;

        public ObservableCollection<TourReservationDTO> FilteredReservations
        {
            get => _filteredReservations;
            set => SetProperty(ref _filteredReservations, value);
        }

        public ObservableCollection<string> StatusFilterOptions { get; set; } = new()
        {
            "Sve", "Aktivne", "Završene", "Otkazane"
        };

        public string SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set
            {
                if (SetProperty(ref _selectedStatusFilter, value))
                {
                    ApplyFilter();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public TourReservationDTO? SelectedReservation
        {
            get => _selectedReservation;
            set => SetProperty(ref _selectedReservation, value);
        }

        public bool HasReservations => FilteredReservations.Count > 0;
        public bool HasNoReservations => !HasReservations && !IsLoading;

        public ICommand RefreshCommand { get; private set; } = null!;
        public ICommand CancelReservationCommand { get; private set; } = null!;
        public ICommand ReviewTourCommand { get; private set; } = null!;

        public MojeRezervacijeViewModel() : this(null) { }

        public MojeRezervacijeViewModel(ITourReservationService? reservationService)
        {
            _reservationService = reservationService ?? Services.Injector.CreateInstance<ITourReservationService>();
            InitializeCommands();
            LoadReservations();
        }

        private void InitializeCommands()
        {
            RefreshCommand = new RelayCommand(ExecuteRefresh);
            CancelReservationCommand = new RelayCommand<TourReservationDTO>(ExecuteCancelReservation, CanCancelReservation);
            ReviewTourCommand = new RelayCommand<TourReservationDTO>(ExecuteReviewTour, CanReviewTour);
        }

        private void LoadReservations()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Učitavam rezervacije...";

                var currentUser = Session.CurrentUser;
                if (currentUser == null)
                {
                    StatusMessage = "Morate biti ulogovani da biste videli rezervacije.";
                    return;
                }

                var reservations = _reservationService.GetReservationsByTourist(currentUser.Id);

                _allReservations.Clear();
                foreach (var reservation in reservations.OrderByDescending(r => r.ReservationDate))
                {
                    _allReservations.Add(reservation);
                }

                ApplyFilter();

                if (!HasReservations)
                {
                    StatusMessage = "Nemate nijednu rezervaciju.";
                }
                else
                {
                    StatusMessage = $"Učitano {FilteredReservations.Count} rezervacija";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Greška pri učitavanju rezervacija: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                OnPropertyChanged(nameof(HasReservations));
                OnPropertyChanged(nameof(HasNoReservations));
            }
        }

        private void ApplyFilter()
        {
            FilteredReservations.Clear();

            var filtered = SelectedStatusFilter switch
            {
                "Aktivne" => _allReservations.Where(r => r.Status == TourReservationStatus.ACTIVE),
                "Završene" => _allReservations.Where(r => r.Status == TourReservationStatus.COMPLETED),
                "Otkazane" => _allReservations.Where(r => r.Status == TourReservationStatus.CANCELLED),
                _ => _allReservations
            };

            foreach (var reservation in filtered)
            {
                FilteredReservations.Add(reservation);
            }

            OnPropertyChanged(nameof(HasReservations));
            OnPropertyChanged(nameof(HasNoReservations));
        }

        private void ExecuteRefresh()
        {
            LoadReservations();
        }

        private void ExecuteCancelReservation(TourReservationDTO? reservation)
        {
            if (reservation == null) return;

            try
            {
                if (!_reservationService.CanCancelReservation(reservation.Id))
                {
                    StatusMessage = "Rezervaciju nije moguće otkazati.";
                    return;
                }

                _reservationService.CancelReservation(reservation.Id);
                StatusMessage = "Rezervacija je uspešno otkazana.";
                LoadReservations();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Greška pri otkazivanju rezervacije: {ex.Message}";
            }
        }

        private bool CanCancelReservation(TourReservationDTO? reservation)
        {
            return reservation != null &&
                   reservation.Status == TourReservationStatus.ACTIVE &&
                   _reservationService.CanCancelReservation(reservation.Id);
        }

        private void ExecuteReviewTour(TourReservationDTO? reservation)
        {
            if (reservation != null)
            {
                ReviewRequested?.Invoke(this, reservation);
            }
        }

        private bool CanReviewTour(TourReservationDTO? reservation)
        {
            return reservation != null && reservation.Status == TourReservationStatus.COMPLETED;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _allReservations?.Clear();
                FilteredReservations?.Clear();
                StatusFilterOptions?.Clear();
            }
            base.Dispose(disposing);
        }
    }
}