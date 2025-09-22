using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;
using BookingApp.Utilities;
using System.Windows.Input;
using BookingApp.Domain.Model;
using System.Windows;

namespace BookingApp.Presentation.ViewModel.Tourist
{
    public class TourRequestViewModel : INotifyPropertyChanged
    {
        private readonly ITourRequestService _requestService;
        private readonly int _currentUserId;

        private ObservableCollection<TourRequestDTO> _tourRequests;
        private ObservableCollection<TourRequestParticipantDTO> _participants;
        private TourRequestDTO _selectedRequest;
        private TourRequestDTO _newRequest;
        private TourRequestParticipantDTO _newParticipant;
        private bool _isCreatingRequest;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<TourRequestDTO> TourRequests
        {
            get => _tourRequests;
            set
            {
                _tourRequests = value;
                OnPropertyChanged();
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

        public TourRequestDTO SelectedRequest
        {
            get => _selectedRequest;
            set
            {
                _selectedRequest = value;
                OnPropertyChanged();
                if (_selectedRequest != null)
                {
                    LoadParticipantsForRequest(_selectedRequest.Id);
                }
            }
        }

        public TourRequestDTO NewRequest
        {
            get => _newRequest;
            set
            {
                _newRequest = value;
                OnPropertyChanged();
            }
        }

        public TourRequestParticipantDTO NewParticipant
        {
            get => _newParticipant;
            set
            {
                _newParticipant = value;
                OnPropertyChanged();
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

            TourRequests = new ObservableCollection<TourRequestDTO>();
            Participants = new ObservableCollection<TourRequestParticipantDTO>();

            CreateRequestCommand = new RelayCommand(StartCreateRequest);
            SaveRequestCommand = new RelayCommand(SaveRequest, CanSaveRequest);
            CancelCreateRequestCommand = new RelayCommand(CancelCreateRequest);
            AddParticipantCommand = new RelayCommand(AddParticipant, CanAddParticipant);
            RemoveParticipantCommand = new RelayCommand(RemoveParticipant);
            RefreshCommand = new RelayCommand(RefreshRequests);

            InitializeNewRequest();
            LoadTourRequests();
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
                var requests = _requestService.GetRequestsByTourist(_currentUserId);

                TourRequests.Clear();
                foreach (var request in requests)
                {
                    TourRequests.Add(request);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju zahteva za ture: {ex.Message}",
                               "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadParticipantsForRequest(int requestId)
        {
            try
            {
                var participants = _requestService.GetParticipantsByRequest(requestId);

                Participants.Clear();
                foreach (var participant in participants)
                {
                    Participants.Add(participant);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju učesnika: {ex.Message}",
                               "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                // Validacija
                if (string.IsNullOrWhiteSpace(NewRequest.City))
                {
                    MessageBox.Show("Molimo unesite grad.", "Validacija", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewRequest.Country))
                {
                    MessageBox.Show("Molimo unesite državu.", "Validacija", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewRequest.Language))
                {
                    MessageBox.Show("Molimo unesite jezik.", "Validacija", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (NewRequest.NumberOfPeople <= 0)
                {
                    MessageBox.Show("Broj ljudi mora biti veći od 0.", "Validacija", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (NewRequest.DateFrom <= DateTime.Now.AddDays(2))
                {
                    MessageBox.Show("Datum početka mora biti najmanje 3 dana unapred.", "Validacija", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (NewRequest.DateTo <= NewRequest.DateFrom)
                {
                    MessageBox.Show("Datum završetka mora biti nakon datuma početka.", "Validacija", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Dodeli učesnike
                NewRequest.Participants = Participants.ToList();

                // Sačuvaj zahtev
                var savedRequest = _requestService.CreateRequest(NewRequest);
                TourRequests.Add(savedRequest);

                MessageBox.Show("Zahtev za turu je uspešno kreiran!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);

                IsCreatingRequest = false;
                InitializeNewRequest();
                Participants.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri kreiranju zahteva: {ex.Message}",
                               "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSaveRequest(object parameter)
        {
            return NewRequest != null &&
                   !string.IsNullOrWhiteSpace(NewRequest.City) &&
                   !string.IsNullOrWhiteSpace(NewRequest.Country) &&
                   !string.IsNullOrWhiteSpace(NewRequest.Language) &&
                   NewRequest.NumberOfPeople > 0;
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
                if (string.IsNullOrWhiteSpace(NewParticipant.FirstName) ||
                    string.IsNullOrWhiteSpace(NewParticipant.LastName) ||
                    NewParticipant.Age <= 0)
                {
                    MessageBox.Show("Molimo unesite sva polja za učesnika.", "Validacija",
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Participants.Add(new TourRequestParticipantDTO
                {
                    FirstName = NewParticipant.FirstName,
                    LastName = NewParticipant.LastName,
                    Age = NewParticipant.Age
                });

                // Ažuriraj broj ljudi
                NewRequest.NumberOfPeople = Participants.Count;

                // Resetuj formu za novog učesnika
                NewParticipant = new TourRequestParticipantDTO();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri dodavanju učesnika: {ex.Message}",
                               "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanAddParticipant(object parameter)
        {
            return NewParticipant != null &&
                   !string.IsNullOrWhiteSpace(NewParticipant.FirstName) &&
                   !string.IsNullOrWhiteSpace(NewParticipant.LastName) &&
                   NewParticipant.Age > 0;
        }

        private void RemoveParticipant(object parameter)
        {
            if (parameter is TourRequestParticipantDTO participant)
            {
                try
                {
                    Participants.Remove(participant);

                    // Ažuriraj broj ljudi
                    NewRequest.NumberOfPeople = Participants.Count;

                    // Ako je učesnik već sačuvan, obriši ga iz baze
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
