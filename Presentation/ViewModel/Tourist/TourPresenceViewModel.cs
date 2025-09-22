using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Services;
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
        private readonly ITourPresenceNotificationService _notificationService;


        private ObservableCollection<TourPresenceNotificationDTO> _unreadNotifications;
        private bool _hasUnreadNotifications;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<TourPresenceNotificationDTO> UnreadNotifications
        {
            get => _unreadNotifications;
            set
            {
                _unreadNotifications = value;
                HasUnreadNotifications = value?.Count > 0;
                OnPropertyChanged();
            }
        }

        public bool HasUnreadNotifications
        {
            get => _hasUnreadNotifications;
            set
            {
                _hasUnreadNotifications = value;
                OnPropertyChanged();
            }
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
            _notificationService = Injector.CreateInstance<ITourPresenceNotificationService>();
            UnreadNotifications = new ObservableCollection<TourPresenceNotificationDTO>();

            LoadActiveTours();
            LoadNotifications();
       
        }

        private void LoadNotifications()
        {
            var notifications = _notificationService.GetUnreadByUserId(_currentUserId);
            UnreadNotifications = new ObservableCollection<TourPresenceNotificationDTO>(notifications);
        }

        public void MarkNotificationAsRead(int notificationId)
        {
            _notificationService.MarkAsRead(notificationId);
            LoadNotifications(); 
        }


        public void LoadTourPresenceData()
        {
            LoadActiveTours();
        }

        private void LoadActiveTours()
        {
            try
            {
                var activePresences = _presenceService.GetActiveTourPresences(_currentUserId);
             
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    ActiveTourPresences.Clear();

                    if (activePresences != null)
                    {
                        foreach (var presence in activePresences)
                        {
                            ActiveTourPresences.Add(presence);
                        }
                    }

                    //  ažuriraj HasActivePresence
                    HasActivePresence = ActiveTourPresences.Count > 0;
                    OnPropertyChanged(nameof(HasActivePresence));
                });
            }
            catch (Exception ex)
            {
                ActiveTourPresences?.Clear();
                MessageBox.Show($"Greška pri učitavanju aktivnih tura: {ex.Message}",
                               "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void RefreshData()
        {
            LoadActiveTours();
            LoadNotifications();
        }

        private void RefreshActiveTours(object parameter = null)
        {
            LoadActiveTours();
            LoadNotifications();
        }
    }
}