using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Utilities;
using System;
using BookingApp.Services.DTO;
using BookingApp.Presentation.View.Owner;
using BookingApp.Services;

namespace BookingApp.Presentation.ViewModel.Owner
{
    public class UnratedGuestsViewModel : INotifyPropertyChanged
    {
        private readonly IReservationService _reservationService;
        private readonly IGuestReviewService _guestReviewService;
        private ObservableCollection<ReservationDTO> _unratedReservations;
        public ObservableCollection<ReservationDTO> UnratedReservations
        {
            get => _unratedReservations;
            set
            {
                _unratedReservations = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<GuestRatingDetailsDTO> _displayItems;
        public ObservableCollection<GuestRatingDetailsDTO> DisplayItems
        {
            get => _displayItems;
            set
            {
                _displayItems = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasNoUnratedGuests));
            }
        }
        public bool HasNoUnratedGuests => DisplayItems?.Count == 0;

        public ICommand RateGuestCommand { get; }
        public ICommand BackToHomeCommand { get; set; }

        public UnratedGuestsViewModel(IReservationService reservationService, IGuestReviewService guestReviewService, Action goBackAction)
        {
            _reservationService = reservationService;
            _guestReviewService = guestReviewService;

            LoadUnratedReservations();

            RateGuestCommand = new RelayCommand(param => RateGuest(param));
            BackToHomeCommand = new RelayCommand(param => goBackAction?.Invoke());
        }

        private void LoadUnratedReservations()
        {
            var unrated = _reservationService.GetUnratedReservations();
            UnratedReservations = new ObservableCollection<ReservationDTO>(unrated);
            LoadDisplayData();
        }

        private void LoadDisplayData()
        {
            if (UnratedReservations == null)
            {
                DisplayItems = new ObservableCollection<GuestRatingDetailsDTO>();
                return;
            }

            var displayItems = UnratedReservations
                .Select(CreateDisplayItem)
                .Where(item => item != null)
                .ToList();

            DisplayItems = new ObservableCollection<GuestRatingDetailsDTO>(displayItems);
        }

        private GuestRatingDetailsDTO CreateDisplayItem(ReservationDTO reservation)
        {
            try
            {
                return _guestReviewService.GetRatingDetailsForReservation(reservation.Id);
            }
            catch
            {
                return new GuestRatingDetailsDTO
                {
                    ReservationId = reservation.Id,
                    StartDate = reservation.StartDate,
                    EndDate = reservation.EndDate,
                    GuestName = "Guest",
                    AccommodationName = "Property",
                    Review = new GuestReviewDTO { ReservationId = reservation.Id }
                };
            }
        }

        private void RateGuest(object parameter)
        {
            if (parameter is int reservationId)
            {
                var rateGuestView = Injector.CreateRateGuestView(reservationId);
                rateGuestView.ShowDialog();
                LoadUnratedReservations();
            }
        }

      
        private void BackToHome()
        {
            BackToHomeCommand?.Execute("Home");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}