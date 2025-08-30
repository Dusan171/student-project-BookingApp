using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Tourist
{
    public class TourPresenceViewModel : INotifyPropertyChanged
    {
        private readonly ITourPresenceService _presenceService;
        private readonly ITourService _tourService;
        private readonly int _currentUserId;

        private ObservableCollection<TourPresenceDTO> _activeTourPresences;
        private TourPresenceDTO _selectedTourPresence;
        private bool _hasActivePresence;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<TourPresenceDTO> ActiveTourPresences
        {
            get => _activeTourPresences;
            set
            {
                _activeTourPresences = value;
                OnPropertyChanged();
                HasActivePresence = _activeTourPresences?.Count > 0;
            }
        }

        public TourPresenceDTO SelectedTourPresence
        {
            get => _selectedTourPresence;
            set
            {
                _selectedTourPresence = value;
                OnPropertyChanged();
            }
        }

        public bool HasActivePresence
        {
            get => _hasActivePresence;
            set
            {
                _hasActivePresence = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; }

        public TourPresenceViewModel(ITourPresenceService presenceService,
                                   ITourService tourService,
                                   int currentUserId)
        {
            _presenceService = presenceService ?? throw new ArgumentNullException(nameof(presenceService));
            _tourService = tourService ?? throw new ArgumentNullException(nameof(tourService));
            _currentUserId = currentUserId;

            ActiveTourPresences = new ObservableCollection<TourPresenceDTO>();

            RefreshCommand = new RelayCommand(RefreshActiveTours);

            LoadActiveTours();
        }

        private void LoadActiveTours()
        {
            try
            {
                var activePresences = _presenceService.GetActiveTourPresences(_currentUserId);

                ActiveTourPresences.Clear();
                foreach (var presence in activePresences)
                {
                    ActiveTourPresences.Add(presence);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju aktivnih tura: {ex.Message}",
                               "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshActiveTours(object parameter)
        {
            LoadActiveTours();
        }
    }
}
