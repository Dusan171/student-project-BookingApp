using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Guest
{
    public class AccommodationDetailsViewModel : ViewModelBase
    {
        public static event Action GoBackToSearchRequested;
        public static event Action<AccommodationDetailsDTO> ReserveFromDetailsRequested;

        private readonly AccommodationDetailsDTO _accommodation;
        private int _currentImageIndex;

        #region Properties for Binding
        public string AccommodationName => _accommodation.Name;
        public string Location => $"{_accommodation.GeoLocation.City}, {_accommodation.GeoLocation.Country}";
        public string Type => _accommodation.Type;
        public int? MaxGuests => _accommodation.MaxGuests;
        public int? MinReservationDays => _accommodation.MinReservationDays;

        public List<string> ImagePaths { get; }

        private string _selectedImage;
        public string SelectedImage
        {
            get => _selectedImage;
            set { _selectedImage = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands
        public ICommand GoBackCommand { get; }
        public ICommand NextImageCommand { get; }
        public ICommand PreviousImageCommand { get; }
        public ICommand SelectImageCommand { get; }
        public ICommand ReserveCommand { get; }
        #endregion

        public AccommodationDetailsViewModel(AccommodationDetailsDTO accommodation)
        {
            _accommodation = accommodation;
            ImagePaths = _accommodation.ImagePaths;
            _currentImageIndex = 0;

            if (ImagePaths != null && ImagePaths.Any())
            {
                SelectedImage = ImagePaths[_currentImageIndex];
            }

            GoBackCommand = new RelayCommand(GoBack);
            NextImageCommand = new RelayCommand(NextImage, CanChangeImage);
            PreviousImageCommand = new RelayCommand(PreviousImage, CanChangeImage);
            SelectImageCommand = new RelayCommand(SelectImage);
            ReserveCommand = new RelayCommand(Reserve);
        }

        #region Command Logic
        private bool CanChangeImage(object obj) => ImagePaths != null && ImagePaths.Count > 1;

        private void NextImage(object obj)
        {
            _currentImageIndex = (_currentImageIndex + 1) % ImagePaths.Count;
            SelectedImage = ImagePaths[_currentImageIndex];
        }

        private void PreviousImage(object obj)
        {
            _currentImageIndex = (_currentImageIndex - 1 + ImagePaths.Count) % ImagePaths.Count;
            SelectedImage = ImagePaths[_currentImageIndex];
        }

        private void SelectImage(object imagePath)
        {
            SelectedImage = imagePath as string;
            _currentImageIndex = ImagePaths.IndexOf(SelectedImage);
        }

        private void Reserve(object obj)
        {
            ReserveFromDetailsRequested?.Invoke(_accommodation);
        }

        private void GoBack(object obj)
        {
            GoBackToSearchRequested?.Invoke();
        }
        #endregion
    }
}