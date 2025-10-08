using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Interfaces.ServiceInterfaces;
using BookingApp.Services.DTO;
using BookingApp.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace BookingApp.Presentation.ViewModel.Owner
{
    public class RegisterAccommodationViewModel : INotifyPropertyChanged
    {
        private readonly IAccommodationService _accommodationService;
        private readonly Action _navigateBack;
        private readonly IImageFileHandler _fileHandler; // NOVO: Zavisnost za fajl sistem
        private readonly IAccommodationValidationService _validationService; // DODAJ

        private AccommodationDTO _accommodation;
        public AccommodationDTO Accommodation
        {
            get => _accommodation;
            set
            {
                _accommodation = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<AccommodationImageDTO> _imagePaths;
        public ObservableCollection<AccommodationImageDTO> ImagePaths
        {
            get => _imagePaths;
            set
            {
                _imagePaths = value;
                OnPropertyChanged();
            }
        }

        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand BrowseImagesCommand { get; }
        public ICommand RemoveImageCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public RegisterAccommodationViewModel(IAccommodationService accommodationService, IImageFileHandler fileHandler, IAccommodationValidationService validationService,Action navigateBack = null)
        {
            _accommodationService = accommodationService;
            _navigateBack = navigateBack;
            _fileHandler = fileHandler; // NOVO: Inicijalizacija
            _validationService = validationService; // INICIJALIZACIJA

            Accommodation = new AccommodationDTO
            {
                CancellationDeadlineDays = 1,
                GeoLocation = new LocationDTO()
            };
            ImagePaths = new ObservableCollection<AccommodationImageDTO>();

            ConfirmCommand = new RelayCommand(_ => Confirm(), _ => true);
            CancelCommand = new RelayCommand(_ => Cancel());
            BrowseImagesCommand = new RelayCommand(_ => BrowseImages());
            RemoveImageCommand = new RelayCommand(param => RemoveImage(param));
        }
        private void BrowseImages()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Multiselect = true,
                Title = "Select Property Images"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filePath in openFileDialog.FileNames)
                {
                    try
                    {
                        string relativePath = _fileHandler.CopyImageToResources(filePath);
                        if (!ImagePaths.Any(img => img.Path == relativePath))
                        {
                            ImagePaths.Add(new AccommodationImageDTO { Path = relativePath, AccommodationId = 0 });
                        }
                    }
                    catch (Exception ex) { ShowMessage($"Error adding image: {ex.Message}", "Error");}
                }
                OnPropertyChanged(nameof(ImagePaths));
                CommandManager.InvalidateRequerySuggested();
            }
        }
        private void RemoveImage(object parameter)
        {
            if (parameter is AccommodationImageDTO image)
            {
                ImagePaths.Remove(image);
                OnPropertyChanged(nameof(ImagePaths));
                CommandManager.InvalidateRequerySuggested();
            }
        }
        private bool CanConfirm()
        {
            return Accommodation != null &&
              !string.IsNullOrWhiteSpace(Accommodation.Name) &&
              Accommodation.MaxGuests > 0 &&
              Accommodation.MinReservationDays > 0;
        }

        private void Confirm()
        {
            if (!_validationService.IsAccommodationValid(Accommodation, out string errorMessage))
            { ShowMessage(errorMessage, "Validation Error"); return; }

            try
            {
                if (Session.CurrentUser != null)
                {
                    Accommodation.OwnerId = Session.CurrentUser.Id;
                }

                Accommodation.ImagePaths = ImagePaths.ToList();

                var success = _accommodationService.RegisterAccommodation(Accommodation);

                if (success)
                {
                    ShowMessage("Property created successfully!", "Success");
                    ResetForm();
                    _navigateBack?.Invoke();
                }
                else
                {
                    ShowMessage("Failed to create property. Please check all fields.", "Error");
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error saving accommodation: {ex.Message}", "Error");
            }
        }
        private void Cancel()
        {
            ResetForm();
            _navigateBack?.Invoke();
        }
        private void ResetForm()
        {
            Accommodation = new AccommodationDTO
            {
                CancellationDeadlineDays = 1,
                GeoLocation = new LocationDTO()
            };
            ImagePaths.Clear();

            OnPropertyChanged(nameof(Accommodation));
            OnPropertyChanged(nameof(ImagePaths));
        }
        private void ShowMessage(string text, string title = "Information")
        {
            var icon = title == "Error" || title == "Validation Error"
                ? MessageBoxImage.Error
                : MessageBoxImage.Information;

            MessageBox.Show(text, title, MessageBoxButton.OK, icon);
        }
    }
}