using BookingApp.Domain.Interfaces;
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

        public RegisterAccommodationViewModel(IAccommodationService accommodationService, Action navigateBack = null)
        {
            _accommodationService = accommodationService;
            _navigateBack = navigateBack;

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
                        string relativePath = CopyImageToResources(filePath);

                        if (!ImagePaths.Any(img => img.Path == relativePath))
                        {
                            ImagePaths.Add(new AccommodationImageDTO
                            {
                                Path = relativePath,
                                AccommodationId = 0
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowMessage($"Error adding image: {ex.Message}", "Error");
                    }
                }

                OnPropertyChanged(nameof(ImagePaths));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string CopyImageToResources(string sourcePath)
        {
            try
            {
                string baseDir = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "..", "..", "..", "Resources", "Images"
                );
                baseDir = Path.GetFullPath(baseDir);

                if (!Directory.Exists(baseDir))
                {
                    Directory.CreateDirectory(baseDir);
                }

                string fileName = Path.GetFileName(sourcePath);
                string destPath = Path.Combine(baseDir, fileName);

                if (File.Exists(destPath))
                {
                    string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                    string extension = Path.GetExtension(fileName);
                    fileName = $"{fileNameWithoutExt}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    destPath = Path.Combine(baseDir, fileName);
                }

                File.Copy(sourcePath, destPath, true);

                return fileName;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error copying image: {ex.Message}");
                return Path.GetFileName(sourcePath);
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
            return !string.IsNullOrWhiteSpace(Accommodation?.Name) &&
                   Accommodation?.GeoLocation != null &&
                   !string.IsNullOrWhiteSpace(Accommodation.GeoLocation.City) &&
                   !string.IsNullOrWhiteSpace(Accommodation.GeoLocation.Country) &&
                   !string.IsNullOrWhiteSpace(Accommodation?.Type) &&
                   Accommodation?.MaxGuests.HasValue == true &&
                   Accommodation.MaxGuests > 0 &&
                   Accommodation?.MinReservationDays.HasValue == true &&
                   Accommodation.MinReservationDays > 0 &&
                   Accommodation?.CancellationDeadlineDays > 0;
        
        }

        private void Confirm()
        {
            if (!ValidateAccommodation())
                return;

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

        private bool ValidateAccommodation()
        {
            if (string.IsNullOrWhiteSpace(Accommodation.Name))
            {
                ShowMessage("Please enter a property name.", "Validation Error");
                return false;
            }

            if (Accommodation.GeoLocation == null ||
                string.IsNullOrWhiteSpace(Accommodation.GeoLocation.City) ||
                string.IsNullOrWhiteSpace(Accommodation.GeoLocation.Country))
            {
                ShowMessage("Please enter a valid location (City, Country).", "Validation Error");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Accommodation.Type))
            {
                ShowMessage("Please select a property type.", "Validation Error");
                return false;
            }

            if (!Accommodation.MaxGuests.HasValue || Accommodation.MaxGuests < 1)
            {
                ShowMessage("Maximum guests must be at least 1.", "Validation Error");
                return false;
            }

            if (!Accommodation.MinReservationDays.HasValue || Accommodation.MinReservationDays < 1)
            {
                ShowMessage("Minimum reservation days must be at least 1.", "Validation Error");
                return false;
            }

            if (Accommodation.CancellationDeadlineDays < 1)
            {
                ShowMessage("Cancellation deadline must be at least 1 day.", "Validation Error");
                return false;
            }


            return true;
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