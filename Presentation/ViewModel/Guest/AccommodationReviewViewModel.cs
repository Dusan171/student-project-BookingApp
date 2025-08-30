using System;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using BookingApp.Services;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Guest
{
    public class AccommodationReviewViewModel : ViewModelBase
    {
        private readonly ReservationDetailsDTO _reservationDetails;
        private readonly IAccommodationReviewService _accommodationReviewService;

        public Action CloseAction { get; set; }

        #region Svojstva za unos
        public string CleanlinessRating { get; set; }
        public string OwnerRating { get; set; }
        public string Comment { get; set; }
        public string ImagePaths { get; set; }
        #endregion

        public ICommand SubmitCommand { get; }

        public AccommodationReviewViewModel(ReservationDetailsDTO reservationDetails)
        {
            _reservationDetails = reservationDetails ?? throw new ArgumentNullException(nameof(reservationDetails));
            _accommodationReviewService = Injector.CreateInstance<IAccommodationReviewService>();
            SubmitCommand = new RelayCommand(Submit);
        }

        #region Logika

        private void Submit(object obj)
        {
            if (!IsInputValid(out int cleanliness, out int ownerRating))
            {
                return;
            }

            try
            {
                var reviewDto = new CreateAccommodationReviewDTO
                {
                    ReservationId = _reservationDetails.ReservationId,
                    CleanlinessRating = cleanliness,
                    OwnerRating = ownerRating,
                    Comment = this.Comment,
                    ImagePaths = this.ImagePaths
                };

                _accommodationReviewService.SubmitReview(reviewDto, _reservationDetails.EndDate);

                MessageBox.Show("Thank you for your review!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsInputValid(out int cleanliness, out int ownerRating)
        {
            cleanliness = 0;
            ownerRating = 0;

            if (!int.TryParse(CleanlinessRating, out cleanliness) || cleanliness < 1 || cleanliness > 5)
            {
                MessageBox.Show("Enter a valid cleanliness rating (1-5).");
                return false;
            }
            if (!int.TryParse(OwnerRating, out ownerRating) || ownerRating < 1 || ownerRating > 5)
            {
                MessageBox.Show("Enter a valid owner rating (1-5).");
                return false;
            }
            return true;
        }

        #endregion
    }
}