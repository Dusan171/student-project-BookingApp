using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Tourist
{
    public class TourPresenceViewModel : INotifyPropertyChanged
    {
        private readonly int _currentUserId;

        private ObservableCollection<TourPresenceDTO> _activeTourPresences;
        private TourPresenceDTO _selectedTourPresence;
        private bool _hasActivePresence;

        private ObservableCollection<TourPresenceNotificationDTO> _unreadNotifications;
        private bool _hasUnreadNotifications;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ObservableCollection<TourPresenceDTO> ActiveTourPresences
        {
            get => _activeTourPresences;
            set { _activeTourPresences = value; OnPropertyChanged(); HasActivePresence = _activeTourPresences?.Count > 0; }
        }

        public TourPresenceDTO SelectedTourPresence
        {
            get => _selectedTourPresence;
            set { _selectedTourPresence = value; OnPropertyChanged(); }
        }

        public bool HasActivePresence
        {
            get => _hasActivePresence;
            set { _hasActivePresence = value; OnPropertyChanged(); }
        }

        public ObservableCollection<TourPresenceNotificationDTO> UnreadNotifications
        {
            get => _unreadNotifications;
            set { _unreadNotifications = value; HasUnreadNotifications = _unreadNotifications?.Count > 0; OnPropertyChanged(); }
        }

        public bool HasUnreadNotifications
        {
            get => _hasUnreadNotifications;
            set { _hasUnreadNotifications = value; OnPropertyChanged(); }
        }

        public ICommand RefreshCommand { get; }

        public TourPresenceViewModel(int currentUserId = 0)
        {
            _currentUserId = currentUserId;

            ActiveTourPresences = new ObservableCollection<TourPresenceDTO>();
            UnreadNotifications = new ObservableCollection<TourPresenceNotificationDTO>();
            RefreshCommand = new RelayCommand(_ => RefreshData());

            LoadActiveTours_Mock();
            LoadNotifications_Mock();
        }

        public void RefreshData()
        {
            LoadActiveTours_Mock();
            LoadNotifications_Mock();
        }

        public void LoadTourPresenceData() => RefreshData();

        public void MarkNotificationAsRead(int notificationId)
        {
            var item = UnreadNotifications.FirstOrDefault(n => n.Id == notificationId);
            if (item != null)
            {
                UnreadNotifications.Remove(item);
                HasUnreadNotifications = UnreadNotifications.Count > 0;
            }
        }

       
        private void LoadNotifications_Mock()
        {
            UnreadNotifications.Clear();
            UnreadNotifications.Add(new TourPresenceNotificationDTO
            {
                Id = 501,
                Message = "Vodič je zabeležio prisustvo na turi 'Tura Beograd' za: Milica Mitic, Jovana Jovic",
                CreatedAt = DateTime.Now.AddMinutes(-12),
                IsRead = false
            });
            HasUnreadNotifications = true;
        }

        private void LoadActiveTours_Mock()
        {
            ActiveTourPresences.Clear();

            var dto = new TourPresenceDTO
            {
                TourName = " Tura Beograd",
                JoinedAt = DateTime.Now.AddHours(-1),
                TotalKeyPoints = 5,
                IsPresent = true,
                LastUpdated = DateTime.Now.AddMinutes(-3),
                ProgressText = "Ključna tačka 2 od 5"
            };

            
            dto.CurrentKeyPointIndex = 2;
            dto.CurrentKeyPointName = "Kalemegdan";

            ActiveTourPresences.Add(dto);
            HasActivePresence = true;
        }
    }
}
