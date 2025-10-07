using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;
using BookingApp.Utilities;
using System.Collections.Generic;

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

        // Mock podaci za opise i vodiče
        private static readonly List<string> MockDescriptions = new List<string>
        {
            "Istražite najlepše delove ovog grada kroz jedinstveno turistićko iskustvo koje će vam omogućiti da upoznate istoriju, kulturu i tradiciju na autentičan način.",
            "Otkrijte skrivene bisere i najčuvenija mesta ovog prelepog grada uz ekspertsko vođenje našeg profesionalnog vodiča.",
            "Uživajte u nezaboravnom putovanju kroz vreme i prostor, gde ćete saznati fascinantne priče i legende ovog mesta.",
            "Pridružite nam se na putovanju koje će vam pružiti dublje razumevanje kulture, arhitekture i načina života lokalnog stanovništva.",
            "Doživite autentičan turistički doživljaj uz stručno vođenje i interesantne anegdote o najznačajnijim znamenitostima.",
            "Prošetajte kroz istoriju i upoznajte se sa bogatim nasleđem kroz oči lokalnih stručnjaka i poznavalaca."
        };

        private static readonly List<string> MockGuides = new List<string>
        {
            "Marko Petrović - Profesionalni turistički vodič",
            "Ana Stojanović - Istoričar umetnosti",
            "Stefan Jovanović - Licencirani vodič",
            "Milica Nikolić - Kulturni antropolog",
            "Aleksandar Mitrović - Lokalni stručnjak",
            "Jovana Pavlović - Turistički vodič sa 10+ godina iskustva"
        };

        private static readonly Random _random = new Random();

        public string SearchCity { get => _searchCity; set => SetProperty(ref _searchCity, value); }
        public string SearchCountry { get => _searchCountry; set => SetProperty(ref _searchCountry, value); }
        public string SearchLanguage { get => _searchLanguage; set => SetProperty(ref _searchLanguage, value); }
        public string SearchDuration { get => _searchDuration; set => SetProperty(ref _searchDuration, value); }
        public string SearchPeopleCount { get => _searchPeopleCount; set => SetProperty(ref _searchPeopleCount, value); }
        public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
        public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

        public ObservableCollection<TourDTO> Tours { get; set; } = new();
        public ObservableCollection<string> Languages { get; set; } = new()
        {
            "srpski", "engleski", "nemački", "francuski", "španski", "italijanski"
        };

        public ICommand SearchToursCommand { get; private set; }
        public ICommand ViewTourDetailsCommand { get; private set; }
        public ICommand ReserveTourCommand { get; private set; }
        public ICommand ClearSearchCommand { get; private set; }

        public event Action<TourDTO> TourReserveRequested;
        public event Action<TourDTO> TourDetailsRequested;

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
            ViewTourDetailsCommand = new RelayCommand<TourDTO>(ExecuteViewTourDetails);
        }

        private void LoadInitialTours()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Učitavanje dostupnih tura...";

                var tours = _tourService.GetAllTours();
                Tours.Clear();

                var reservationService = Services.Injector.CreateInstance<ITourReservationService>();

                foreach (var tour in tours)
                {
                    var tourDto = TourDTO.FromDomain(tour);

                    // Dodajemo mock podatke za opis i vodiča
                    AddMockDataToTour(tourDto);

                    tourDto.RefreshAvailability(reservationService);

                    Tours.Add(tourDto);
                }

                StatusMessage = $"Pronađeno {Tours.Count} dostupnih tura";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Greška pri učitavanju tura: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"LoadInitialTours error: {ex}");
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

                var reservationService = Services.Injector.CreateInstance<ITourReservationService>();

                foreach (var tour in searchResults)
                {
                    var tourDto = TourDTO.FromDomain(tour);

                    // Dodajemo mock podatke za opis i vodiča
                    AddMockDataToTour(tourDto);

                    tourDto.RefreshAvailability(reservationService);

                    Tours.Add(tourDto);
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
                System.Diagnostics.Debug.WriteLine($"ExecuteSearchTours error: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Dodaje mock podatke za opis i vodiča
        private void AddMockDataToTour(TourDTO tour)
        {
            try
            {
                // Ako opis ne postoji ili je prazan, dodaj mock opis
                if (string.IsNullOrWhiteSpace(tour.Description))
                {
                    tour.Description = MockDescriptions[_random.Next(MockDescriptions.Count)];
                }

                // Ako vodič ne postoji ili je prazan, dodaj mock vodiča
                if (string.IsNullOrWhiteSpace(tour.GuideName))
                {
                    tour.GuideName = MockGuides[_random.Next(MockGuides.Count)];
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding mock data: {ex.Message}");
                // Fallback vrednosti
                if (string.IsNullOrWhiteSpace(tour.Description))
                {
                    tour.Description = "Istražite najlepše delove ovog grada kroz jedinstveno turistićko iskustvo.";
                }
                if (string.IsNullOrWhiteSpace(tour.GuideName))
                {
                    tour.GuideName = "Profesionalni turistički vodič";
                }
            }
        }

        private void ExecuteReserveTour(TourDTO tour)
        {
            if (tour == null) return;

            try
            {
                // Proveri da li tura ima dovoljno mesta
                if (!tour.CanReserve)
                {
                    StatusMessage = $"Tura '{tour.Name}' je trenutno popunjena.";
                    ShowAlternativeTours(tour);
                    return;
                }

                var reservationVM = new TourReservationViewModel(tour);

                // Subscribe na događaj kada se rezervacija završi
                reservationVM.ReservationCompleted += () => {
                    RefreshTours();
                };

                TourReserveRequested?.Invoke(tour);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Greška pri rezervaciji: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"ExecuteReserveTour error: {ex}");
            }
        }

        private void ShowAlternativeTours(TourDTO originalTour)
        {
            try
            {
                int requiredSpots = ParseIntValue(SearchPeopleCount) ?? 1;

                var alternatives = _tourService.GetAlternativeTours(originalTour.Id, requiredSpots);

                if (alternatives.Any())
                {
                    StatusMessage = $"Tura '{originalTour.Name}' je popunjena. Pronađene su {alternatives.Count} alternative na istoj lokaciji:";

                    Tours.Clear();
                    var reservationService = Services.Injector.CreateInstance<ITourReservationService>();

                    foreach (var alternative in alternatives)
                    {
                        var tourDto = TourDTO.FromDomain(alternative);
                        AddMockDataToTour(tourDto);
                        tourDto.RefreshAvailability(reservationService);
                        Tours.Add(tourDto);
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
                System.Diagnostics.Debug.WriteLine($"ShowAlternativeTours error: {ex}");
            }
        }

        private void ExecuteViewTourDetails(TourDTO tour)
        {
            if (tour == null)
            {
                StatusMessage = "Greška: Tura nije validna";
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"TourSearchViewModel: Zahtev za detalje ture '{tour.Name}' (ID: {tour.Id})");
                TourDetailsRequested?.Invoke(tour);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Greška pri prikazivanju detalja: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error in ExecuteViewTourDetails: {ex.Message}");
            }
        }

        private void ExecuteClearSearch()
        {
            try
            {
                SearchCity = "";
                SearchCountry = "";
                SearchLanguage = "srpski";
                SearchDuration = "";
                SearchPeopleCount = "1";

                LoadInitialTours();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Greška pri brisanju pretrage: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"ExecuteClearSearch error: {ex}");
            }
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