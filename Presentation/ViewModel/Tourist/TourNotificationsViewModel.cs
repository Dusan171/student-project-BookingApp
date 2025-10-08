using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BookingApp.Services.DTO;
using BookingApp.Utilities;
using BookingApp.Domain.Interfaces;

namespace BookingApp.Presentation.ViewModel.Tourist
{
    public class TourNotificationsViewModel : INotifyPropertyChanged
    {
        private readonly int _currentUserId;
        private readonly ITourNotificationService _notificationService;

        private TourNotificationDTO _latestNotification;
        private bool _hasLatestNotification;
        private bool _showTourDetails;
        private NotifiedTourDTO _selectedTour;
        private ObservableCollection<TourNotificationDTO> _notificationHistory;
        private string _statusMessage = "";

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<NotifiedTourDTO> ReservationRequested;
        public event EventHandler<TourNotificationDTO> ShowDetailsRequested;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public TourNotificationDTO LatestNotification
        {
            get => _latestNotification;
            set { _latestNotification = value; OnPropertyChanged(); }
        }

        public bool HasLatestNotification
        {
            get => _hasLatestNotification;
            set { _hasLatestNotification = value; OnPropertyChanged(); }
        }


        public NotifiedTourDTO SelectedTour
        {
            get => _selectedTour;
            set { _selectedTour = value; OnPropertyChanged(); }
        }

        public ObservableCollection<TourNotificationDTO> NotificationHistory
        {
            get => _notificationHistory;
            set { _notificationHistory = value; OnPropertyChanged(); }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public bool HasNotifications => NotificationHistory?.Count > 0;

        public ICommand ViewTourDetailsCommand { get; }
        public ICommand ReserveTourCommand { get; }
       


        public TourNotificationsViewModel(int currentUserId, ITourNotificationService notificationService = null)
        {
            _currentUserId = currentUserId;
            _notificationService = notificationService ?? Services.Injector.CreateInstance<ITourNotificationService>();

            NotificationHistory = new ObservableCollection<TourNotificationDTO>();

            ViewTourDetailsCommand = new RelayCommand<TourNotificationDTO>(ViewTourDetails);
            ReserveTourCommand = new RelayCommand<NotifiedTourDTO>(ReserveTour);
          

            LoadNotifications();
        }

        public void LoadNotifications()
        {
            try
            {
                LoadLatestNotification();
                LoadNotificationHistory();
                StatusMessage = "";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Greška pri učitavanju obaveštenja: {ex.Message}";
            }
        }

        private void LoadLatestNotification()
        {
            try
            {
                var latest = _notificationService.GetLatestNotification(_currentUserId);

                if (latest != null)
                {
                    LatestNotification = latest;
                    HasLatestNotification = true;
                }
                else
                {
                    HasLatestNotification = false;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Greška pri učitavanju poslednje notifikacije: {ex.Message}";
                HasLatestNotification = false;
            }
        }

        private void LoadNotificationHistory()
        {
            try
            {
                NotificationHistory.Clear();

                var notifications = _notificationService.GetNotificationsByTourist(_currentUserId);

                foreach (var notification in notifications)
                {
                    NotificationHistory.Add(notification);
                }

                OnPropertyChanged(nameof(HasNotifications));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Greška pri učitavanju istorije: {ex.Message}";
            }
        }


        private void ReserveTour(NotifiedTourDTO tour)
        {
            try
            {
                if (tour != null)
                {
                    ReservationRequested?.Invoke(this, tour);
                    StatusMessage = $"Preusmeren na rezervaciju ture: {tour.Name}";
                }
                else
                {
                    StatusMessage = "Greška: Tura nije pronađena.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Greška pri rezervaciji ture: {ex.Message}";
            }
        }

        private void ViewTourDetails(TourNotificationDTO notification)
        {
            try
            {
                if (notification?.Tour != null)
                {
                    
                    _notificationService.MarkAsRead(notification.Id);

                  
                    ShowDetailsRequested?.Invoke(this, notification);

                    StatusMessage = "";
                }
                else
                {
                    StatusMessage = "Nema dostupnih detalja za ovu turu.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Greška pri prikazivanju detalja ture: {ex.Message}";
            }
        }

     
    }
}