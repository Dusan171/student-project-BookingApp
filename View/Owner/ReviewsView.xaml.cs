using BookingApp.Model;
using BookingApp.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookingApp.View.Owner
{
    /// <summary>
    /// Interaction logic for ReviewsView.xaml
    /// </summary>
    public partial class ReviewsView : UserControl
    {
        public ObservableCollection<GuestReview> HostToGuestReviews { get; set; }
        public ObservableCollection<GuestReviewD> GuestToHostReviews {  get; set; }
        private readonly GuestReviewRepository _guestReviewRepository;
        private readonly GuestReviewRepositoryD _hostReviewRepository;

        public ReviewsView()
        {
            InitializeComponent();
            _guestReviewRepository = new GuestReviewRepository();
            _hostReviewRepository = new GuestReviewRepositoryD();
            var reviews = _guestReviewRepository.GetAll();
            var hostReviews = _hostReviewRepository.GetAll();

            HostToGuestReviews = new ObservableCollection<GuestReview>(reviews);
            GuestToHostReviews = new ObservableCollection<GuestReviewD>(hostReviews);


            HostToGuestGrid.ItemsSource = HostToGuestReviews; 

           
            var filteredGuestToHost = hostReviews
                                    .Where(gth => reviews.Any(htg => htg.ReservationId == gth.ReservationId))
                                    .ToList();

            if (filteredGuestToHost.Count > 0)
            {
                GuestToHostReviews = new ObservableCollection<GuestReviewD>(filteredGuestToHost);
                GuestToHostGrid.ItemsSource = GuestToHostReviews;
               
            }
            else
            {
                GuestToHostGrid.ItemsSource = null;
           
            }



        }
        private void ImagesButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is GuestReviewD review)
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
