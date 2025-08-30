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
    
        public ICommand RateGuestCommand { get; }

        public UnratedGuestsViewModel(IReservationService reservationService, IGuestReviewService guestReviewService)
        {
            _reservationService = reservationService;
            _guestReviewService = guestReviewService;

            LoadUnratedReservations();

            RateGuestCommand = new RelayCommand(param => RateGuest(param));
        }

        private void LoadUnratedReservations()
        {
            var unrated = _reservationService.GetUnratedReservations();
            UnratedReservations = new ObservableCollection<ReservationDTO>(unrated);
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}