using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BookingApp.Services.DTO;
using BookingApp.Utilities;
using BookingApp.Domain.Interfaces;
using System.Windows;
using Microsoft.VisualBasic;

namespace BookingApp.Presentation.ViewModel.Owner
{
    public class RateGuestViewModel : INotifyPropertyChanged
    {
        private readonly IGuestReviewService _guestReviewService;
        public Action CloseAction { get; set; }

        private GuestRatingDetailsDTO _ratingDetails;
        public GuestRatingDetailsDTO RatingDetails
        {
            get => _ratingDetails;
            set
            {
                _ratingDetails = value;
                OnPropertyChanged();
            }
        }

        public ICommand SubmitReviewCommand { get; }
        public ICommand CancelCommand { get; }

        public RateGuestViewModel(IGuestReviewService guestReviewService, int reservationId)
        {
            _guestReviewService = guestReviewService;

            RatingDetails = _guestReviewService.GetRatingDetailsForReservation(reservationId);

            SubmitReviewCommand = new RelayCommand(SubmitReview);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void SubmitReview()
        {
            if (RatingDetails?.Review != null)
            {
                _guestReviewService.AddReview(RatingDetails.Review);
                MessageBox.Show("Guest successfully rated!", "Rating Submitted", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseAction?.Invoke();
            }
        }

        private void Cancel()
        {
            CloseAction?.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}