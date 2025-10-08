using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BookingApp.Utilities;
using BookingApp.Services.DTO;
using BookingApp.Domain.Interfaces.ServiceInterfaces;
using BookingApp.Domain.Interfaces;

namespace BookingApp.Presentation.ViewModel.Owner
{
    public class SystemSuggestionsViewModel : INotifyPropertyChanged
    {
        private readonly Action _navigateBack;
        private readonly Action<HighDemandLocationDTO> _navigateToAddAccommodation;
        private readonly ISystemSuggestionsService _suggestionsService;
        private readonly IAccommodationService _accommodationService;
        private readonly int _ownerId;

        public ObservableCollection<HighDemandLocationDTO> HighDemandLocations { get; set; }
        public ObservableCollection<LowDemandAccommodationDTO> LowDemandAccommodations { get; set; }

        public DateTime AnalysisDate { get; set; }

        public bool HasNoHighDemand => HighDemandLocations?.Count == 0;
        public bool HasNoLowDemand => LowDemandAccommodations?.Count == 0;

        public ICommand BackToHomeCommand { get; }
        public ICommand RefreshSuggestionsCommand { get; }
        public ICommand AddAccommodationCommand { get; }
        public ICommand ViewAccommodationCommand { get; }
        public ICommand CloseAccommodationCommand { get; }

        public SystemSuggestionsViewModel(
            ISystemSuggestionsService suggestionsService,
            IAccommodationService accommodationService,
            int ownerId,
            Action navigateBack = null,
            Action<HighDemandLocationDTO> navigateToAddAccommodation = null)
        {
            _suggestionsService = suggestionsService;
            _accommodationService = accommodationService;
            _ownerId = ownerId;
            _navigateBack = navigateBack;
            _navigateToAddAccommodation = navigateToAddAccommodation;

            HighDemandLocations = new ObservableCollection<HighDemandLocationDTO>();
            LowDemandAccommodations = new ObservableCollection<LowDemandAccommodationDTO>();

            BackToHomeCommand = new RelayCommand(BackToHome);
            RefreshSuggestionsCommand = new RelayCommand(RefreshSuggestions);
            AddAccommodationCommand = new RelayCommand<HighDemandLocationDTO>(AddAccommodation);
            ViewAccommodationCommand = new RelayCommand<LowDemandAccommodationDTO>(ViewAccommodation);
            CloseAccommodationCommand = new RelayCommand<LowDemandAccommodationDTO>(CloseAccommodation);

            LoadData();
        }

        private void LoadData()
        {
            AnalysisDate = DateTime.Now;

            HighDemandLocations.Clear();
            LowDemandAccommodations.Clear();

            // Učitavanje podataka iz servisa
            var highDemand = _suggestionsService.GetHighDemandLocations(_ownerId);
            var lowDemand = _suggestionsService.GetLowDemandAccommodations(_ownerId);

            foreach (var location in highDemand)
            {
                HighDemandLocations.Add(location);
            }

            foreach (var accommodation in lowDemand)
            {
                LowDemandAccommodations.Add(accommodation);
            }

            OnPropertyChanged(nameof(HasNoHighDemand));
            OnPropertyChanged(nameof(HasNoLowDemand));
        }

        private void BackToHome()
        {
            _navigateBack?.Invoke();
        }

        private void RefreshSuggestions()
        {
            LoadData();
            MessageBox.Show("Suggestions refreshed successfully", "System", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AddAccommodation(HighDemandLocationDTO location)
        {
            if (location != null)
            {
                _navigateToAddAccommodation?.Invoke(location);
            }
        }

        private void ViewAccommodation(LowDemandAccommodationDTO accommodation)
        {
            if (accommodation != null)
            {
                MessageBox.Show($"Viewing details for {accommodation.AccommodationName}",
                              "View Details", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CloseAccommodation(LowDemandAccommodationDTO accommodation)
        {
            if (accommodation != null)
            {
                var result = MessageBox.Show($"Are you sure you want to permanently delete {accommodation.AccommodationName}?\n\nThis action cannot be undone.","Delete Property",MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var accommodationEntity = _accommodationService.GetAccommodationById(accommodation.AccommodationId);
                        if (accommodationEntity != null)
                        {
                            _accommodationService.DeleteAccommodation(accommodationEntity);
                            LowDemandAccommodations.Remove(accommodation);
                            OnPropertyChanged(nameof(HasNoLowDemand));
                            MessageBox.Show("Property deleted successfully", "System", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting property: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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