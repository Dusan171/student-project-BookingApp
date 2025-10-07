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
        private string _statusMessage = "";
        private bool _hasStatusMessage;
        private bool _isLoading;

        public string TourName { get; private set; }
        public string LocationText { get; private set; }
        public string DurationText { get; private set; }
        public string Language { get; private set; }
        public string AvailableSpotsText { get; private set; }
        public Brush AvailableSpotsColor { get; private set; }

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
        public bool HasNoGuests => GuestDetails.Count == 0;

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

        public bool CanAttemptReservation => !_reservationInProgress && !ShowingAlternatives && HasGuestDetails;

        public ICommand AddGuestCommand { get; private set; }
        public ICommand RemoveGuestCommand { get; private set; }
        public ICommand SelectAlternativeCommand { get; private set; }
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

            GuestDetails.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(HasGuestDetails));
                OnPropertyChanged(nameof(HasNoGuests));
                OnPropertyChanged(nameof(CanAttemptReservation));
            };

            InitializeProperties();
            RefreshTourData();
            InitializeCommands();

            // Dodaj glavnog kontakta po defaultu
            AddMainContact();

            OnPropertyChanged(nameof(HasAlternativeTours));
            OnPropertyChanged(nameof(HasGuestDetails));
            OnPropertyChanged(nameof(HasNoGuests));
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
            AddGuestCommand = new RelayCommand(
                execute: _ => ExecuteAddGuest(),
                canExecute: _ => CanAddGuest()
            );

            RemoveGuestCommand = new RelayCommand<ReservationGuestDTO>(ExecuteRemoveGuest);

            CancelAlternativesCommand = new RelayCommand(_ =>
            {
                ShowingAlternatives = false;
                AlternativeTours.Clear();
                StatusMessage = "";
                OnPropertyChanged(nameof(HasAlternativeTours));
                OnPropertyChanged(nameof(CanAttemptReservation));
            });

            ConfirmReservationCommand = new RelayCommand(
                execute: _ => ExecuteConfirmReservation(),
                canExecute: _ => CanConfirmReservation()
            );

            SelectAlternativeCommand = new RelayCommand<AlternativeTourDTO>(ExecuteSelectAlternative);
        }

        private void AddMainContact()
        {
            // Dodaj glavnog kontakta po defaultu
            var mainContact = new ReservationGuestDTO
            {
                FirstName = "",
                LastName = "",
                Age = 0,
                Email = "",
                IsMainContact = true
            };

            GuestDetails.Add(mainContact);
        }

        private bool CanAddGuest()
        {
            // Dozvoliti dodavanje do 20 učesnika bez obzira na dostupnost ture
            // Alternativne ture će se prikazati prilikom rezervacije ako nema mesta
            return GuestDetails.Count < 20 && !IsLoading;
        }

        private void ExecuteAddGuest()
        {
            if (!CanAddGuest()) return;

            var newGuest = new ReservationGuestDTO
            {
                FirstName = "",
                LastName = "",
                Age = 0,
                Email = null,
                IsMainContact = false
            };

            GuestDetails.Add(newGuest);
            StatusMessage = "";
        }

        private void ExecuteRemoveGuest(ReservationGuestDTO? guest)
        {
            if (guest == null || guest.IsMainContact) return;

            GuestDetails.Remove(guest);
            StatusMessage = "";
        }

        private bool CanConfirmReservation()
        {
            return HasGuestDetails && !IsLoading && !ShowingAlternatives;
        }

        private void ShowAlternativeTours(int requiredSpots)
        {
            ShowingAlternatives = true;

            var alternatives = _reservationService.GetAlternativeToursForLocation(_selectedTour.Id);

            AlternativeTours.Clear();

            var suitableAlternatives = alternatives.Where(alt => alt.AvailableSpots >= requiredSpots).ToList();

            foreach (var alt in suitableAlternatives)
            {
                AlternativeTours.Add(alt);
            }

            if (suitableAlternatives.Count == 0 && alternatives.Count > 0)
            {
                foreach (var alt in alternatives.Where(a => a.AvailableSpots > 0))
                {
                    AlternativeTours.Add(alt);
                }
            }
            else if (suitableAlternatives.Count == 0)
            {
                StatusMessage = "Nema dostupnih alternativnih tura na ovoj lokaciji.";
            }
            else
            {
                StatusMessage = $"Odaberite alternativnu turu (potrebno je {requiredSpots} mesta):";
            }

            OnPropertyChanged(nameof(HasAlternativeTours));
            OnPropertyChanged(nameof(CanAttemptReservation));
        }

        private void ExecuteSelectAlternative(AlternativeTourDTO? alternative)
        {
            if (alternative == null) return;

            int requiredSpots = GuestDetails.Count;

            if (alternative.AvailableSpots < requiredSpots)
            {
                StatusMessage = $"Izabrana alternativa nema dovoljno mesta. Potrebno: {requiredSpots}, dostupno: {alternative.AvailableSpots}";
                return;
            }

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

            ShowingAlternatives = false;
            AlternativeTours.Clear();

            InitializeProperties();
            StatusMessage = $"Izabrana alternativa: {alternative.Name}. Možete nastaviti sa rezervacijom.";

            OnPropertyChanged(nameof(TourName));
            OnPropertyChanged(nameof(LocationText));
            OnPropertyChanged(nameof(DurationText));
            OnPropertyChanged(nameof(Language));
            OnPropertyChanged(nameof(AvailableSpotsText));
            OnPropertyChanged(nameof(AvailableSpotsColor));
            OnPropertyChanged(nameof(HasAlternativeTours));
            OnPropertyChanged(nameof(CanAttemptReservation));
        }

        private void ExecuteConfirmReservation()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "";
                _reservationInProgress = true;

                RefreshTourData();

                if (!ValidateReservation())
                {
                    return;
                }

                var peopleCount = GuestDetails.Count;
                var currentUser = Session.CurrentUser;
                if (currentUser == null)
                {
                    StatusMessage = "Morate biti ulogovani da biste rezervisali turu.";
                    return;
                }

                var reservationDto = new TourReservationDTO
                {
                    TourId = _selectedTour.Id,
                    TouristId = currentUser.Id,
                    NumberOfGuests = peopleCount,
                    ReservationDate = DateTime.Now,
                    Status = TourReservationStatus.ACTIVE,
                    Guests = GuestDetails.ToList()
                };

                if (!_reservationService.ValidateReservation(reservationDto))
                {
                    var availableSpots = _reservationService.GetAvailableSpotsForTour(_selectedTour.Id);
                    StatusMessage = $"Nema dovoljno slobodnih mesta. Dostupno je {availableSpots}. Odaberite alternativnu turu:";
                    ShowAlternativeTours(peopleCount);
                    return;
                }

                var createdReservation = _reservationService.CreateReservation(reservationDto);

                if (createdReservation != null)
                {
                    StatusMessage = "Rezervacija je uspešno kreirana!";
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
                var peopleCount = GuestDetails.Count;
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
                _reservationInProgress = false;
                OnPropertyChanged(nameof(CanAttemptReservation));
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

                AvailableSpotsText = _selectedTour.AvailableSpotsText;
                AvailableSpotsColor = _selectedTour.AvailableSpotsColor;

                OnPropertyChanged(nameof(AvailableSpotsText));
                OnPropertyChanged(nameof(AvailableSpotsColor));
            }
            catch (Exception)
            {
                // Silent fail for refresh errors
            }
        }

        private bool ValidateReservation()
        {
            if (GuestDetails.Count == 0)
            {
                StatusMessage = "Morate dodati barem jednog učesnika.";
                return false;
            }

            for (int i = 0; i < GuestDetails.Count; i++)
            {
                var guest = GuestDetails[i];
                var guestTitle = guest.IsMainContact ? "glavnog kontakta" : $"učesnika {i + 1}";

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

                if (guest.IsMainContact && string.IsNullOrWhiteSpace(guest.Email))
                {
                    StatusMessage = $"Email je obavezan za {guestTitle}.";
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