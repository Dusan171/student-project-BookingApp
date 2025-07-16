using BookingApp.Model;
using BookingApp.Repository;
using System;
using System.Windows;

namespace BookingApp.View.Owner
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
                ReservationId= reservationId
            };
            GuestReviewRepository = new GuestReviewRepository();

            DataContext = this;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(CleanlinessBox.Text, out int cleanliness) || cleanliness < 1 || cleanliness > 5 ||
                !int.TryParse(RuleRespectBox.Text, out int ruleRespect) || ruleRespect < 1 || ruleRespect > 5)
            {
                MessageBox.Show("Choose number between 1 and 5", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            GuestReview.CleanlinessRating = cleanliness;
            GuestReview.RuleRespectingRating = ruleRespect;
            GuestReview.Comment = CommentBox.Text;

            try
            {
                GuestReviewRepository.Save(GuestReview);
                RatingChanged?.Invoke(this, EventArgs.Empty);
                MessageBox.Show("Successfully saved guest rating.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (System.Exception ex)
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
