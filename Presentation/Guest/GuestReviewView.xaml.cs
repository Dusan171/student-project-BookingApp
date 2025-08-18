using BookingApp.Domain;
using BookingApp.Repositories;
using BookingApp.Services;
using System;
using System.Windows;

namespace BookingApp.Presentation.Guest
{
    /// <summary>
    /// Interaction logic for GuestReviewView.xaml
    /// </summary>
    public partial class GuestReviewView : Window
    {
        private readonly Reservation _reservation;
        // private readonly OwnerReviewRepository _ownerReviewRepository;
        private readonly ReviewService _reviewService;
        public GuestReviewView(Reservation reservation)
        {
            InitializeComponent();
            _reservation = reservation;

            var ownerReviewRepository = new OwnerReviewRepository();
            _reviewService = new ReviewService(ownerReviewRepository);

        }
        private void Submit_Click(Object sender, RoutedEventArgs e)
        {
            // 1. Validacija unosa sa forme
            if (!int.TryParse(CleanlinessTextBox.Text, out int cleanliness) || cleanliness < 1 || cleanliness > 5)
            {
                MessageBox.Show("Enter a valid cleanliness rating (1-5).");
                return;
            }
            if (!int.TryParse(OwnerTextBox.Text, out int ownerRating) || ownerRating < 1 || ownerRating > 5)
            {
                MessageBox.Show("Enter a valid owner rating (1-5).");
                return;
            }

            string comment = CommentTextBox.Text.Trim();
            string imagePaths = ImagesTextBox.Text.Trim();

            try
            {
                // 2. Pozivamo servis da obavi sav posao
                _reviewService.CreateOwnerReview(_reservation, cleanliness, ownerRating, comment, imagePaths);

                MessageBox.Show("Thank you for your review!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                // Prikazujemo greške koje dolaze iz poslovne logike
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }                                                      
        }
        private void Cancel_Click(object sender, RoutedEventArgs r)
        {
            this.Close();
        }
    }
}
