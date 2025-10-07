using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Tourist
{
    public class MyReservationsViewModel : ViewModelBase
    {
        private readonly ITourReservationService _reservationService;
        private ObservableCollection<TourReservationDTO> _allReservations = new();
        private ObservableCollection<TourReservationDTO> _filteredReservations = new();
        private string _selectedStatusFilter = "Sve";
        private bool _isLoading = false;
        private string _statusMessage = "";

       
        private static readonly List<string> MockGuides = new List<string>
        {
            "Marko Petrović",
            "Ana Stojanović",
            "Stefan Jovanović",
            "Milica Nikolić",
            "Aleksandar Mitrović",
            "Jovana Pavlović",
            "Nikola Stanković",
            "Tamara Đorđević",
            "Milan Radović",
            "Jelena Milosavljević"
        };

        private static readonly Random _random = new Random();

        public event EventHandler<TourReservationDTO>? ReviewRequested;

        public ObservableCollection<TourReservationDTO> FilteredReservations
        {
            get => _filteredReservations;
            set => SetProperty(ref _filteredReservations, value);
        }

        public string SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set
            {
                if (SetProperty(ref _selectedStatusFilter, value))
                {
                    ApplyFilter();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool HasReservations => FilteredReservations.Count > 0;
        public bool HasNoReservations => !HasReservations && !IsLoading;

        public ICommand FilterCommand { get; private set; } = null!;
        public ICommand RefreshCommand { get; private set; } = null!;
        public ICommand ReviewTourCommand { get; private set; } = null!;

        public MyReservationsViewModel() : this(null) { }

        public MyReservationsViewModel(ITourReservationService? reservationService)
        {
            _reservationService = reservationService ?? Services.Injector.CreateInstance<ITourReservationService>();
            InitializeCommands();
            LoadReservations();
        }

        private void InitializeCommands()
        {
            FilterCommand = new RelayCommand<string>(ExecuteFilter);
            RefreshCommand = new RelayCommand(ExecuteRefresh);
            ReviewTourCommand = new RelayCommand<TourReservationDTO>(ExecuteReviewTour, CanReviewTour);
        }

        private void ExecuteFilter(string? filterType)
        {
            SelectedStatusFilter = filterType switch
            {
                "Aktivne" => "Aktivne",
                "Završene" => "Završene",
                _ => "Sve"
            };
        }

        private void ExecuteRefresh()
        {
            LoadReservations();
        }

        private void LoadReservations()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Učitavam rezervacije...";

                var currentUser = Session.CurrentUser;
                if (currentUser == null)
                {
                    StatusMessage = "Morate biti ulogovani da biste videli rezervacije.";
                    return;
                }

                var reservations = _reservationService.GetReservationsByTourist(currentUser.Id);

                _allReservations.Clear();
                foreach (var reservation in reservations.OrderByDescending(r => r.ReservationDate))
                {
                    // Uklanjamo otkazane rezervacije iz prikaza
                    if (reservation.Status != TourReservationStatus.CANCELLED)
                    {
                        // Dodaj mock podatke za vodiča ako ne postoji
                        AddMockGuideData(reservation);
                        _allReservations.Add(reservation);
                    }
                }

                ApplyFilter();

                if (!HasReservations)
                {
                    StatusMessage = "Nemate nijednu rezervaciju.";
                }
                else
                {
                    StatusMessage = "";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Greška pri učitavanju rezervacija: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                OnPropertyChanged(nameof(HasReservations));
                OnPropertyChanged(nameof(HasNoReservations));
            }
        }

        private void AddMockGuideData(TourReservationDTO reservation)
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(reservation.GuideName))
                {
                    reservation.GuideName = MockGuides[_random.Next(MockGuides.Count)];
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding mock guide data: {ex.Message}");
               
                if (string.IsNullOrWhiteSpace(reservation.GuideName))
                {
                    reservation.GuideName = "Profesionalni vodič";
                }
            }
        }

        private void ApplyFilter()
        {
            FilteredReservations.Clear();

            var filtered = SelectedStatusFilter switch
            {
                "Aktivne" => _allReservations.Where(r => r.Status == TourReservationStatus.ACTIVE),
                "Završene" => _allReservations.Where(r => r.Status == TourReservationStatus.COMPLETED),
                _ => _allReservations
            };

            foreach (var reservation in filtered)
            {
                FilteredReservations.Add(reservation);
            }

            // Ažuriraj status message u zavisnosti od filtera
            if (FilteredReservations.Count == 0 && !IsLoading)
            {
                StatusMessage = SelectedStatusFilter switch
                {
                    "Aktivne" => "Nemate aktivnih rezervacija.",
                    "Završene" => "Nemate završenih rezervacija.",
                    _ => "Nemate nijednu rezervaciju."
                };
            }
            else
            {
                StatusMessage = "";
            }

            OnPropertyChanged(nameof(HasReservations));
            OnPropertyChanged(nameof(HasNoReservations));
        }

        private void ExecuteReviewTour(TourReservationDTO? reservation)
        {
            try
            {
                if (reservation != null)
                {
                    ReviewRequested?.Invoke(this, reservation);
                    StatusMessage = $"Otvaranje forme za ocenjivanje ture: {reservation.TourName}";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Greška pri otvaranju forme za ocenjivanje: {ex.Message}";
            }
        }

        private bool CanReviewTour(TourReservationDTO? reservation)
        {
            return reservation != null && reservation.Status == TourReservationStatus.COMPLETED;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _allReservations?.Clear();
                FilteredReservations?.Clear();
            }
            base.Dispose(disposing);
        }
    }
}