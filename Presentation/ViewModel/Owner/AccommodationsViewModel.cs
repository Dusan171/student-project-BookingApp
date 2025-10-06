using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Owner
{
    public class AccommodationsViewModel : INotifyPropertyChanged
    {
        private readonly IAccommodationService _accommodationService;
        private readonly IAccommodationImageService _imageService; // DODAJ OVO
        private readonly IUserService _userService;
        private readonly Action _navigateToAdd;

        private ObservableCollection<AccommodationDTO> _accommodations;
        public ObservableCollection<AccommodationDTO> Accommodations
        {
            get => _accommodations;
            set
            {
                _accommodations = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasNoAccommodations));
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterAccommodations();
            }
        }

        public bool HasNoAccommodations => Accommodations?.Count == 0;

        public ICommand AddNewCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand BackToHomeCommand { get; set; }

       
        public AccommodationsViewModel(IAccommodationService accommodationService,
                                     IAccommodationImageService imageService, // DODAJ OVO
                                     IUserService userService,
                                     Action goBackAction,
                                     Action navigateToAdd)
        {
            _accommodationService = accommodationService;
            _imageService = imageService; // DODAJ OVO
            _userService = userService;
            _navigateToAdd = navigateToAdd;

            AddNewCommand = new RelayCommand(param => AddNew());
            DeleteCommand = new RelayCommand(param => Delete(param));
            BackToHomeCommand = new RelayCommand(param => goBackAction?.Invoke());

            LoadAccommodations();
        }

        private void LoadAccommodations()
        {
            try
            {
                int currentOwnerId = _userService.GetCurrentUserId();
                var ownerAccommodations = _accommodationService.GetAccommodationsByOwnerId(currentOwnerId);

                // DODAJ OVO - učitaj slike za svaki smeštaj
                foreach (var accommodation in ownerAccommodations)
                {
                    accommodation.ImagePaths = _imageService.GetImagesByAccommodation(accommodation);
                }

                Accommodations = new ObservableCollection<AccommodationDTO>(ownerAccommodations);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading accommodations: {ex.Message}");
                Accommodations = new ObservableCollection<AccommodationDTO>();
            }
        }

        private void FilterAccommodations()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadAccommodations();
                return;
            }

            try
            {
                int currentOwnerId = _userService.GetCurrentUserId();
                var allAccommodations = _accommodationService.GetAccommodationsByOwnerId(currentOwnerId);

                // DODAJ I OVDE - učitaj slike
                foreach (var accommodation in allAccommodations)
                {
                    accommodation.ImagePaths = _imageService.GetImagesByAccommodation(accommodation);
                }

                var filtered = allAccommodations.Where(a =>
                    a.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    a.GeoLocation.City.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    a.GeoLocation.Country.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                ).ToList();

                Accommodations = new ObservableCollection<AccommodationDTO>(filtered);
            }
            catch
            {
                LoadAccommodations();
            }
        }

        private void AddNew()
        {
            _navigateToAdd?.Invoke();
        }

        private void Delete(object parameter)
        {
            if (parameter is AccommodationDTO accommodation)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete '{accommodation.Name}'?",
                    "Delete Accommodation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _accommodationService.DeleteAccommodation(accommodation);
                        LoadAccommodations();
                        MessageBox.Show("Accommodation deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting accommodation: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}