using BookingApp.Domain;
using BookingApp.Domain.Interfaces;
using BookingApp.Services;
using BookingApp.Utilities;
using System;
using System.Windows;
using System.Windows.Input;
using BookingApp.Services.DTO;
using BookingApp.Domain.Model;

namespace BookingApp.Presentation.ViewModel
{
    public class AccommodationReviewViewModel : ViewModelBase
    {
        private readonly Reservation _reservation;
        private readonly IAccommodationReviewService _accommodationReviewService;

        public Action CloseAction { get; set; }

        #region Svojstva za povezivanje (Binding)

        private string _cleanlinessRating;
        public string CleanlinessRating
        {
            get => _cleanlinessRating;
            set { _cleanlinessRating = value; OnPropertyChanged(); }
        }

        private string _ownerRating;
        public string OwnerRating
        {
            get => _ownerRating;
            set { _ownerRating = value; OnPropertyChanged(); }
        }

        private string _comment;
        public string Comment
        {
            get => _comment;
            set { _comment = value; OnPropertyChanged(); }
        }

        private string _imagePaths;
        public string ImagePaths
        {
            get => _imagePaths;
            set { _imagePaths = value; OnPropertyChanged(); }
        }

        #endregion

        #region Komande
        public ICommand SubmitCommand { get; }
        #endregion

        public AccommodationReviewViewModel(Reservation reservation)
        {
            _reservation = reservation ?? throw new ArgumentNullException(nameof(reservation));

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
                _accommodationReviewService.Create(new ReservationDTO(_reservation), cleanliness, ownerRating, Comment, ImagePaths);
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