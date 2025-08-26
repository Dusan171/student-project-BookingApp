using BookingApp.Domain.Interfaces;
using BookingApp.Domain;
using BookingApp.Repositories;
using BookingApp.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BookingApp.Services;
using BookingApp.Services.DTO;
using BookingApp.Presentation.View.Owner;

namespace BookingApp.Presentation.ViewModel.Owner
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private readonly IGuestReviewRepository guestReviewRepository;
        private readonly IOccupiedDateRepository occupiedDateRepository;
        private readonly IReservationRepository reservationRepository;
        private readonly IReservationService reservationService;
        private readonly IGuestReviewService guestReviewService;

        public event PropertyChangedEventHandler PropertyChanged;

        public event Action<string> ToastMessageRequested;

        private object _currentContent;
        public object CurrentContent
        {
            get => _currentContent;
            set
            {
                _currentContent = value;
                OnPropertyChanged(nameof(CurrentContent));
            }
        }
        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        private bool _hasUnratedGuests;
        public bool HasUnratedGuests
        {
            get => _hasUnratedGuests;
            set { _hasUnratedGuests = value; OnPropertyChanged(nameof(HasUnratedGuests)); }
        }

        public ObservableCollection<NotificationDTO> Notifications { get; set; }

        public ICommand ShowNotificationsCommand { get; }
        public ICommand CommunityCommand { get; }
        public ICommand LibraryCommand { get; }

        public HomeViewModel()
        {
            //Username = username;

            reservationRepository = Injector.CreateInstance<IReservationRepository>();
            occupiedDateRepository = Injector.CreateInstance<IOccupiedDateRepository>();
            guestReviewRepository = Injector.CreateInstance<IGuestReviewRepository>();

            reservationService = Injector.CreateInstance<IReservationService>();
            guestReviewService = Injector.CreateInstance<IGuestReviewService>();

            Notifications = new ObservableCollection<NotificationDTO>();

            ShowNotificationsCommand = new RelayCommand(ShowNotifications);
            CommunityCommand = new RelayCommand(OpenCommunity);
            LibraryCommand = new RelayCommand(OpenLibrary);

            CurrentContent = new WelcomeView();
            CheckForUnratedGuests();
        }

        private void ShowNotifications()
        {
            var allReservations = reservationService.GetAll();
            var notifications = NotificationsGenerator.Generate(allReservations, guestReviewRepository);

            Notifications.Clear();
            foreach (var n in notifications)
                Notifications.Add(n);
        }

        private void OpenCommunity()
        {
            // logika za Community
        }

        private void OpenLibrary()
        {
            // logika za Library
        }

        private void CheckForUnratedGuests()
        {
            var allReservations = reservationService.GetAll();
            var notifications = NotificationsGenerator.Generate(allReservations, guestReviewRepository);

            HasUnratedGuests = notifications.Any();

            if (HasUnratedGuests && !ToastShownToday())
            {
                ToastMessageRequested?.Invoke("🔔 You have guests to rate!");
                SaveToastShownDate(DateTime.Now);
            }
        }

        #region Toast helpers

        private const string ToastInfoFile = "toastShownDate.txt";

        private bool ToastShownToday()
        {
            if (!System.IO.File.Exists(ToastInfoFile))
                return false;

            string content = System.IO.File.ReadAllText(ToastInfoFile);
            if (DateTime.TryParse(content, out DateTime lastShown))
            {
                return lastShown.Date == DateTime.Today;
            }

            return false;
        }

        private void SaveToastShownDate(DateTime date)
        {
            System.IO.File.WriteAllText(ToastInfoFile, date.ToString("yyyy-MM-dd"));
        }

        #endregion

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}