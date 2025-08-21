using BookingApp.Domain;
using BookingApp.Domain.Interfaces;
using BookingApp.Services;
using BookingApp.Utilities;
using System;
using System.Windows;
using System.Windows.Input;

namespace BookingApp.Presentation.ViewModel
{
    public class GuestReviewViewModel : ViewModelBase
    {
        private readonly Reservation _reservation;
        private readonly IReviewService _reviewService;

        // Akcija za zatvaranje prozora
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

        public GuestReviewViewModel(Reservation reservation)
        {
            _reservation = reservation ?? throw new ArgumentNullException(nameof(reservation));

            // Dobijanje zavisnosti
            _reviewService = Injector.CreateInstance<IReviewService>();

            // Inicijalizacija komandi
            SubmitCommand = new RelayCommand(Submit);

            CheckIfReviewIsAllowed();
        }

        #region Logika

        private void CheckIfReviewIsAllowed()
        {
            if (_reviewService.IsReviewPeriodExpired(_reservation))
            {
                MessageBox.Show("The period for leaving a review has expired (5 days). The window will now close.", "Review Period Expired", MessageBoxButton.OK, MessageBoxImage.Warning);
                CloseAction?.Invoke();
            }
        }

        private void Submit(object obj)
        {
            // --- Validacija unosa ---
            if (!int.TryParse(CleanlinessRating, out int cleanliness) || cleanliness < 1 || cleanliness > 5)
            {
                MessageBox.Show("Enter a valid cleanliness rating (1-5).");
                return;
            }
            if (!int.TryParse(OwnerRating, out int ownerRating) || ownerRating < 1 || ownerRating > 5)
            {
                MessageBox.Show("Enter a valid owner rating (1-5).");
                return;
            }

            // --- Poziv servisa ---
            try
            {
                _reviewService.CreateOwnerReview(_reservation, cleanliness, ownerRating, Comment, ImagePaths);
                MessageBox.Show("Thank you for your review!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Signaliziramo View-u da je operacija uspela (ako je otvoren kao Dialog)
                // Ovu logiku je teže prebaciti, pa za sada ostavljamo samo zatvaranje
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}