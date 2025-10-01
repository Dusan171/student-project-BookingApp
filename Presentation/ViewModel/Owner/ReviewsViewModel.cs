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
        private readonly IReservationService _reservationService;
        private readonly IAccommodationService _accommodationService;
        private readonly IUserService _userService;

        // Original collections
        private List<GuestReviewDTO> _allHostReviews;
        private List<AccommodationReviewDTO> _allGuestReviews;

        // Display collections
        private ObservableCollection<ReviewDisplayDTO> _hostToGuestDisplay;
        private ObservableCollection<ReviewDisplayDTO> _guestToHostDisplay;

        // Search/Sort properties
        private string _searchText;
        private string _selectedSortOption;

        public ObservableCollection<string> SortOptions { get; set; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                _selectedSortOption = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public ObservableCollection<ReviewDisplayDTO> HostToGuestDisplay
        {
            get => _hostToGuestDisplay;
            set
            {
                _hostToGuestDisplay = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ReviewDisplayDTO> GuestToHostDisplay
        {
            get => _guestToHostDisplay;
            set
            {
                _guestToHostDisplay = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenImageGalleryCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand CloseGalleryCommand { get; }
        public ICommand PreviousImageCommand { get; }
        public ICommand NextImageCommand { get; }
        public event Action<List<string>> OpenImageGalleryRequested;

        public ReviewsViewModel(
            IGuestReviewService guestReviewService,
            IAccommodationReviewService accommodationReviewService,
            IReservationService reservationService,
            IAccommodationService accommodationService,
            IUserService userService)
        {
            _guestReviewService = guestReviewService;
            _accommodationReviewService = accommodationReviewService;
            _reservationService = reservationService;
            _accommodationService = accommodationService;
            _userService = userService;

            // Initialize collections to prevent null reference
            _allHostReviews = new List<GuestReviewDTO>();
            _allGuestReviews = new List<AccommodationReviewDTO>();

            OpenImageGalleryCommand = new RelayCommand(ExecuteOpenImageGallery, CanExecuteOpenImageGallery);
            ClearSearchCommand = new RelayCommand(_ => SearchText = string.Empty);
            CloseGalleryCommand = new RelayCommand(_ => { });
            PreviousImageCommand = new RelayCommand(_ => { });
            NextImageCommand = new RelayCommand(_ => { });

            SortOptions = new ObservableCollection<string>
            {
                "Newest First",
                "Oldest First",
                "Highest Rating",
                "Lowest Rating"
            };

            _selectedSortOption = "Newest First";

            LoadReviews();
        }

        private void LoadReviews()
        {
            LoadHostToGuestReviews();
            LoadGuestToHostReviews();
        }

        private void LoadHostToGuestReviews()
        {
            _allHostReviews = _guestReviewService.GetAllReviews() ?? new List<GuestReviewDTO>();
            ApplyFilters();
        }

        private void LoadGuestToHostReviews()
        {
            var allGuestReviews = _accommodationReviewService.GetAll() ?? new List<AccommodationReviewDTO>();
            var allHostReviews = _guestReviewService.GetAllReviews() ?? new List<GuestReviewDTO>();

            // Filter only reviews where owner also rated the guest
            _allGuestReviews = allGuestReviews
                .Where(gr => allHostReviews.Any(hr => hr.ReservationId == gr.ReservationId))
                .ToList();

            ApplyFilters();
        }

        private void ApplyFilters()
        {
            // Process Host → Guest
            var hostDisplayItems = _allHostReviews
                .Select(CreateHostToGuestDisplayItem)
                .Where(item => item != null)
                .ToList();

            // Apply search
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                hostDisplayItems = hostDisplayItems
                    .Where(item => item.GuestName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                   item.AccommodationName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Apply sort
            hostDisplayItems = ApplySort(hostDisplayItems);
            HostToGuestDisplay = new ObservableCollection<ReviewDisplayDTO>(hostDisplayItems);

            // Process Guest → Host
            var guestDisplayItems = _allGuestReviews
                .Select(CreateGuestToHostDisplayItem)
                .Where(item => item != null)
                .ToList();

            // Apply search
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                guestDisplayItems = guestDisplayItems
                    .Where(item => item.GuestName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                   item.AccommodationName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Apply sort
            guestDisplayItems = ApplySort(guestDisplayItems);
            GuestToHostDisplay = new ObservableCollection<ReviewDisplayDTO>(guestDisplayItems);
        }

        private List<ReviewDisplayDTO> ApplySort(List<ReviewDisplayDTO> items)
        {
            return SelectedSortOption switch
            {
                "Newest First" => items.OrderByDescending(x => x.CheckInDate).ToList(),
                "Oldest First" => items.OrderBy(x => x.CheckInDate).ToList(),
                "Highest Rating" => items.OrderByDescending(x => x.AverageRating).ToList(),
                "Lowest Rating" => items.OrderBy(x => x.AverageRating).ToList(),
                _ => items
            };
        }

        private ReviewDisplayDTO CreateHostToGuestDisplayItem(GuestReviewDTO review)
        {
            try
            {
                var reservation = _reservationService.GetById(review.ReservationId);
                if (reservation == null) return null;

                var guest = _userService.GetUserById(reservation.GuestId);
                var accommodation = _accommodationService.GetAccommodationById(reservation.AccommodationId);

                return new ReviewDisplayDTO
                {
                    ReservationId = review.ReservationId,
                    GuestName = guest != null ? $"{guest.FirstName} {guest.LastName}".Trim() : "Guest",
                    AccommodationName = accommodation?.Name ?? "Property",
                    CheckInDate = reservation.StartDate,
                    CheckOutDate = reservation.EndDate,
                    CleanlinessRating = review.CleanlinessRating,
                    RuleRespectingRating = review.RuleRespectingRating,
                    Comment = review.Comment ?? "",
                    ImagePaths = null,
                    ImageCount = 0,
                    FirstImagePath = null
                };
            }
            catch
            {
                return null;
            }
        }

        private ReviewDisplayDTO CreateGuestToHostDisplayItem(AccommodationReviewDTO review)
        {
            try
            {
                var reservation = _reservationService.GetById(review.ReservationId);
                if (reservation == null) return null;

                var guest = _userService.GetUserById(reservation.GuestId);
                var accommodation = _accommodationService.GetAccommodationById(reservation.AccommodationId);

                var images = review.ImagePaths?
                    .Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .Where(p => !string.IsNullOrEmpty(p))
                    .ToList();

                return new ReviewDisplayDTO
                {
                    ReservationId = review.ReservationId,
                    GuestName = guest != null ? $"{guest.FirstName} {guest.LastName}".Trim() : "Guest",
                    AccommodationName = accommodation?.Name ?? "Property",
                    CheckInDate = reservation.StartDate,
                    CheckOutDate = reservation.EndDate,
                    CleanlinessRating = review.CleanlinessRating,
                    OwnerRating = review.OwnerRating,
                    Comment = review.Comment ?? "",
                    ImagePaths = review.ImagePaths,
                    ImageCount = images?.Count ?? 0,
                    FirstImagePath = images?.FirstOrDefault()
                };
            }
            catch
            {
                return null;
            }
        }

        private void ExecuteOpenImageGallery(object parameter)
        {
            if (parameter is ReviewDisplayDTO review && !string.IsNullOrEmpty(review.ImagePaths))
            {
                var images = review.ImagePaths
                    .Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(path => path.Trim())
                    .Where(p => !string.IsNullOrEmpty(p))
                    .ToList();

                if (images.Count > 0)
                {
                    OpenImageGalleryRequested?.Invoke(images);
                }
            }
        }

        private bool CanExecuteOpenImageGallery(object parameter)
        {
            return parameter is ReviewDisplayDTO review && !string.IsNullOrEmpty(review.ImagePaths);
        }
    }
}