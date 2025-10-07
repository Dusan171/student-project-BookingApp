using BookingApp.Domain.Model;
using BookingApp.Presentation.View.Guide;
using BookingApp.Repositories;
using BookingApp.Services;
using BookingApp.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookingApp.Presentation.ViewModel.Guide
{
    public class ToursControlViewModel : ViewModelBase
    {
        private readonly TourRepository tourRepo = new();
        private readonly TourReservationRepository reservationRepo = new();
        private readonly TourReportService reportService = new();

        public ObservableCollection<TourCardViewModel> Tours { get; set; } = new();

        public List<string> FilterOptions { get; } = new() { "Tours Today", "All Tours" };

        private string _selectedFilter;
        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                OnPropertyChanged();
                LoadTours();
            }
        }

        private List<Tour> allTours;
        private List<TourReservation> allReservations;

        public ICommand StartTourCommand { get; }
        public ICommand CancelTourCommand { get; }
        public ICommand NewTourCommand { get; }
        public ICommand GenerateReportCommand { get; }
        public ICommand ShowTourDetailsCommand { get; }
        // Događaji
        public event Action<Page> NavigationRequested;
        public event Action<string> PDFGenerated;

        public ToursControlViewModel()
        {
            StartTourCommand = new RelayCommand(StartTour);
            CancelTourCommand = new RelayCommand(CancelTour);
            NewTourCommand = new RelayCommand(NewTour);
            GenerateReportCommand = new RelayCommand(GenerateReport);
            ShowTourDetailsCommand = new RelayCommand(ShowTourDetails);

            allTours = tourRepo.GetAll();
            allReservations = reservationRepo.GetAll();

            FillTourDetails(allTours);

            SelectedFilter = "Tours Today";
        }
        private void ShowTourDetails(object obj)
        {
            if (obj is not TourCardViewModel vm) return;

            var detailsPage = new DetailedTourPage(vm.Tour);
            NavigationRequested?.Invoke(detailsPage);
        }

        private void FillTourDetails(List<Tour> tours)
        {
            var locationRepo = new LocationRepository();
            var keyPointRepo = new KeyPointRepository();
            var startTimeRepo = new StartTourTimeRepository();
            var imageRepo = new ImageRepository();

            foreach (var tour in tours)
            {
                if (tour.Location != null)
                    tour.Location = locationRepo.GetById(tour.Location.Id);

                tour.KeyPoints = tour.KeyPoints
                    .Select(kp => keyPointRepo.GetById(kp.Id))
                    .Where(kp => kp != null)
                    .ToList();

                tour.StartTimes = tour.StartTimes
                    .Select(st => startTimeRepo.GetById(st.Id))
                    .Where(st => st != null)
                    .ToList();

                tour.Images = tour.Images
                    .Select(img => imageRepo.GetById(img.Id))
                    .Where(img => img != null)
                    .ToList();
            }
        }

        private void LoadTours()
        {
            var tours = allTours.Where(t => t.GuideId == Session.CurrentUser.Id).ToList();
            var filtered = SelectedFilter == "Tours Today"
                ? tours.Where(t => t.StartTimes.Any(st => st.Time.Date == DateTime.Today)).ToList()
                : tours;

            Tours.Clear();
            foreach (var t in filtered)
            {
                if (t.StartTimes != null && t.StartTimes.Any() && t.Status != TourStatus.CANCELLED)
                    Tours.Add(new TourCardViewModel(t, allReservations, allTours));
            }
        }

        private void StartTour(object obj)
        {
            if (obj is not TourCardViewModel vm) return;

            if (vm.IsActive)
            {
                var livePage = new TourLiveTrackingControl(vm.Tour, vm.StartTime);
                NavigationRequested?.Invoke(livePage);
                return;
            }

            if (!vm.HasActiveReservation)
            {
                MessageBox.Show("Tura ne može početi jer nema rezervisanih turista.", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            vm.Tour.Status = TourStatus.ACTIVE;
            tourRepo.Update(vm.Tour);
            
            var newLivePage = new TourLiveTrackingControl(vm.Tour, vm.StartTime);
            vm.Refresh();
            NavigationRequested?.Invoke(newLivePage);
        }

        private void NewTour(object obj)
        {
            NavigationRequested?.Invoke(new CreateTourControl());
        }

        private void CancelTour(object obj)
        {
            if (obj is not TourCardViewModel vm) return;

            var result = MessageBox.Show("Da li ste sigurni da želite otkazati ovu turu?",
                "Potvrda", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            vm.Tour.Status = TourStatus.CANCELLED;
            tourRepo.Update(vm.Tour);


            MessageBox.Show("Tura je uspešno otkazana.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            vm.Refresh();
            LoadTours();
        }

        private void GenerateReport(object obj)
        {
            try
            {
                var paramArray = obj as object[];
                if (paramArray == null || paramArray.Length < 2)
                {
                    MessageBox.Show("Molimo odaberite datum početka i kraja perioda.", "Greška",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (paramArray[0] is not DateTime startDate || paramArray[1] is not DateTime endDate)
                {
                    MessageBox.Show("Neispravno odabrani datumi.", "Greška",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (startDate > endDate)
                {
                    MessageBox.Show("Datum početka mora biti pre datuma kraja.", "Greška",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var guideTours = allTours.Where(t => t.GuideId == Session.CurrentUser.Id).ToList();

                var toursWithDatesInPeriod = new List<Tour>();

                foreach (var tour in guideTours)
                {
                    var hasTerminInPeriod = tour.StartTimes?.Any(st =>
                        st.Time.Date >= startDate.Date && st.Time.Date <= endDate.Date) ?? false;

                    if (hasTerminInPeriod)
                    {
                        toursWithDatesInPeriod.Add(tour);
                    }
                }

                if (!toursWithDatesInPeriod.Any())
                {
                    MessageBox.Show($"Nema tura u periodu od {startDate:dd.MM.yyyy} do {endDate:dd.MM.yyyy}.",
                        "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                string filePath = reportService.GenerateTourReport(toursWithDatesInPeriod, startDate, endDate, Session.CurrentUser);

                PDFGenerated?.Invoke(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška prilikom kreiranja izveštaja: {ex.Message}", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class TourCardViewModel : ViewModelBase
    {
        private readonly List<Tour> allTours;

        public Tour Tour { get; }
        public DateTime StartTime => Tour.StartTimes.First().Time;

        public string Name => Tour.Name;
        public string Language => Tour.Language;
        public string Duration => $"Duration: {Tour.DurationHours}h";
        public string Date => StartTime.ToString("dd.MM.yyyy, HH:mm");
        public string ImagePath => Tour.Images?.FirstOrDefault()?.FullPath;

        public bool IsToday => StartTime.Date == DateTime.Today;
        public bool HasActiveReservation { get; }
        public bool IsActive => Tour.Status == TourStatus.ACTIVE;
        public bool CanStart => HasActiveReservation && !IsActive && Tour.Status != TourStatus.FINISHED && Tour.Status != TourStatus.CANCELLED && !HasOngoingTour(Tour.Id);
        public bool CanCancel => (StartTime - DateTime.Now).TotalHours >= 48 && Tour.Status != TourStatus.ACTIVE;
        public bool CanTrack => IsActive;

        public TourCardViewModel(Tour tour, List<TourReservation> reservations, List<Tour> allToursList)
        {
            Tour = tour;
            allTours = allToursList;
            HasActiveReservation = reservations.Any(r => r.TourId == tour.Id && r.Status == 0);
        }
        public void Refresh()
        {
            OnPropertyChanged(nameof(IsActive));
            OnPropertyChanged(nameof(CanStart));
            OnPropertyChanged(nameof(CanCancel));
            OnPropertyChanged(nameof(CanTrack));
            OnPropertyChanged(nameof(Date));
        }

        private bool HasOngoingTour(int id)
        {
            return allTours.Any(t => t.Status == TourStatus.ACTIVE && t.Id != id);
        }
    }
}