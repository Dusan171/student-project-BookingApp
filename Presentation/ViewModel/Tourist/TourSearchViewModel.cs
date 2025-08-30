using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Tourist
{
    public class TourSearchViewModel : ViewModelBase
    {
        private readonly ITourService _tourService;
        private string _searchCity = "";
        private string _searchCountry = "";
        private string _searchLanguage = "srpski";
        private string _searchDuration = "";
        private string _searchPeopleCount = "1";
        private bool _isLoading = false;
        private string _statusMessage = "";

        public string SearchCity { get => _searchCity; set => SetProperty(ref _searchCity, value); }
        public string SearchCountry { get => _searchCountry; set => SetProperty(ref _searchCountry, value); }
        public string SearchLanguage { get => _searchLanguage; set => SetProperty(ref _searchLanguage, value); }
        public string SearchDuration { get => _searchDuration; set => SetProperty(ref _searchDuration, value); }
        public string SearchPeopleCount { get => _searchPeopleCount; set => SetProperty(ref _searchPeopleCount, value); }
        public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
        public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

        public ObservableCollection<TourDTO> Tours { get; set; } = new();
        public ObservableCollection<string> Languages { get; set; } = new() { "srpski", "engleski", "nemački", "francuski", "španski", "italijanski" };

        public ICommand SearchToursCommand { get; private set; }
        public ICommand ReserveTourCommand { get; private set; }
        public ICommand ClearSearchCommand { get; private set; }

        public event Action<TourDTO> TourReserveRequested;

        public TourSearchViewModel() : this(null)
        {
        }
        public TourSearchViewModel(ITourService tourService)
        {
            _tourService = tourService ?? Services.Injector.CreateInstance<ITourService>();
            InitializeCommands();
            LoadInitialTours();
        }

        private void InitializeCommands()
        {
            SearchToursCommand = new RelayCommand(ExecuteSearchTours);
            ReserveTourCommand = new RelayCommand<TourDTO>(ExecuteReserveTour);
            ClearSearchCommand = new RelayCommand(ExecuteClearSearch);
        }

        private void LoadInitialTours()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Učitavanje dostupnih tura...";

                var tours = _tourService.GetAllTours();
                Tours.Clear();

                foreach (var tour in tours)
                {
                    Tours.Add(TourDTO.FromDomain(tour));
                }

                StatusMessage = $"Pronađeno {Tours.Count} dostupnih tura";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Greška pri učitavanju tura: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ExecuteSearchTours()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Pretražujem ture...";

                var criteria = new SearchCriteriaDTO
                {
                    City = string.IsNullOrWhiteSpace(SearchCity) ? null : SearchCity.Trim(),
                    Country = string.IsNullOrWhiteSpace(SearchCountry) ? null : SearchCountry.Trim(),
                    Language = string.IsNullOrWhiteSpace(SearchLanguage) || SearchLanguage == "srpski" ? null : SearchLanguage,
                    MaxPeople = ParseIntValue(SearchPeopleCount),
                    Duration = ParseDoubleValue(SearchDuration)
                };

                var searchResults = _tourService.SearchTours(criteria);

                Tours.Clear();
                foreach (var tour in searchResults)
                {
                    Tours.Add(TourDTO.FromDomain(tour));
                }

                if (Tours.Count == 0)
                {
                    StatusMessage = "Nema tura koje odgovaraju kriterijumima pretrage";
                }
                else
                {
                    StatusMessage = $"Pronađeno {Tours.Count} tura";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Greška pri pretrazi: {ex.Message}";
                Tours.Clear();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ExecuteReserveTour(TourDTO tour)
        {
            if (tour == null) return;

            try
            {
                var reservationVM = new TourReservationViewModel(tour);

                // Dodaj event listener za refresh
                reservationVM.ReservationCompleted += () => {
                    RefreshTours(); // Već postojeća metoda!
                };
                // Увек иди на резервацијски екран, без обзира на попуњеност
                TourReserveRequested?.Invoke(tour);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Greška pri rezervaciji: {ex.Message}";
            }
        }

        private void ShowAlternativeTours(TourDTO originalTour)
        {
            try
            {
                // Узми број људи из претраге (default 1)
                int requiredSpots = ParseIntValue(SearchPeopleCount) ?? 1;

                // Пронађи алтернативне туре на истој локацији
                var alternatives = _tourService.GetAlternativeTours(originalTour.Id, requiredSpots);

                if (alternatives.Any())
                {
                    StatusMessage = $"Tura '{originalTour.Name}' je popunjena. Pronađene su {alternatives.Count} alternative na istoj lokaciji:";

                    // Замени тренутну листу са алтернативама
                    Tours.Clear();
                    foreach (var alternative in alternatives)
                    {
                        Tours.Add(TourDTO.FromDomain(alternative));
                    }
                }
                else
                {
                    StatusMessage = $"Tura '{originalTour.Name}' je popunjena i nema dostupnih alternativa na lokaciji {originalTour.LocationText}.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Greška pri traženju alternativnih tura: {ex.Message}";
            }
        }

        private void ExecuteClearSearch()
        {
            SearchCity = "";
            SearchCountry = "";
            SearchLanguage = "srpski";
            SearchDuration = "";
            SearchPeopleCount = "1";

            LoadInitialTours();
        }

        private int? ParseIntValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return int.TryParse(value.Trim(), out int result) && result > 0 ? result : null;
        }

        private double? ParseDoubleValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return double.TryParse(value.Trim(), out double result) && result > 0 ? result : null;
        }

        public void RefreshTours()
        {
            if (IsSearchActive())
            {
                ExecuteSearchTours();
            }
            else
            {
                LoadInitialTours();
            }
        }

        private bool IsSearchActive()
        {
            return !string.IsNullOrWhiteSpace(SearchCity) ||
                   !string.IsNullOrWhiteSpace(SearchCountry) ||
                   (!string.IsNullOrWhiteSpace(SearchLanguage) && SearchLanguage != "srpski") ||
                   !string.IsNullOrWhiteSpace(SearchDuration) ||
                   (!string.IsNullOrWhiteSpace(SearchPeopleCount) && SearchPeopleCount != "1");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Tours?.Clear();
                Languages?.Clear();
            }
            base.Dispose(disposing);
        }
    }
}