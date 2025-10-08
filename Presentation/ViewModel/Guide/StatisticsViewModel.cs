using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain.Model;
using BookingApp.Repositories;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Guide
{
    public class YearlyRequestStats : INotifyPropertyChanged
    {
        public int Year { get; set; }
        public int TotalRequests => MonthlyRequests.Sum(x => x.Value);

        public Dictionary<string, int> MonthlyRequests { get; set; } = new();

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set 
            { 
                if (_isExpanded != value) 
                {
                    _isExpanded = value; 
                    OnPropertyChanged(); 
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    public class StatisticsViewModel : INotifyPropertyChanged
    {
        private readonly TourRepository _tourRepository;
        private readonly TouristAttendanceRepository _attendanceRepository;
        public ObservableCollection<YearlyRequestStats> YearlyRequestStatsList { get; set; }
        public ICommand ToggleYearExpansionCommand { get; }
        private bool _filterByLocation = true;
        public bool FilterByLocation
        {
            get => _filterByLocation;
            set
            {
                if (_filterByLocation != value) 
                {
                    _filterByLocation = value;
                    OnPropertyChanged();

                    if (_filterByLocation)
                        SelectedLanguage = null;
                    else
                        SelectedLocation = null;

                    ApplyRequestFilter(); 
                }
            }
        }
        public ObservableCollection<string> AvailableLocations { get; set; } = new();
        public ObservableCollection<string> AvailableLanguages { get; set; } = new();
        private List<TourRequest> TourRequests;

        private string _selectedLocation;
        public string SelectedLocation 
        { 
            get => _selectedLocation;
            set
            {
                if (_selectedLocation != value)
                {
                    _selectedLocation = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _selectedLanguage;
        public string SelectedLanguage 
        { 
            get => _selectedLanguage;
            set
            {
                if (_selectedLanguage != value)
                {
                    _selectedLanguage = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ApplyRequestFilterCommand { get; }
        public ICommand RefreshRequestStatsCommand { get; }


        private string _selectedYear;
        private Tour _mostVisitedTour;

        public ObservableCollection<string> Years { get; } = new ObservableCollection<string>();
        public ObservableCollection<Tour> OtherTours { get; } = new ObservableCollection<Tour>();

        public Tour MostVisitedTour
        {
            get => _mostVisitedTour;
            set 
            { 
                if (_mostVisitedTour != value) 
                {
                    _mostVisitedTour = value; 
                    OnPropertyChanged(); 
                }
            }
        }

        public string SelectedYear
        {
            get => _selectedYear;
            set
            {
                if (_selectedYear != value) 
                {
                    _selectedYear = value;
                    OnPropertyChanged();
                    UpdateStatistics();
                }
            }
        }

        private List<Tour> _finishedTours;

        public ICommand DetailsCommand { get; }
        public ICommand CreateTourSuggestionCommand { get; }

        public StatisticsViewModel()
        {
            _tourRepository = new TourRepository();
            _attendanceRepository = new TouristAttendanceRepository();

            DetailsCommand = new RelayCommand(ShowTourDetails);

            LoadFinishedTours();
            PopulateYearList();
            SelectedYear = "All Time";
            YearlyRequestStatsList = new ObservableCollection<YearlyRequestStats>();
            ToggleYearExpansionCommand = new RelayCommand(ToggleYearExpansion);
            RefreshRequestStatsCommand = new RelayCommand(RefreshRequestStats);
            CreateTourSuggestionCommand = new RelayCommand(CreateTourBySuggestion);
            TourRequestRepository tourRequestRepository = new TourRequestRepository();
            var allRequests = tourRequestRepository.GetAll().ToList();
            TourRequests = allRequests;
            ApplyRequestFilterCommand = new RelayCommand(ApplyRequestFilter);

            YearlyRequestStatsList = new ObservableCollection<YearlyRequestStats>();
            ToggleYearExpansionCommand = new RelayCommand(ToggleYearExpansion);

            FillAvailableValues();
            ApplyRequestFilter();


        }
        public Location GetSuggestionForMostRequestedLocation()
        {
            var lastYear = DateTime.Now.AddYears(-1);
            var mostRequested = TourRequests
                .Where(r => r.CreatedAt >= lastYear)
                .GroupBy(r => r.City)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key;

            if (mostRequested == null) return null;

            var locationRepo = new LocationRepository();
            return locationRepo.GetAll().FirstOrDefault(l => l.City == mostRequested);
        }

        public string GetSuggestionForMostRequestedLanguage()
        {
            var lastYear = DateTime.Now.AddYears(-1);
            return TourRequests
                .Where(r => r.CreatedAt >= lastYear)
                .GroupBy(r => r.Language)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key;
        }

        private void CreateTourBySuggestion()
        {
            if (FilterByLocation)
            {
                var suggestedLocation = GetSuggestionForMostRequestedLocation();
                if (suggestedLocation != null)
                    CreateTourByLocationSuggestion(suggestedLocation);
            }
            else
            {
                var suggestedLanguage = GetSuggestionForMostRequestedLanguage();
                if (!string.IsNullOrEmpty(suggestedLanguage))
                    CreateTourByLanguageSuggestion(suggestedLanguage);
            }
        }

        private void CreateTourByLocationSuggestion(Location suggestedLocation)
        {
            var newTour = new Tour
            {
                Location = suggestedLocation,
                Language = "" 
            };
            OnCreateTourRequested(newTour);
        }

        private void CreateTourByLanguageSuggestion(string suggestedLanguage)
        {
            var newTour = new Tour
            {
                Location = null, 
                Language = suggestedLanguage
            };
            OnCreateTourRequested(newTour);
        }

        public event Action<Tour> CreateTourRequested;
        protected virtual void OnCreateTourRequested(Tour tour)
        {
            CreateTourRequested?.Invoke(tour);
        }

        private void RefreshRequestStats()
        {
            FilterByLocation = true;

            SelectedLocation = null;
            SelectedLanguage = null;

            ApplyRequestFilter();
        }

        private void FillAvailableValues()
        {
            AvailableLocations = new ObservableCollection<string>(
                TourRequests
                    .Select(r => r.City)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Distinct()
                    .OrderBy(c => c)
            );

            AvailableLanguages = new ObservableCollection<string>(
                TourRequests
                    .Select(r => r.Language)
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .Distinct()
                    .OrderBy(l => l)
            );

            OnPropertyChanged(nameof(AvailableLocations));
            OnPropertyChanged(nameof(AvailableLanguages));
        }
        private void ApplyRequestFilter()
        {
            IEnumerable<TourRequest> filtered = TourRequests;

            if (FilterByLocation && !string.IsNullOrEmpty(SelectedLocation))
            {
                filtered = filtered.Where(r => r.City == SelectedLocation);
            }
            else if (!FilterByLocation && !string.IsNullOrEmpty(SelectedLanguage))
            {
                filtered = filtered.Where(r => r.Language == SelectedLanguage);
            }

            var grouped = filtered
                .GroupBy(r => r.CreatedAt.Year)
                .OrderByDescending(g => g.Key)
                .Select(g => new YearlyRequestStats
                {
                    Year = g.Key,
                    MonthlyRequests = g
                        .GroupBy(r => r.CreatedAt.ToString("MMMM"))
                        .OrderBy(mg => DateTime.ParseExact(mg.Key, "MMMM", System.Globalization.CultureInfo.InvariantCulture).Month)
                        .ToDictionary(mg => mg.Key, mg => mg.Count()),
                    IsExpanded = false
                });

            YearlyRequestStatsList.Clear();
            foreach (var y in grouped)
                YearlyRequestStatsList.Add(y);
        }

        private void LoadFinishedTours()
        {
            _finishedTours = _tourRepository.GetAll()
                                .Where(t => t.Status == TourStatus.FINISHED)
                                .ToList();

            FillTourDetails(_finishedTours);
        }

        private void PopulateYearList()
        {
            Years.Clear();
            Years.Add("All Time");
            foreach (var year in _finishedTours.SelectMany(t => t.StartTimes)
                                              .Select(st => st.Time.Year)
                                              .Distinct()
                                              .OrderByDescending(y => y))
            {
                Years.Add(year.ToString());
            }
        }

        private void FillTourDetails(List<Tour> tours)
        {
            var locationRepo = new LocationRepository();
            var keyPointRepo = new KeyPointRepository();
            var startTimeRepo = new StartTourTimeRepository();
            var imagesRepo = new ImageRepository();

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
                    .Select(img => imagesRepo.GetById(img.Id))
                    .Where(img => img != null)
                    .ToList();
            }
        }

        private void UpdateStatistics()
        {
            IEnumerable<Tour> filtered = _finishedTours;
            if (SelectedYear != "All Time")
            {
                int year = int.Parse(SelectedYear);
                filtered = filtered.Where(t => t.StartTimes.Any(st => st.Time.Year == year));
            }

            var attendances = _attendanceRepository.GetAll();
            if (!filtered.Any())
            {
                MostVisitedTour = null;
                OtherTours.Clear();
                return;
            }

            Dictionary<int, int> tourCount = new Dictionary<int, int>();
            foreach (var tour in filtered)
            {
                tourCount[tour.Id] = attendances.Count(a => a.TourId == tour.Id);
            }

            var mostVisitedId = tourCount.OrderByDescending(x => x.Value).First().Key;
            MostVisitedTour = filtered.FirstOrDefault(t => t.Id == mostVisitedId);

            OtherTours.Clear();
            foreach (var tour in filtered.Where(t => t.Id != MostVisitedTour?.Id))
            {
                OtherTours.Add(tour);
            }
        }

        private void ShowTourDetails(object parameter)
        {
            if (parameter is Tour tour)
            {
                OnTourDetailsRequested(tour);
            }
        }
        public event Action<Tour> TourDetailsRequested;
        protected virtual void OnTourDetailsRequested(Tour tour)
        {
            TourDetailsRequested?.Invoke(tour);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void ToggleYearExpansion(object parameter)
        {
            if (parameter is YearlyRequestStats stats)
            {
                stats.IsExpanded = !stats.IsExpanded;
            }
        }

    }
}
