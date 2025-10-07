using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Tourist
{
    public class TourRequestViewModel : INotifyPropertyChanged
    {
        private readonly ITourRequestService _requestService;
        private readonly int _currentUserId;

        private ObservableCollection<TourRequestDTO> _allRequests = new();
        private ObservableCollection<TourRequestDTO> _filteredRequests = new();
        private ObservableCollection<TourRequestParticipantDTO> _participants;
        private TourRequestDTO _newRequest;
        private TourRequestParticipantDTO _newParticipant;
        private bool _isCreatingRequest;
        private bool _isLoading;
        private string _statusMessage = "";
        private string _selectedFilter = "Moji";

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<TourRequestDTO> FilteredRequests
        {
            get => _filteredRequests;
            set
            {
                _filteredRequests = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasNoRequests));
            }
        }

        public ObservableCollection<TourRequestParticipantDTO> Participants
        {
            get => _participants;
            set
            {
                _participants = value;
                OnPropertyChanged();
            }
        }

        public TourRequestDTO NewRequest
        {
            get => _newRequest;
            set
            {
                _newRequest = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public TourRequestParticipantDTO NewParticipant
        {
            get => _newParticipant;
            set
            {
                if (_newParticipant != null)
                {
                    _newParticipant.PropertyChanged -= OnParticipantPropertyChanged;
                }

                _newParticipant = value;

                if (_newParticipant != null)
                {
                    _newParticipant.PropertyChanged += OnParticipantPropertyChanged;
                }

                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsCreatingRequest
        {
            get => _isCreatingRequest;
            set
            {
                _isCreatingRequest = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        public bool HasNoRequests => FilteredRequests.Count == 0 && !IsLoading;

        public ICommand FilterCommand { get; }
        public ICommand CreateRequestCommand { get; }
        public ICommand SaveRequestCommand { get; }
        public ICommand CancelCreateRequestCommand { get; }
        public ICommand AddParticipantCommand { get; }
        public ICommand RemoveParticipantCommand { get; }
        public ICommand RefreshCommand { get; }

        public TourRequestViewModel(ITourRequestService requestService, int currentUserId)
        {
            _requestService = requestService ?? throw new ArgumentNullException(nameof(requestService));
            _currentUserId = currentUserId;

            Participants = new ObservableCollection<TourRequestParticipantDTO>();

            FilterCommand = new RelayCommand<string>(ExecuteFilter);
            CreateRequestCommand = new RelayCommand(StartCreateRequest);
            SaveRequestCommand = new RelayCommand(SaveRequest); 
            CancelCreateRequestCommand = new RelayCommand(CancelCreateRequest);
            AddParticipantCommand = new RelayCommand(AddParticipant); 
            RemoveParticipantCommand = new RelayCommand(RemoveParticipant);
            RefreshCommand = new RelayCommand(RefreshRequests);

            InitializeNewRequest();
            LoadTourRequests();
        }

        private void OnParticipantPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }

        private void ExecuteFilter(string? filterType)
        {
            SelectedFilter = filterType switch
            {
                "Svi" => "Svi",
                _ => "Moji"
            };
        }

        private void InitializeNewRequest()
        {
            NewRequest = new TourRequestDTO
            {
                TouristId = _currentUserId,
                DateFrom = DateTime.Now.AddDays(3),
                DateTo = DateTime.Now.AddDays(10),
                NumberOfPeople = 1,
                Participants = new System.Collections.Generic.List<TourRequestParticipantDTO>()
            };

            NewParticipant = new TourRequestParticipantDTO();
        }

        private void LoadTourRequests()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Učitavam zahteve...";

                _allRequests.Clear();

                
                var allRequests = _requestService.GetAllRequests();
                foreach (var request in allRequests)
                {
                    _allRequests.Add(request);
                }

                
                ApplyFilter();

                if (FilteredRequests.Count == 0)
                {
                    StatusMessage = SelectedFilter == "Moji"
                        ? "Nemate kreiranih zahteva za ture."
                        : "Nema dostupnih zahteva.";
                }
                else
                {
                    StatusMessage = "";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Greška pri učitavanju zahteva: {ex.Message}";
                MessageBox.Show($"Greška pri učitavanju zahteva za ture: {ex.Message}",
                               "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
                OnPropertyChanged(nameof(HasNoRequests));
            }
        }

        private void ApplyFilter()
        {
            FilteredRequests.Clear();

            var filtered = SelectedFilter == "Moji"
                ? _allRequests.Where(r => r.TouristId == _currentUserId)
                : _allRequests;

            foreach (var request in filtered.OrderByDescending(r => r.CreatedAt))
            {
                FilteredRequests.Add(request);
            }

            OnPropertyChanged(nameof(HasNoRequests));
        }

        private void StartCreateRequest(object parameter)
        {
            IsCreatingRequest = true;
            InitializeNewRequest();
            Participants.Clear();
        }

        private void SaveRequest(object parameter)
        {
            try
            {
               
                if (string.IsNullOrWhiteSpace(NewRequest?.City))
                {
                    MessageBox.Show("Molimo unesite grad.", "Validacija",
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                
                if (string.IsNullOrWhiteSpace(NewRequest?.Country))
                {
                    MessageBox.Show("Molimo unesite državu.", "Validacija",
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                
                if (string.IsNullOrWhiteSpace(NewRequest?.Description))
                {
                    MessageBox.Show("Molimo unesite opis.", "Validacija",
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewRequest?.Language))
                {
                    MessageBox.Show("Molimo izaberite jezik.", "Validacija",
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                
                if (NewRequest?.DateFrom == default(DateTime))
                {
                    MessageBox.Show("Molimo izaberite datum početka.", "Validacija",
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                
                if (NewRequest?.DateTo == default(DateTime))
                {
                    MessageBox.Show("Molimo izaberite datum završetka.", "Validacija",
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

               
                if (NewRequest.DateFrom <= DateTime.Now.AddDays(2))
                {
                    MessageBox.Show("Datum početka mora biti najmanje 3 dana unapred.", "Validacija",
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                
                if (NewRequest.DateTo <= NewRequest.DateFrom)
                {
                    MessageBox.Show("Datum završetka mora biti nakon datuma početka.", "Validacija",
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                
                if (Participants.Count == 0)
                {
                    MessageBox.Show("Dodajte najmanje jednog učesnika.", "Validacija",
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

               
                NewRequest.Participants = Participants.ToList();
                NewRequest.NumberOfPeople = Participants.Count;

                var savedRequest = _requestService.CreateRequest(NewRequest);
                _allRequests.Add(savedRequest);

                MessageBox.Show("Zahtev za turu je uspešno kreiran!", "Uspeh",
                               MessageBoxButton.OK, MessageBoxImage.Information);

                IsCreatingRequest = false;
                InitializeNewRequest();
                Participants.Clear();

                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri kreiranju zahteva: {ex.Message}",
                               "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelCreateRequest(object parameter)
        {
            IsCreatingRequest = false;
            InitializeNewRequest();
            Participants.Clear();
        }

        private void AddParticipant(object parameter)
        {
            try
            {
               
                if (string.IsNullOrWhiteSpace(NewParticipant?.FirstName))
                {
                    MessageBox.Show("Molimo unesite ime učesnika.", "Validacija",
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                
                if (string.IsNullOrWhiteSpace(NewParticipant?.LastName))
                {
                    MessageBox.Show("Molimo unesite prezime učesnika.", "Validacija",
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                
                if (NewParticipant?.Age <= 0 || NewParticipant?.Age > 120)
                {
                    MessageBox.Show("Molimo unesite validne godine učesnika (1-120).", "Validacija",
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Participants.Add(new TourRequestParticipantDTO
                {
                    FirstName = NewParticipant.FirstName,
                    LastName = NewParticipant.LastName,
                    Age = NewParticipant.Age
                });

               
                NewParticipant = new TourRequestParticipantDTO();

                MessageBox.Show("Učesnik je uspešno dodat!", "Uspeh",
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri dodavanju učesnika: {ex.Message}",
                               "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveParticipant(object parameter)
        {
            if (parameter is TourRequestParticipantDTO participant)
            {
                try
                {
                    Participants.Remove(participant);

                   
                    if (participant.Id > 0)
                    {
                        _requestService.RemoveParticipant(participant.Id);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Greška pri brisanju učesnika: {ex.Message}",
                                   "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshRequests(object parameter)
        {
            LoadTourRequests();
        }
    }
}