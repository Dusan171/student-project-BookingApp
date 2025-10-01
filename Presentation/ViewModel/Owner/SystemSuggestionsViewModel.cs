using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BookingApp.Utilities;
using BookingApp.Services.DTO;

namespace BookingApp.Presentation.ViewModel.Owner
{
    public class SystemSuggestionsViewModel : INotifyPropertyChanged
    {
        private readonly Action _navigateBack;
        private readonly Action<HighDemandLocationDTO> _navigateToAddAccommodation;

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

        public SystemSuggestionsViewModel(Action navigateBack = null, Action<HighDemandLocationDTO> navigateToAddAccommodation = null)
        {
            _navigateBack = navigateBack;
            _navigateToAddAccommodation = navigateToAddAccommodation;

            HighDemandLocations = new ObservableCollection<HighDemandLocationDTO>();
            LowDemandAccommodations = new ObservableCollection<LowDemandAccommodationDTO>();

            BackToHomeCommand = new RelayCommand(BackToHome);
            RefreshSuggestionsCommand = new RelayCommand(RefreshSuggestions);
            AddAccommodationCommand = new RelayCommand<HighDemandLocationDTO>(AddAccommodation);
            ViewAccommodationCommand = new RelayCommand<LowDemandAccommodationDTO>(ViewAccommodation);
            CloseAccommodationCommand = new RelayCommand<LowDemandAccommodationDTO>(CloseAccommodation);

            LoadMockData();
        }

        private void LoadMockData()
        {
            AnalysisDate = DateTime.Now;

            // High Demand Mock Data
            HighDemandLocations.Add(new HighDemandLocationDTO
            {
                City = "Belgrade",
                Country = "Serbia",
                ReservationCount = 85,
                OccupancyRate = 92.5,
                Recommendation = "Excellent location for new property investment"
            });

            HighDemandLocations.Add(new HighDemandLocationDTO
            {
                City = "Novi Sad",
                Country = "Serbia",
                ReservationCount = 67,
                OccupancyRate = 88.3,
                Recommendation = "Growing tourism market with high potential"
            });

            HighDemandLocations.Add(new HighDemandLocationDTO
            {
                City = "Zlatibor",
                Country = "Serbia",
                ReservationCount = 74,
                OccupancyRate = 91.2,
                Recommendation = "Popular mountain resort destination"
            });

            HighDemandLocations.Add(new HighDemandLocationDTO
            {
                City = "Kopaonik",
                Country = "Serbia",
                ReservationCount = 58,
                OccupancyRate = 86.7,
                Recommendation = "Ski resort with year-round appeal"
            });

            // Low Demand Mock Data
            LowDemandAccommodations.Add(new LowDemandAccommodationDTO
            {
                AccommodationName = "Villa Sunset",
                City = "Pancevo",
                Country = "Serbia",
                ReservationCount = 3,
                OccupancyRate = 15.2,
                Recommendation = "Consider relocating or closing this property"
            });

            LowDemandAccommodations.Add(new LowDemandAccommodationDTO
            {
                AccommodationName = "Riverside Apartments",
                City = "Smederevo",
                Country = "Serbia",
                ReservationCount = 7,
                OccupancyRate = 23.8,
                Recommendation = "Low performance, review pricing strategy"
            });

            OnPropertyChanged(nameof(HasNoHighDemand));
            OnPropertyChanged(nameof(HasNoLowDemand));
        }

        private void BackToHome()
        {
            _navigateBack?.Invoke();
        }

        private void RefreshSuggestions()
        {
            LoadMockData();
            MessageBox.Show("Suggestions refreshed (F5)", "System", MessageBoxButton.OK, MessageBoxImage.Information);
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
                var result = MessageBox.Show($"Are you sure you want to close {accommodation.AccommodationName}?",
                                           "Close Property", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    LowDemandAccommodations.Remove(accommodation);
                    OnPropertyChanged(nameof(HasNoLowDemand));
                    MessageBox.Show("Property closed successfully", "System", MessageBoxButton.OK, MessageBoxImage.Information);
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