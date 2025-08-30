using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Tourist
{
    public class TourReservationViewModel : ViewModelBase
    {
        private readonly ITourReservationService _reservationService;
        private TourDTO _selectedTour;
        private string _numberOfPeople = "1";
        private string _statusMessage = "";
        private bool _hasStatusMessage;
        private bool _isLoading;

        public string TourName { get; private set; }
        public string LocationText { get; private set; }
        public string DurationText { get; private set; }
        public string Language { get; private set; }
        public string AvailableSpotsText { get; private set; }
        public Brush AvailableSpotsColor { get; private set; }

        public string NumberOfPeople
        {
            get => _numberOfPeople;
            set
            {
                if (SetProperty(ref _numberOfPeople, value))
                    GenerateGuestFields();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (SetProperty(ref _statusMessage, value))
                    HasStatusMessage = !string.IsNullOrEmpty(value);
            }
        }

        public bool HasStatusMessage
        {
            get => _hasStatusMessage;
            set => SetProperty(ref _hasStatusMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public bool HasGuestDetails => GuestDetails.Count > 0;

        public ObservableCollection<ReservationGuestDTO> GuestDetails { get; set; } = new();

        public ObservableCollection<AlternativeTourDTO> AlternativeTours { get; set; } = new();

        public bool HasAlternativeTours => AlternativeTours.Count > 0;

        private bool _showingAlternatives;
        private bool _reservationInProgress;

        public bool ShowingAlternatives
        {
            get => _showingAlternatives;
            set => SetProperty(ref _showingAlternatives, value);
        }

        public bool CanAttemptReservation => !_reservationInProgress && !ShowingAlternatives;

        public ICommand SelectAlternativeCommand { get; private set; }
        public ICommand IncreaseGuestsCommand { get; private set; }
        public ICommand DecreaseGuestsCommand { get; private set; }
        public ICommand ConfirmReservationCommand { get; private set; }
        public ICommand CancelAlternativesCommand { get; private set; }

        public event Action ReservationCompleted;
        public event Action<string> ReservationFailed;

        public TourReservationViewModel(TourDTO tourDto)
        {
            _selectedTour = tourDto ?? throw new ArgumentNullException(nameof(tourDto));
            _reservationService = Services.Injector.CreateInstance<ITourReservationService>();

            AlternativeTours = new ObservableCollection<AlternativeTourDTO>();
            AlternativeTours.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(HasAlternativeTours));
            };

            InitializeProperties();
            RefreshTourData();
            InitializeCommands();
            GenerateGuestFields();
            OnPropertyChanged(nameof(HasAlternativeTours));
            OnPropertyChanged(nameof(HasGuestDetails));
        }

        private void InitializeProperties()
        {
            TourName = _selectedTour.Name;
            LocationText = _selectedTour.LocationText;
            DurationText = _selectedTour.DurationText;
            Language = _selectedTour.Language;
            AvailableSpotsText = _selectedTour.AvailableSpotsText;
            AvailableSpotsColor = _selectedTour.AvailableSpotsColor;
        }

        private void InitializeCommands()
        {
            IncreaseGuestsCommand = new RelayCommand(() =>
            {
                if (int.TryParse(NumberOfPeople, out int count))
                {
                    var maxAllowed = Math.Min(20, _selectedTour.AvailableSpots);
                    if (count < maxAllowed) NumberOfPeople = (count + 1).ToString();
                }
            });

            DecreaseGuestsCommand = new RelayCommand(() =>
            {
                if (int.TryParse(NumberOfPeople, out int count) && count > 1)
                    NumberOfPeople = (count - 1).ToString();
            });

            CancelAlternativesCommand = new RelayCommand(() =>
            {
                ShowingAlternatives = false;
                AlternativeTours.Clear();
                StatusMessage = "";

                OnPropertyChanged(nameof(HasAlternativeTours));
                OnPropertyChanged(nameof(CanAttemptReservation));
            });

            ConfirmReservationCommand = new RelayCommand(ExecuteConfirmReservation);

            SelectAlternativeCommand = new RelayCommand<AlternativeTourDTO>(ExecuteSelectAlternative);
        }

        private void GenerateGuestFields()
        {
            GuestDetails.Clear();

            if (!int.TryParse(NumberOfPeople, out int count) || count <= 0) return;

            for (int i = 0; i < count; i++)
            {
                GuestDetails.Add(new ReservationGuestDTO
                {
                    FirstName = "",
                    LastName = "",
                    Age = 0,
                    Email = i == 0 ? "" : null // Email samo za glavnog kontakta
                });
            }

            OnPropertyChanged(nameof(HasGuestDetails));
        }

        private void ShowAlternativeTours(int requiredSpots)
        {
            ShowingAlternatives = true;

            var alternatives = _reservationService.GetAlternativeToursForLocation(_selectedTour.Id);

            System.Diagnostics.Debug.WriteLine($"Tražim alternative za turu ID {_selectedTour.Id}, potrebno mesta: {requiredSpots}");
            System.Diagnostics.Debug.WriteLine($"Pronađeno {alternatives.Count} alternativa");

            AlternativeTours.Clear();

            var suitableAlternatives = alternatives.Where(alt => alt.AvailableSpots >= requiredSpots).ToList();

            System.Diagnostics.Debug.WriteLine($"Pogodnih alternativa sa dovoljno mesta: {suitableAlternatives.Count}");

            foreach (var alt in suitableAlternatives)
            {
                System.Diagnostics.Debug.WriteLine($"Alternativa: {alt.Name}, Dostupna mesta: {alt.AvailableSpots}");
                AlternativeTours.Add(alt);
            }

            if (suitableAlternatives.Count == 0 && alternatives.Count > 0)
            {
                StatusMessage = $"Nema alternativnih tura sa dovoljno mesta ({requiredSpots}). Evo svih dostupnih tura na istoj lokaciji:";
                foreach (var alt in alternatives.Where(a => a.AvailableSpots > 0))
                {
                    AlternativeTours.Add(alt);
                }
            }
            else if (suitableAlternatives.Count == 0)
            {
                StatusMessage = "Nema dostupnih alternativnih tura na ovoj lokaciji.";
            }

            OnPropertyChanged(nameof(HasAlternativeTours));
            OnPropertyChanged(nameof(CanAttemptReservation));
        }

        // Modifikujte ExecuteSelectAlternative
        private void ExecuteSelectAlternative(AlternativeTourDTO alternative)
        {
            if (alternative == null) return;

            int requiredSpots = int.TryParse(NumberOfPeople, out int count) ? count : 1;

            if (alternative.AvailableSpots < requiredSpots)
            {
                StatusMessage = $"Izabrana alternativa nema dovoljno mesta. Potrebno: {requiredSpots}, dostupno: {alternative.AvailableSpots}";
                return;
            }

            // Kreiraj novi TourDTO iz alternative
            _selectedTour = new TourDTO
            {
                Id = alternative.Id,
                Name = alternative.Name,
                LocationText = alternative.LocationText,
                DurationText = alternative.DurationText,
                Language = alternative.Language,
                AvailableSpots = alternative.AvailableSpots,
                AvailableSpotsText = alternative.AvailableSpotsText,
                AvailableSpotsColor = alternative.AvailableSpotsColor
            };

            // Resetuj stanje
            ShowingAlternatives = false;
            AlternativeTours.Clear();

            InitializeProperties();
            StatusMessage = $"Izabrana alternativa: {alternative.Name}. Možete nastaviti sa rezervacijom.";

            OnPropertyChanged(nameof(HasAlternativeTours));
            OnPropertyChanged(nameof(CanAttemptReservation));
        }
        private void ExecuteConfirmReservation()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "";

                if (!ValidateReservation()) return;

                var peopleCount = int.Parse(NumberOfPeople);
                var currentUser = Session.CurrentUser;
                if (currentUser == null)
                {
                    StatusMessage = "Morate biti ulogovani da biste rezervisali turu.";
                    return;
                }

                // Kreiraj DTO
                var reservationDto = new TourReservationDTO
                {
                    TourId = _selectedTour.Id,
                    TouristId = currentUser.Id,
                    NumberOfGuests = peopleCount,
                    ReservationDate = DateTime.Now,
                    Status = TourReservationStatus.ACTIVE,
                    Guests = GuestDetails.ToList()
                };

                // KLJUČNO: Koristi ValidateReservation iz servisa koji proverava real-time
                if (!_reservationService.ValidateReservation(reservationDto))
                {
                    // Ako validacija ne prođe, prikaži alternative
                    var availableSpots = _reservationService.GetAvailableSpotsForTour(_selectedTour.Id);
                    StatusMessage = $"Nema dovoljno slobodnih mesta. Dostupno je {availableSpots}. Odaberite alternativnu turu:";
                    ShowAlternativeTours(peopleCount);
                    return; // STVARNO prekini izvršavanje
                }

                // Ako je validacija OK, pokušaj kreiranje rezervacije
                var createdReservation = _reservationService.CreateReservation(reservationDto);

                if (createdReservation != null)
                {
                    StatusMessage = "Rezervacija je uspešno kreirana!";

                    // Refresh podataka o trenutnoj turi nakon uspešne rezervacije
                    RefreshTourData();

                    ReservationCompleted?.Invoke();
                }
                else
                {
                    StatusMessage = "Rezervacija nije uspela. Pokušajte ponovo.";
                }
            }
            catch (ArgumentException ex)
            {
                // Ovo se baca iz CreateReservation ako validacija ne prođe
                var peopleCount = int.Parse(NumberOfPeople);
                var availableSpots = _reservationService.GetAvailableSpotsForTour(_selectedTour.Id);
                StatusMessage = $"Rezervacija nije moguća: {ex.Message}. Dostupno mesta: {availableSpots}";
                ShowAlternativeTours(peopleCount);
            }
            catch (Exception ex)
            {
                ReservationFailed?.Invoke($"Greška pri rezervaciji: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        
        private void RefreshTourData()
        {
            try
            {
                var availableSpots = _reservationService.GetAvailableSpotsForTour(_selectedTour.Id);

                _selectedTour.AvailableSpots = availableSpots;
                _selectedTour.AvailableSpotsText = $"Slobodnih mesta: {availableSpots}";
                _selectedTour.AvailableSpotsColor = availableSpots > 0 ? Brushes.Green : Brushes.Red;

                // Ažuriraj UI
                InitializeProperties();

                System.Diagnostics.Debug.WriteLine($"Refreshed tour data: {_selectedTour.Name} has {availableSpots} available spots");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing tour data: {ex.Message}");
            }
        }
        private bool ValidateReservation()
        {
            if (!int.TryParse(NumberOfPeople, out int peopleCount) || peopleCount <= 0)
            {
                StatusMessage = "Broj ljudi mora biti veći od 0.";
                return false;
            }

            if (peopleCount > _selectedTour.AvailableSpots)
            {
                StatusMessage = $"Nema dovoljno slobodnih mesta. Dostupno je {_selectedTour.AvailableSpots}.";
                return false;
            }

            for (int i = 0; i < GuestDetails.Count; i++)
            {
                var guest = GuestDetails[i];
                var guestTitle = i == 0 ? "glavnog kontakta" : $"učesnika {i + 1}";

                if (string.IsNullOrWhiteSpace(guest.FirstName))
                {
                    StatusMessage = $"Ime je obavezno za {guestTitle}.";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(guest.LastName))
                {
                    StatusMessage = $"Prezime je obavezno za {guestTitle}.";
                    return false;
                }

                if (guest.Age <= 0 || guest.Age > 120)
                {
                    StatusMessage = $"Godine moraju biti između 1 i 120 za {guestTitle}.";
                    return false;
                }

                if (i == 0 && string.IsNullOrWhiteSpace(guest.Email))
                {
                    StatusMessage = "Email je obavezan za glavnog kontakta.";
                    return false;
                }
            }

            return true;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GuestDetails?.Clear();
                AlternativeTours?.Clear();
            }

            base.Dispose(disposing);
        }
    }
}
