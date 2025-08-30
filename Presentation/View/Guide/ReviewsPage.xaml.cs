using BookingApp.Domain;
using BookingApp.Domain.Model;
using BookingApp.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BookingApp.Presentation.View.Guide
{
    public partial class ReviewsPage : UserControl 
    {
        private List<TourReview> reviews;
        private List<TouristAttendance> attendances;
        private List<Tour> tours;
        private List<ReservationGuest> guests;
        private TourReviewRepository reviewRepository;
        private KeyPointRepository keypointRepository;
        private TouristAttendanceRepository attendancesRepository;
        private ReservationGuestRepository guestsRepository;
        private UserRepository userRepository;
        private List<User> tourists;
        private TourRepository toursRepository;
        private ObservableCollection<TourReviewDisplay> reviewDisplays;

        public ReviewsPage()
        {
            InitializeComponent();
            reviewRepository = new TourReviewRepository();
            toursRepository = new TourRepository();
            attendancesRepository = new TouristAttendanceRepository();
            userRepository = new UserRepository();
            guestsRepository = new ReservationGuestRepository();
            keypointRepository = new KeyPointRepository();
            reviewDisplays = new ObservableCollection<TourReviewDisplay>();

            LoadReviews();
            LoadAttendaces();
            LoadTourists();
            LoadGuests();
            LoadTours();
            CreateReviewForShow();
        }

        private void LoadReviews()
        {
            reviews = reviewRepository.GetAll(); 
        }
        private void LoadAttendaces()
        {
            attendances = attendancesRepository.GetAll();
        }
        private void LoadTourists()
        {
            tourists = userRepository.GetAll();
            tourists = tourists.Where(t => t.Role == UserRole.TOURIST).ToList();
        }
        private void LoadGuests()
        {
            guests = guestsRepository.GetAll();
        }
        public void LoadTours()
        {
            tours = toursRepository.GetAll();
        }
        private void CreateReviewForShow()
        {
            //ime turiste, ime ture, keypoint, sve ove ocene podaci
            foreach(var review in reviews)
            {
                var tour = tours.FirstOrDefault(t => t.Id == review.TourId);
                var tourist = tourists.FirstOrDefault(t => t.Id == review.TouristId);
                String joinedAt = GetKeyPointJoinedAt(tourist.Id, guests);
                TourReviewDisplay reviewDisplay = new TourReviewDisplay(1, tour.Name, review.IsValid, tourist.FirstName + " " + tourist.LastName, joinedAt, review.GuideKnowledge, review.GuideLanguage, review.TourInterest, review.Comment, review);
                reviewDisplays.Add(reviewDisplay);
                //dodaj u fajl preko repository, a mozda i ne mora fr
            }
            ReviewsList.ItemsSource = null;
            ReviewsList.ItemsSource = reviewDisplays; 

        }
        private String GetKeyPointJoinedAt(int touristId, List<ReservationGuest> guests)
        {
            var guest = guests.FirstOrDefault(g => g.TouristId == touristId);
            var kp = keypointRepository.GetById(guest.KeyPointJoinedAt);
            return kp.Name;
        }
        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is TourReviewDisplay review)
            {
                var result = MessageBox.Show("Da li ste sigurni da želite da prijavite ovu recenziju?",
                                             "Potvrda prijave",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    
                    review.OriginalReview.IsValid = false;
                    review.IsValid = false;
                    reviewRepository.UpdateReview(review.OriginalReview); 
                    reviewRepository.SaveToFile();
                    //CreateReviewForShow();
                    ReviewsList.Items.Refresh();
                }
            }
        }
    }
}
