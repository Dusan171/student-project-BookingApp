using BookingApp.Domain;
using BookingApp.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BookingApp.Presentation.Owner
{
    /// <summary>
    /// Interaction logic for ReviewsView.xaml
    /// </summary>
    public partial class ReviewsView : UserControl
    {
        public ObservableCollection<GuestReview> HostToGuestReviews { get; set; }
        public ObservableCollection<OwnerReview> GuestToHostReviews {  get; set; }
        private readonly GuestReviewRepository _guestReviewRepository;
        private readonly OwnerReviewRepository _hostReviewRepository;

        public ReviewsView()
        {
            InitializeComponent();
            _guestReviewRepository = new GuestReviewRepository();
            _hostReviewRepository = new OwnerReviewRepository();
            var reviews = _guestReviewRepository.GetAll();
            var hostReviews = _hostReviewRepository.GetAll();

            HostToGuestReviews = new ObservableCollection<GuestReview>(reviews);
            GuestToHostReviews = new ObservableCollection<OwnerReview>(hostReviews);


            HostToGuestGrid.ItemsSource = HostToGuestReviews; 

           
            var filteredGuestToHost = hostReviews
                                    .Where(gth => reviews.Any(htg => htg.ReservationId == gth.ReservationId))
                                    .ToList();

            if (filteredGuestToHost.Count > 0)
            {
                GuestToHostReviews = new ObservableCollection<OwnerReview>(filteredGuestToHost);
                GuestToHostGrid.ItemsSource = GuestToHostReviews;
               
            }
            else
            {
                GuestToHostGrid.ItemsSource = null;
           
            }



        }
        private void ImagesButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is OwnerReview review)
            {
                var images = review.ImagePaths?
    .Split(';', StringSplitOptions.RemoveEmptyEntries)
    .Select(path => path.Trim()) 
    .ToList();
                if (images != null && images.Count > 0)
                {
                    ImageGallery.SetImages(images);
                    ImageGallery.Visibility = Visibility.Visible;
                    ImageGallery.Focus(); // da hvata tastaturu
                }
            }
        }
    }
}
