using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BookingApp.Services.DTO;
using BookingApp.Utilities;
using BookingApp.Domain.Interfaces;
using System.Windows;

namespace BookingApp.Presentation.ViewModel.Owner
{
    public class RateGuestViewModel : INotifyPropertyChanged
    {
        private readonly IGuestReviewService _guestReviewService;
        public Action CloseAction { get; set; }

        private GuestReviewDTO _guestReview;
        public GuestReviewDTO GuestReview
        {
            get => _guestReview;
            set
            {
                _guestReview = value;
                OnPropertyChanged();
            }
        }
        public ICommand SubmitReviewCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler RequestClose;
        public RateGuestViewModel(IGuestReviewService guestReviewService, int reservationId)
        {
            _guestReviewService = guestReviewService;
            GuestReview = new GuestReviewDTO{ReservationId = reservationId};

            SubmitReviewCommand = new RelayCommand(SubmitReview);
            CancelCommand = new RelayCommand(Cancel);
        }
        private void SubmitReview()
        {
         
            if (!int.TryParse(GuestReview.CleanlinessRating.ToString(), out int cleanlinessRating) ||
                !int.TryParse(GuestReview.RuleRespectingRating.ToString(), out int ruleRespectingRating))
            {
                MessageBox.Show("Enter numbers.");
                return;
            }
                _guestReviewService.AddReview(GuestReview);
                CloseAction?.Invoke();
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