using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain.Model;
using BookingApp.Repositories;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Tourist
{
    public class TourReviewViewModel : INotifyPropertyChanged
    {
        private readonly TourReservationRepository _tourReservationRepository = new();
        private readonly TourReviewRepository _reviewRepository = new();
        private readonly TourRepository _tourRepository = new();
        private readonly UserRepository _userRepository = new();

        public ObservableCollection<TourReservation> CompletedReservations { get; set; } = new();

        private TourReservation? _selectedReservation;
        public TourReservation? SelectedReservation
        {
            get => _selectedReservation;
            set
            {
                _selectedReservation = value;
                OnPropertyChanged(nameof(SelectedReservation));
                OnPropertyChanged(nameof(SelectedTourName));
                IsReviewFormVisible = value != null;
            }
        }

        public string SelectedTourName => SelectedReservation?.TourName ?? "";

        private bool _isReviewFormVisible;
        public bool IsReviewFormVisible
        {
            get => _isReviewFormVisible;
            set { _isReviewFormVisible = value; OnPropertyChanged(nameof(IsReviewFormVisible)); }
        }

        private int _guideKnowledgeRating;
        public int GuideKnowledgeRating
        {
            get => _guideKnowledgeRating;
            set
            {
                _guideKnowledgeRating = value;
                OnPropertyChanged(nameof(GuideKnowledgeRating));
                OnPropertyChanged(nameof(GuideKnowledgeRatingText));
            }
        }
        public string GuideKnowledgeRatingText => $"({GuideKnowledgeRating}/5)";

        private int _guideLanguageRating;
        public int GuideLanguageRating
        {
            get => _guideLanguageRating;
            set
            {
                _guideLanguageRating = value;
                OnPropertyChanged(nameof(GuideLanguageRating));
                OnPropertyChanged(nameof(GuideLanguageRatingText));
            }
        }
        public string GuideLanguageRatingText => $"({GuideLanguageRating}/5)";

        private int _tourInterestRating;
        public int TourInterestRating
        {
            get => _tourInterestRating;
            set
            {
                _tourInterestRating = value;
                OnPropertyChanged(nameof(TourInterestRating));
                OnPropertyChanged(nameof(TourInterestRatingText));
            }
        }
        public string TourInterestRatingText => $"({TourInterestRating}/5)";

        private string _reviewComment = "";
        public string ReviewComment
        {
            get => _reviewComment;
            set
            {
                _reviewComment = value;
                OnPropertyChanged(nameof(ReviewComment));
            }
        }

        // Commands
        public ICommand ReviewCommand => new RelayCommand<TourReservation>(r => SelectedReservation = r);
        public ICommand CancelReviewCommand => new RelayCommand(() =>
        {
            SelectedReservation = null;
            ResetRatings();
            ReviewComment = "";
        });
        public ICommand SubmitReviewCommand => new RelayCommand(SubmitReview);

        public ICommand SetGuideKnowledgeRatingCommand => new RelayCommand<object>(param =>
        {
            if (param != null && int.TryParse(param.ToString(), out int r))
                GuideKnowledgeRating = r;
        });

        public ICommand SetGuideLanguageRatingCommand => new RelayCommand<object>(param =>
        {
            if (param != null && int.TryParse(param.ToString(), out int r))
                GuideLanguageRating = r;
        });

        public ICommand SetTourInterestRatingCommand => new RelayCommand<object>(param =>
        {
            if (param != null && int.TryParse(param.ToString(), out int r))
                TourInterestRating = r;
        });

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<int>? ReviewSubmitted;

        public TourReviewViewModel()
        {
            LoadCompletedTours();
        }

        public void LoadCompletedTours()
        {
            if (Session.CurrentUser == null) return;

            CompletedReservations.Clear();
            var all = _tourReservationRepository.GetReservationsForTourist(Session.CurrentUser.Id);
            foreach (var r in all)
            {
                if (r.Status == TourReservationStatus.COMPLETED && !_reviewRepository.HasReview(r.TouristId, r.TourId))
                {
                    r.Tour = _tourRepository.GetById(r.TourId);

                    if (r.Tour?.Guide?.Id > 0)
                    {
                        r.Tour.Guide = _userRepository.GetById(r.Tour.Guide.Id);
                    }

                    CompletedReservations.Add(r);
                }
            }
        }

        private void SubmitReview()
        {
            if (SelectedReservation == null) return;

            var review = new TourReview
            {
                ReservationId = SelectedReservation.Id,
                GuideKnowledge = GuideKnowledgeRating,
                GuideLanguage = GuideLanguageRating,
                TourInterest = TourInterestRating,
                Comment = ReviewComment,
                TouristId = Session.CurrentUser?.Id ?? 0,
                Date = DateTime.Now
            };

            _reviewRepository.AddReview(review);
            CompletedReservations.Remove(SelectedReservation);

            MessageBox.Show("Vaša ocena je sačuvana!", "Ocena", MessageBoxButton.OK, MessageBoxImage.Information);

            ReviewSubmitted?.Invoke(this, SelectedReservation.Id);

            SelectedReservation = null;
            ResetRatings();
            ReviewComment = "";
            LoadCompletedTours();
        }

        private void ResetRatings()
        {
            GuideKnowledgeRating = 0;
            GuideLanguageRating = 0;
            TourInterestRating = 0;
        }

        public void SelectReservationForReview(TourReservationDTO reservationDTO)
        {
            LoadCompletedTours();

            var reservation = CompletedReservations.FirstOrDefault(r => r.Id == reservationDTO.Id);
            if (reservation != null)
            {
                SelectedReservation = reservation;
                return;
            }

            reservation = CompletedReservations.FirstOrDefault(r => r.TourName == reservationDTO.TourName);
            if (reservation != null)
            {
                SelectedReservation = reservation;
            }
            else
            {
                MessageBox.Show("Rezervacija nije pronađena ili je već ocenjena.",
                              "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        protected void OnPropertyChanged(string? name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}