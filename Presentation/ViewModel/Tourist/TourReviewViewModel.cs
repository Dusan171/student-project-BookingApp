using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using BookingApp.Domain.Model;
using BookingApp.Repositories;
using BookingApp.Services.DTO;
using BookingApp.Utilities;
using Microsoft.Win32;

namespace BookingApp.Presentation.ViewModel.Tourist
{
    public class TourReviewViewModel : INotifyPropertyChanged
    {
        private readonly TourReservationRepository _tourReservationRepository = new();
        private readonly TourReviewRepository _reviewRepository = new();
        private readonly TourRepository _tourRepository = new();
        private readonly UserRepository _userRepository = new();
        private ObservableCollection<string> _selectedImages = new();
        public ObservableCollection<string> SelectedImages
        {
            get => _selectedImages;
            set { _selectedImages = value; 
            OnPropertyChanged(nameof(SelectedImages)); }
        }

        public string ImageCount => $"({SelectedImages.Count} slika)";

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

        

        public ICommand AddImageCommand => new RelayCommand(AddImage);
        public ICommand RemoveImageCommand => new RelayCommand<string>(RemoveImage);
        public ICommand ReviewCommand => new RelayCommand<TourReservation>(r => SelectedReservation = r);
        public ICommand CancelReviewCommand => new RelayCommand(() =>
        {
            
            foreach (var imagePath in SelectedImages.ToList())
            {
                try
                {
                    string fullPath = Path.GetFullPath(imagePath);
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }
                catch { /* Ignoriši greške brisanja */ }
            }

            SelectedReservation = null;
            ResetRatings();
            ReviewComment = "";
            SelectedImages.Clear();
            OnPropertyChanged(nameof(ImageCount));
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
                if (r.Status == TourReservationStatus.COMPLETED && !_reviewRepository.HasReviewForReservation(r.Id))
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
                TourId = SelectedReservation.TourId,
                GuideKnowledge = GuideKnowledgeRating,
                GuideLanguage = GuideLanguageRating,
                TourInterest = TourInterestRating,
                Comment = ReviewComment,
                TouristId = Session.CurrentUser?.Id ?? 0,
                
                ReviewDate = DateTime.Now,
                ImagePaths = SelectedImages.ToList()
            };

            _reviewRepository.AddReview(review);
            CompletedReservations.Remove(SelectedReservation);

            MessageBox.Show("Vaša ocena je sačuvana!", "Ocena", MessageBoxButton.OK, MessageBoxImage.Information);

            ReviewSubmitted?.Invoke(this, SelectedReservation.Id);

            SelectedReservation = null;
            ResetRatings();
            ReviewComment = "";
            SelectedImages.Clear(); 
            OnPropertyChanged(nameof(ImageCount)); 
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

        private void AddImage()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Odaberite sliku",
                Filter = "Slike|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Sve datoteke|*.*",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
               
                string imagesFolder = "../../../Resources/Images";
                string fullImagesPath = Path.GetFullPath(imagesFolder);

                
                if (!Directory.Exists(fullImagesPath))
                {
                    Directory.CreateDirectory(fullImagesPath);
                }

                foreach (var originalPath in openFileDialog.FileNames)
                {
                    try
                    {
                        
                        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalPath)}";
                        string destinationPath = Path.Combine(fullImagesPath, fileName);

                        // Kopiraj sliku u Resources/Images folder
                        File.Copy(originalPath, destinationPath, true);

                        // Relativna putanja za čuvanje u CSV
                        string relativePath = $"../../../Resources/Images/{fileName}";

                        if (!SelectedImages.Contains(relativePath))
                        {
                            SelectedImages.Add(relativePath);
                            OnPropertyChanged(nameof(ImageCount));
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Greška pri dodavanju slike: {ex.Message}",
                                      "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void RemoveImage(string? imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath) && SelectedImages.Contains(imagePath))
            {
                try
                {
                    // Ukloni iz liste
                    SelectedImages.Remove(imagePath);
                    OnPropertyChanged(nameof(ImageCount));

                    
                    string fullPath = Path.GetFullPath(imagePath);
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Greška pri brisanju slike: {ex.Message}",
                                  "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        protected void OnPropertyChanged(string? name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}