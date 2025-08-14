using BookingApp.Domain;
using BookingApp.Repositories;
using System;
using System.Windows;

namespace BookingApp.Presentation.Owner
{
    public partial class GuestRatingForm : Window
    {
        public GuestReview GuestReview { get; set; }
        public GuestReviewRepository GuestReviewRepository { get; set; }

        public GuestRatingForm(int reservationId, int guestId)
        {
            InitializeComponent();

            GuestReview = new GuestReview()
            {
                Id = guestId,
                ReservationId = reservationId
            };
            GuestReviewRepository = new GuestReviewRepository();

            DataContext = this;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (!TryParseRatings(out int cleanliness, out int ruleRespect)) return;

            UpdateReviewData(cleanliness, ruleRespect);

            if (!ValidateReview()) return;

            SaveReview();
        }

        private bool TryParseRatings(out int cleanliness, out int ruleRespect)
        {
            cleanliness = ruleRespect = 0;

            if (!int.TryParse(CleanlinessBox.Text, out cleanliness) ||
                !int.TryParse(RuleRespectBox.Text, out ruleRespect))
            {
                MessageBox.Show("Please enter valid numbers for ratings.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void UpdateReviewData(int cleanliness, int ruleRespect)
        {
            GuestReview.CleanlinessRating = cleanliness;
            GuestReview.RuleRespectingRating = ruleRespect;
            GuestReview.Comment = CommentBox.Text;
        }

        private bool ValidateReview()
        {
            if (!GuestReview.IsValid(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void SaveReview()
        {
            try
            {
                GuestReviewRepository.Save(GuestReview);
                RatingChanged?.Invoke(this, EventArgs.Empty);
                MessageBox.Show("Successfully saved guest rating.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unsuccessfully saved guest rating: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event EventHandler RatingChanged;

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
