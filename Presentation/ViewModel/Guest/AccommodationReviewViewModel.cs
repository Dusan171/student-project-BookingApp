using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Linq;
using BookingApp.Domain.Interfaces;
using BookingApp.Services;
using BookingApp.Services.DTO;
using BookingApp.Utilities;
using Microsoft.Win32;

namespace BookingApp.Presentation.ViewModel.Guest
{
    public class AccommodationReviewViewModel : ViewModelBase
    {
        private readonly ReservationDetailsDTO _reservationDetails;
        private readonly IAccommodationReviewService _accommodationReviewService;

        public Action CloseAction { get; set; }

        #region Svojstva za unos
        private int _cleanlinessRating = 3;
        public int CleanlinessRating
        {
            get => _cleanlinessRating;
            set 
            {
                _cleanlinessRating = value;
                OnPropertyChanged();
            }
        }
        private int _ownerRating = 3;
        public int OwnerRating
        {
            get => _ownerRating;
            set 
            {
                _ownerRating = value;
                OnPropertyChanged();
            }
        }
        public string Comment { get; set; }
        public ObservableCollection<string> ImagePaths { get; set; }
        #endregion

        public ICommand SubmitCommand { get; }
        public ICommand UploadImageCommand { get; }
        public ICommand RemoveImageCommand { get; }

        public AccommodationReviewViewModel(ReservationDetailsDTO reservationDetails)
        {
            _reservationDetails = reservationDetails ?? throw new ArgumentNullException(nameof(reservationDetails));
            _accommodationReviewService = Injector.CreateInstance<IAccommodationReviewService>();

            ImagePaths = new ObservableCollection<string>();

            SubmitCommand = new RelayCommand(Submit);
            UploadImageCommand = new RelayCommand(UploadImage);
            RemoveImageCommand = new RelayCommand(RemoveImage);
        }

        #region Logika

        private void Submit(object obj)
        {
            try
            {
                var reviewDto = new CreateAccommodationReviewDTO
                {
                    ReservationId = _reservationDetails.ReservationId,
                    CleanlinessRating = this.CleanlinessRating,
                    OwnerRating = this.OwnerRating,
                    Comment = this.Comment,
                    ImagePaths = string.Join(";", ImagePaths)
                };

                _accommodationReviewService.SubmitReview(reviewDto, _reservationDetails.EndDate);

                MessageBox.Show("Thank you for your review!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void UploadImage(object obj)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg",
                Multiselect = true 
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    if (!ImagePaths.Contains(filename))
                    {
                        ImagePaths.Add(filename);
                    }
                }
            }
        }
        private void RemoveImage(object parameter)
        {
            if (parameter is string imagePathToRemove)
            {
                ImagePaths.Remove(imagePathToRemove);
            }
        }

        #endregion
    }
}