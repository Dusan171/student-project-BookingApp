using BookingApp.Domain;
using BookingApp.Domain.Model;
using BookingApp.Repositories;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BookingApp.Utilities;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using System;

namespace BookingApp.Presentation.ViewModel.Guide
{
    public class ReviewsControlViewModel : INotifyPropertyChanged
    {
        private readonly TourReviewRepository reviewRepository;
        private readonly TourRepository toursRepository;
        private readonly TouristAttendanceRepository attendancesRepository;
        private readonly UserRepository userRepository;
        private readonly ReservationGuestRepository guestsRepository;
        private readonly KeyPointRepository keypointRepository;

        public ObservableCollection<TourReviewDisplayDTO> ReviewDisplays { get; set; }

        private List<TourReview> reviews;
        private List<TouristAttendance> attendances;
        private List<Tour> tours;
        private List<ReservationGuest> guests;
        private List<User> tourists;

        public ICommand ReportCommand { get; set; }

        // overlay state
        private int _currentImageIndex;
        private bool _isOverlayVisible;
        private BitmapImage _largeImage;
        private List<string> _allImagesFlat;

        public bool IsOverlayVisible
        {
            get => _isOverlayVisible;
            set { _isOverlayVisible = value; OnPropertyChanged(); }
        }

        public BitmapImage LargeImage
        {
            get => _largeImage;
            set { _largeImage = value; OnPropertyChanged(); }
        }

        public ICommand OpenImageCommand { get; }
        public ICommand CloseOverlayCommand { get; }
        public ICommand NextImageCommand { get; }
        public ICommand PrevImageCommand { get; }

        public ReviewsControlViewModel()
        {
            reviewRepository = new TourReviewRepository();
            toursRepository = new TourRepository();
            attendancesRepository = new TouristAttendanceRepository();
            userRepository = new UserRepository();
            guestsRepository = new ReservationGuestRepository();
            keypointRepository = new KeyPointRepository();

            ReviewDisplays = new ObservableCollection<TourReviewDisplayDTO>();
            ReportCommand = new RelayCommand<TourReviewDisplayDTO>(ReportReview);

            OpenImageCommand = new RelayCommand(OpenImage);
            CloseOverlayCommand = new RelayCommand(_ => CloseOverlay());
            NextImageCommand = new RelayCommand(_ => NextImage());
            PrevImageCommand = new RelayCommand(_ => PrevImage());

            LoadData();
            CreateReviewForShow();
        }

        private void LoadData()
        {
            reviews = reviewRepository.GetAll();
            attendances = attendancesRepository.GetAll();
            tourists = userRepository.GetAll().Where(t => t.Role == UserRole.TOURIST).ToList();
            guests = guestsRepository.GetAll();
            tours = toursRepository.GetAll();
        }

        private void CreateReviewForShow()
        {
            ReviewDisplays.Clear();
            foreach (var review in reviews)
            {
                var tour = tours.FirstOrDefault(t => t.Id == review.TourId);
                var tourist = tourists.FirstOrDefault(t => t.Id == review.TouristId);

                if (tour == null || tourist == null) continue;

                string joinedAt = GetKeyPointJoinedAt(tourist.Id);
                TourReviewDisplayDTO display = new TourReviewDisplayDTO(
                    1,
                    tour.Name,
                    review.IsValid,
                    tourist.FirstName + " " + tourist.LastName,
                    joinedAt,
                    review.GuideKnowledge,
                    review.GuideLanguage,
                    review.TourInterest,
                    review.Comment,
                    review,
                    review.ImagePaths
                );
                ReviewDisplays.Add(display);
            }
        }

        private string GetKeyPointJoinedAt(int touristId)
        {
            var guest = guests.FirstOrDefault(g => g.TouristId == touristId);
            if (guest == null) return "";
            var kp = keypointRepository.GetById(guest.KeyPointJoinedAt);
            return kp?.Name ?? "";
        }

        private void ReportReview(TourReviewDisplayDTO review)
        {
            if (review == null) return;

            var result = System.Windows.MessageBox.Show(
                "Are you sure you want to report this review?",
                "Confirm Report",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (result != System.Windows.MessageBoxResult.Yes)
                return;

            review.OriginalReview.IsValid = false;
            review.IsValid = false;
            reviewRepository.UpdateReview(review.OriginalReview);
            reviewRepository.SaveToFile();
        }

        // -------- overlay logic --------
        private void OpenImage(object parameter)
        {
            if (parameter is string imgPath)
            {
                _allImagesFlat = ReviewDisplays.SelectMany(r => r.ImagePaths).ToList();
                _currentImageIndex = _allImagesFlat.IndexOf(imgPath);
                ShowImage();
                IsOverlayVisible = true;
            }
        }

        private void ShowImage()
        {
            if (_currentImageIndex >= 0 && _currentImageIndex < _allImagesFlat.Count)
            {
                var path = _allImagesFlat[_currentImageIndex];
                LargeImage = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
            }
        }

        private void NextImage()
        {
            if (_allImagesFlat == null || !_allImagesFlat.Any()) return;
            _currentImageIndex = (_currentImageIndex + 1) % _allImagesFlat.Count;
            ShowImage();
        }

        private void PrevImage()
        {
            if (_allImagesFlat == null || !_allImagesFlat.Any()) return;
            _currentImageIndex = (_currentImageIndex - 1 + _allImagesFlat.Count) % _allImagesFlat.Count;
            ShowImage();
        }

        private void CloseOverlay()
        {
            IsOverlayVisible = false;
            LargeImage = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
