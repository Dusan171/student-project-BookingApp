using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain;
using BookingApp.Domain.Model;
using BookingApp.Repositories;
using BookingApp.Services;
using BookingApp.Domain.Interfaces;
using BookingApp.Utilities;
using BookingApp.Presentation.View.Guide;
using System.Collections.Generic;

namespace BookingApp.Presentation.ViewModel.Guide
{
    public class TourLiveTrackingViewModel : INotifyPropertyChanged
    {
        private readonly TourRepository tourRepository = new TourRepository();
        private readonly TouristAttendanceRepository attendanceRepo = new TouristAttendanceRepository();
        private readonly ReservationGuestRepository guestsRepository = new ReservationGuestRepository();
        private readonly TourPresenceService _tourPresenceService;
        private readonly ITouristAttendanceService _touristAttendanceService;

        public Tour CurrentTour { get; private set; }
        public ObservableCollection<KeyPointViewModel> KeyPoints { get; set; } = new ObservableCollection<KeyPointViewModel>();
        public ObservableCollection<TouristAttendance> AttendanceList { get; set; } = new ObservableCollection<TouristAttendance>();

        public string TourName => CurrentTour?.Name ?? "";
        public string TourDateTime => CurrentTour != null && CurrentTour.StartTimes.Any() ? $"Date & Time: {CurrentTour.StartTimes.First().Time:yyyy-MM-dd HH:mm}" : "";
        public string TourLanguage => $"Language: {CurrentTour?.Language}";
        public string TourLocation => CurrentTour?.Location != null ? $"Location: {CurrentTour.Location.City}, {CurrentTour.Location.Country}" : "";
        public string TourDuration => $"Duration: {CurrentTour?.DurationHours}h";

        public ICommand KeyPointCheckedCommand { get; }
        public ICommand OpenTouristsWindowCommand { get; }
        public ICommand StopTourCommand { get; }

        private DateTime startTime;
        private bool touristsWindowOpened = false;

        public event PropertyChangedEventHandler PropertyChanged;
        
        public TourLiveTrackingViewModel(Tour tour = null, DateTime? start = null,
                                        TourPresenceService tourPresenceService = null,
                                        ITouristAttendanceService touristAttendanceService = null)
        {
            CurrentTour = tour ?? throw new ArgumentNullException(nameof(tour));
            _tourPresenceService = tourPresenceService;
            _touristAttendanceService = touristAttendanceService;

            CurrentTour.Status = TourStatus.ACTIVE;
            tourRepository.Update(CurrentTour);

            startTime = start ?? DateTime.Now;

            LoadReservations();
            LoadKeyPoints();

            KeyPointCheckedCommand = new RelayCommand<KeyPointViewModel>(kp => KeyPointChecked(kp));
            OpenTouristsWindowCommand = new RelayCommand(_ => OpenTouristWindow());
            StopTourCommand = new RelayCommand(_ => StopTour());
        }

        private void LoadReservations()
        {
            var reservationRepository = new TourReservationRepository();
            var reservations = reservationRepository.GetAll()
                .Where(r => r.TourId == CurrentTour.Id && r.Status == TourReservationStatus.ACTIVE)
                .ToList();

            AttendanceList.Clear();
            foreach (var r in reservations)
            {
                foreach (var g in r.Guests)
                {
                    AttendanceList.Add(new TouristAttendance
                    {
                        TourId = CurrentTour.Id,
                        GuestId = g.Id,
                        HasAppeared = false,
                        KeyPointJoinedAt = -1
                    });
                }
            }
        }

        private void LoadKeyPoints()
        {
            KeyPoints.Clear();
            int count = 1;
            foreach (var kp in CurrentTour.KeyPoints)
            {
                KeyPoints.Add(new KeyPointViewModel
                {
                    Name = $"#{count} {kp.Name}",
                    KeyPoint = kp,
                    IsChecked = count == 1
                });
                count++;
            }
        }

        private void KeyPointChecked(KeyPointViewModel kp)
        {
            int currentIndex = KeyPoints.IndexOf(kp);

            if (_tourPresenceService != null)
            {
                try
                {
                    _tourPresenceService.UpdateTourKeyPointProgress(CurrentTour.Id, currentIndex);
                }
                catch
                {
                    UpdateTourProgressOldWay(currentIndex);
                }
            }
            else
            {
                UpdateTourProgressOldWay(currentIndex);
            }

            if (KeyPoints.All(k => k.IsChecked))
            {
                if (!touristsWindowOpened)
                {
                    touristsWindowOpened = true;
                    OpenTouristWindow();
                }
                EndTour();
            }
        }

        private void UpdateTourProgressOldWay(int currentIndex)
        {
            var repo = new TourPresenceRepository();
            var activeTourists = repo.GetByTourId(CurrentTour.Id).Where(tp => tp.IsPresent).ToList();

            foreach (var t in activeTourists)
            {
                t.CurrentKeyPointIndex = currentIndex;
                t.LastUpdated = DateTime.Now;
                repo.Update(t);
            }
        }
        public event Action<Tour, List<KeyPoint>, List<ReservationGuest>, List<TouristAttendance>> NavigateToReservedTouristsPageRequested;

        private void OpenTouristWindow()
        {
            var passedKeyPoints = KeyPoints
                .Where(kp => kp.IsChecked)
                .Select(kp => kp.KeyPoint)
                .ToList();

            var guests = AttendanceList.Select(a => new ReservationGuest
            {
                Id = a.GuestId,
            }).ToList();

            NavigateToReservedTouristsPageRequested?.Invoke(CurrentTour, passedKeyPoints, guests, AttendanceList.ToList());
        }

        private void StopTour()
        {
            var result = MessageBox.Show("Da li ste sigurni da želite da završite turu ranije?", "Potvrda",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                EndTour();
            }
        }

        private void EndTour()
        {
            CurrentTour.Status = TourStatus.FINISHED;
            tourRepository.Update(CurrentTour);

            MessageBox.Show("Sve ključne tačke su prošle. Tura je uspešno završena.", "Kraj Ture", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class KeyPointViewModel : INotifyPropertyChanged
    {
        private bool isChecked;
        public string Name { get; set; }
        public KeyPoint KeyPoint { get; set; }
        public bool IsChecked { get => isChecked; set { isChecked = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked))); } }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
