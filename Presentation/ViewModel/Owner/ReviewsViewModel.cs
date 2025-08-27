using BookingApp.Domain;
using BookingApp.Domain.Interfaces;
using BookingApp.DTO;
using BookingApp.Services.DTO;
using BookingApp.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookingApp.Presentation.ViewModel.Owner
{
    public class ReviewsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly IGuestReviewService _guestReviewService;
        private readonly IAccommodationReviewService _accommodationReviewService;
        private ObservableCollection<GuestReviewDTO> _hostReviews;
        private ObservableCollection<AccommodationReviewDTO> _guestReviews;

        public ICommand OpenImageGalleryCommand { get; }
        public event Action<List<string>> OpenImageGalleryRequested;
        public ObservableCollection<GuestReviewDTO> HostReviews
        {
            get => _hostReviews;
            set
            {
                _hostReviews = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<AccommodationReviewDTO> GuestReviews
        {
            get => _guestReviews;
            set
            {
                _guestReviews = value;
                OnPropertyChanged();
            }
        }
        public ReviewsViewModel(IGuestReviewService guestReviewService, IAccommodationReviewService accommodationReviewService)
        {
            _guestReviewService = guestReviewService;
            _accommodationReviewService = accommodationReviewService;

            OpenImageGalleryCommand = new RelayCommand(ExecuteOpenImageGallery, CanExecuteOpenImageGallery);

            LoadReviews();
        }

        private void LoadReviews()
        {
            var allHostToGuestReviews = _guestReviewService.GetAllReviews();
            var allGuestToHostReviews = _accommodationReviewService.GetAll();
            var filteredGuestToHost = allGuestToHostReviews
                .Where(gth => allHostToGuestReviews.Any(htg => htg.ReservationId == gth.ReservationId))
                .ToList();
            HostReviews = new ObservableCollection<GuestReviewDTO>(allHostToGuestReviews);
            GuestReviews = new ObservableCollection<AccommodationReviewDTO>(filteredGuestToHost);
        }

        private void ExecuteOpenImageGallery(object parameter)
        {
            if (parameter is AccommodationReviewDTO selectedReview)
            {
                var images = selectedReview.ImagePaths?
                    .Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(path => path.Trim())
                    .ToList();

                if (images != null && images.Count > 0)
                {
                    OpenImageGalleryRequested?.Invoke(images);
                }
            }
        }

        private bool CanExecuteOpenImageGallery(object parameter)
        {
            if (parameter is AccommodationReviewDTO review)
            {
                return !string.IsNullOrEmpty(review.ImagePaths);
            }
            return false;
        }
    }
}
