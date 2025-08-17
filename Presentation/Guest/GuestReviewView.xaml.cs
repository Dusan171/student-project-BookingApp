using BookingApp.Domain;
using BookingApp.Repositories;
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
        private readonly GuestReviewRepositoryD _guestReviewRepositoryD;
        public GuestReviewView(Reservation reservation)
        {
            InitializeComponent();
            _reservation = reservation;
            _guestReviewRepositoryD = new GuestReviewRepositoryD();

           /* if ((DateTime.Now - _reservation.EndDate).TotalDays > 5)
            {
                MessageBox.Show("You can only leave a review within 5 days after your stay.");
                this.Close();
            }*/
        }
        private void Submit_Click(Object sender, RoutedEventArgs e)
        {
            //validacija unosa
            if (!int.TryParse(CleanlinessTextBox.Text, out int cleanliness) || cleanliness < 1 || cleanliness > 5)
            {
                MessageBox.Show("Enter a valid cleanliness rating (1-5).");
                return;
            }
            if (!int.TryParse(OwnerTextBox.Text, out int owner) || owner < 1 || owner > 5)
            {
                MessageBox.Show("Enter a valid owner rating (1-5).");
                return;
            }
            string comment = CommentTextBox.Text.Trim();
            string imagePaths = ImagesTextBox.Text.Trim();

            //var review = new GuestReview
            /*var review = new GuestReview
            {
                Id = _guestReviewRepositoryD.NextId(),
                ReservationId = _reservation.Id,
                CleanlinessRating = cleanliness,
                OwnerRating = owner,
                Comment = comment,
                ImagePaths = imagePaths,
                CreatedAt = DateTime.Now
            };
            _guestReviewRepository.Save(review);*/
            var reviewD = new GuestReviewD
            {
                Id = _guestReviewRepositoryD.NextId(),
                ReservationId = _reservation.Id,
                CleanlinessRating = cleanliness,
                OwnerRating = owner,
                Comment = comment,
                ImagePaths = imagePaths,
                CreatedAt = DateTime.Now
            };
            _guestReviewRepositoryD.Save(reviewD);

            MessageBox.Show("Thank you for your review!");
            this.Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs r)
        {
            this.Close();
        }
    }
}
