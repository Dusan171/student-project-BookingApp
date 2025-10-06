using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using BookingApp.Presentation.View.Tourist;
using BookingApp.Presentation.ViewModel.Tourist;
using BookingApp.Services;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Tourist
{
    public class ComplexTourRequestViewModel : INotifyPropertyChanged
    {
        private readonly IComplexTourRequestService _requestService;
        private readonly int _currentUserId;

        private ObservableCollection<ComplexTourRequestDTO> _allRequests = new();
        private ObservableCollection<ComplexTourRequestDTO> _filteredRequests = new();
        private ObservableCollection<ComplexTourRequestPartDTO> _currentParts = new();
        private ObservableCollection<ComplexTourRequestParticipantDTO> _currentParticipants = new();

        private ComplexTourRequestPartDTO _newPart;
        private ComplexTourRequestParticipantDTO _newParticipant;
        private ComplexTourRequestDTO _selectedRequest;

        private bool _isCreatingRequest;
        private bool _isLoading;
        private string _statusMessage = "";
        private string _selectedFilter = "Moji";

        // Validation error properties
        private string _cityError = "";
        private string _countryError = "";
        private string _descriptionError = "";
        private string _languageError = "";
        private string _numberOfPeopleError = "";
        private string _dateFromError = "";
        private string _dateToError = "";
        private string _participantFirstNameError = "";
        private string _participantLastNameError = "";
        private string _participantAgeError = "";
        private string _participantCountError = "";
        private string _requestPartsError = "";

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<ComplexTourRequestEventArgs> ShowDetailsRequested;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ComplexTourRequestViewModel(IComplexTourRequestService requestService, int currentUserId)
        {
            _requestService = requestService ?? throw new ArgumentNullException(nameof(requestService));
            _currentUserId = currentUserId;

            CurrentParts = new ObservableCollection<ComplexTourRequestPartDTO>();
            CurrentParticipants = new ObservableCollection<ComplexTourRequestParticipantDTO>();
            CurrentUserId = currentUserId;

            // Inicijalizacija komandi
            FilterCommand = new RelayCommand<string>(ExecuteFilter);
            CreateRequestCommand = new RelayCommand(_ => StartCreateRequest());
            SaveRequestCommand = new RelayCommand(_ => SaveRequest(), _ => CanSaveRequest());
            CancelCreateRequestCommand = new RelayCommand(_ => CancelCreateRequest());
            AddPartCommand = new RelayCommand(_ => AddPart(), _ => CanAddPart());
            RemovePartCommand = new RelayCommand<ComplexTourRequestPartDTO>(RemovePart);
            EditPartCommand = new RelayCommand<ComplexTourRequestPartDTO>(EditPart);
            AddParticipantCommand = new RelayCommand(_ => AddParticipant(), _ => CanAddParticipant());
            RemoveParticipantCommand = new RelayCommand<ComplexTourRequestParticipantDTO>(RemoveParticipant);
            RefreshCommand = new RelayCommand(_ => RefreshRequests());
            ViewDetailsCommand = new RelayCommand<object>(ViewDetails);

            InitializeNewPart();
            _requestService.CheckAndMarkExpiredRequests();
            LoadRequests();
        }

        
        public int CurrentUserId { get; private set; }

        public ObservableCollection<ComplexTourRequestDTO> FilteredRequests
        {
            get => _filteredRequests;
            set
            {
                _filteredRequests = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasNoRequests));
            }
        }

        public ObservableCollection<ComplexTourRequestPartDTO> CurrentParts
        {
            get => _currentParts;
            set
            {
                _currentParts = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasParts));
                OnPropertyChanged(nameof(TotalParts));
                ValidateRequestParts();
            }
        }

        public ObservableCollection<ComplexTourRequestParticipantDTO> CurrentParticipants
        {
            get => _currentParticipants;
            set
            {
                _currentParticipants = value;
                OnPropertyChanged();
                ValidateParticipantCount();
            }
        }

        public ComplexTourRequestPartDTO NewPart
        {
            get => _newPart;
            set
            {
                _newPart = value;
                OnPropertyChanged();
                ValidateNewPart();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ComplexTourRequestParticipantDTO NewParticipant
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
                ValidateNewParticipant();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ComplexTourRequestDTO SelectedRequest
        {
            get => _selectedRequest;
            set
            {
                _selectedRequest = value;
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
        public bool HasParts => CurrentParts.Count > 0;
        public int TotalParts => CurrentParts.Count;

        
        public string CityError
        {
            get => _cityError;
            set
            {
                _cityError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasCityError));
            }
        }

        public string CountryError
        {
            get => _countryError;
            set
            {
                _countryError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasCountryError));
            }
        }

        public string DescriptionError
        {
            get => _descriptionError;
            set
            {
                _descriptionError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasDescriptionError));
            }
        }

        public string LanguageError
        {
            get => _languageError;
            set
            {
                _languageError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasLanguageError));
            }
        }

        public string NumberOfPeopleError
        {
            get => _numberOfPeopleError;
            set
            {
                _numberOfPeopleError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasNumberOfPeopleError));
            }
        }

        public string DateFromError
        {
            get => _dateFromError;
            set
            {
                _dateFromError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasDateFromError));
            }
        }

        public string DateToError
        {
            get => _dateToError;
            set
            {
                _dateToError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasDateToError));
            }
        }

        public string ParticipantFirstNameError
        {
            get => _participantFirstNameError;
            set
            {
                _participantFirstNameError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasParticipantFirstNameError));
            }
        }

        public string ParticipantLastNameError
        {
            get => _participantLastNameError;
            set
            {
                _participantLastNameError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasParticipantLastNameError));
            }
        }

        public string ParticipantAgeError
        {
            get => _participantAgeError;
            set
            {
                _participantAgeError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasParticipantAgeError));
            }
        }

        public string ParticipantCountError
        {
            get => _participantCountError;
            set
            {
                _participantCountError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasParticipantCountError));
            }
        }

        public string RequestPartsError
        {
            get => _requestPartsError;
            set
            {
                _requestPartsError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasRequestPartsError));
            }
        }

        public bool HasCityError => !string.IsNullOrEmpty(CityError);
        public bool HasCountryError => !string.IsNullOrEmpty(CountryError);
        public bool HasDescriptionError => !string.IsNullOrEmpty(DescriptionError);
        public bool HasLanguageError => !string.IsNullOrEmpty(LanguageError);
        public bool HasNumberOfPeopleError => !string.IsNullOrEmpty(NumberOfPeopleError);
        public bool HasDateFromError => !string.IsNullOrEmpty(DateFromError);
        public bool HasDateToError => !string.IsNullOrEmpty(DateToError);
        public bool HasParticipantFirstNameError => !string.IsNullOrEmpty(ParticipantFirstNameError);
        public bool HasParticipantLastNameError => !string.IsNullOrEmpty(ParticipantLastNameError);
        public bool HasParticipantAgeError => !string.IsNullOrEmpty(ParticipantAgeError);
        public bool HasParticipantCountError => !string.IsNullOrEmpty(ParticipantCountError);
        public bool HasRequestPartsError => !string.IsNullOrEmpty(RequestPartsError);

        
        public ICommand FilterCommand { get; }
        public ICommand CreateRequestCommand { get; }
        public ICommand SaveRequestCommand { get; }
        public ICommand CancelCreateRequestCommand { get; }
        public ICommand AddPartCommand { get; }
        public ICommand RemovePartCommand { get; }
        public ICommand EditPartCommand { get; }
        public ICommand AddParticipantCommand { get; }
        public ICommand RemoveParticipantCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ViewDetailsCommand { get; }

       
        private void ExecuteFilter(string filterType)
        {
            SelectedFilter = filterType switch
            {
                "Svi" => "Svi",
                _ => "Moji"
            };
        }

        private void ViewDetails(object selectedRequest)
        {
            if (selectedRequest is ComplexTourRequestDTO request)
            {
                SelectedRequest = request;
                ShowDetailsRequested?.Invoke(this, new ComplexTourRequestEventArgs(request));
            }
        }

        private void StartCreateRequest()
        {
            IsCreatingRequest = true;
            CurrentParts.Clear();
            CurrentParticipants.Clear();
            InitializeNewPart();
            ClearAllErrors();
        }

        private void SaveRequest()
        {
            try
            {
                if (!ValidateComplexRequest())
                    return;

                var requestDTO = new ComplexTourRequestDTO
                {
                    TouristId = _currentUserId,
                    Parts = CurrentParts.ToList()
                };

                for (int i = 0; i < CurrentParts.Count; i++)
                {
                    CurrentParts[i].PartIndex = i + 1;
                }

                var savedRequest = _requestService.CreateRequest(requestDTO);
                _allRequests.Add(savedRequest);

                StatusMessage = "Zahtev za složenu turu je uspešno kreiran!";

                IsCreatingRequest = false;
                CurrentParts.Clear();
                CurrentParticipants.Clear();
                InitializeNewPart();
                ClearAllErrors();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Greška pri kreiranju zahteva: {ex.Message}";
            }
        }

        private bool CanSaveRequest()
        {
            return CurrentParts.Count > 0 && !HasRequestPartsError;
        }

        private void CancelCreateRequest()
        {
            IsCreatingRequest = false;
            CurrentParts.Clear();
            CurrentParticipants.Clear();
            InitializeNewPart();
            ClearAllErrors();
        }

        private void AddPart()
        {
            try
            {
                var partDTO = new ComplexTourRequestPartDTO
                {
                    City = NewPart.City,
                    Country = NewPart.Country,
                    Description = NewPart.Description,
                    Language = NewPart.Language,
                    NumberOfPeople = NewPart.NumberOfPeople,
                    DateFrom = NewPart.DateFrom,
                    DateTo = NewPart.DateTo,
                    PartIndex = CurrentParts.Count + 1,
                    Participants = CurrentParticipants.ToList()
                };

                CurrentParts.Add(partDTO);

                OnPropertyChanged(nameof(HasParts));
                OnPropertyChanged(nameof(TotalParts));

                InitializeNewPart();
                CurrentParticipants.Clear();
                ClearAllErrors();

                StatusMessage = $"Deo ture je uspešno dodat! Ukupno delova: {CurrentParts.Count}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Greška pri dodavanju dela ture: {ex.Message}";
            }
        }

        private bool CanAddPart()
        {
            return NewPart != null &&
                   !HasCityError && !HasCountryError && !HasDescriptionError &&
                   !HasLanguageError && !HasNumberOfPeopleError &&
                   !HasDateFromError && !HasDateToError && !HasParticipantCountError &&
                   !string.IsNullOrWhiteSpace(NewPart.City) &&
                   !string.IsNullOrWhiteSpace(NewPart.Country) &&
                   !string.IsNullOrWhiteSpace(NewPart.Description) &&
                   !string.IsNullOrWhiteSpace(NewPart.Language) &&
                   NewPart.NumberOfPeople > 0 &&
                   NewPart.DateFrom != default(DateTime) &&
                   NewPart.DateTo != default(DateTime) &&
                   CurrentParticipants.Count == NewPart.NumberOfPeople;
        }

        private void RemovePart(ComplexTourRequestPartDTO part)
        {
            if (part != null && CurrentParts.Contains(part))
            {
                CurrentParts.Remove(part);

                for (int i = 0; i < CurrentParts.Count; i++)
                {
                    CurrentParts[i].PartIndex = i + 1;
                }

                OnPropertyChanged(nameof(HasParts));
                OnPropertyChanged(nameof(TotalParts));
                ValidateRequestParts();
            }
        }

        private void EditPart(ComplexTourRequestPartDTO part)
        {
            if (part == null) return;

            NewPart.City = part.City;
            NewPart.Country = part.Country;
            NewPart.Description = part.Description;
            NewPart.Language = part.Language;
            NewPart.NumberOfPeople = part.NumberOfPeople;
            NewPart.DateFrom = part.DateFrom;
            NewPart.DateTo = part.DateTo;

            CurrentParticipants.Clear();
            foreach (var participant in part.Participants)
            {
                CurrentParticipants.Add(new ComplexTourRequestParticipantDTO
                {
                    FirstName = participant.FirstName,
                    LastName = participant.LastName,
                    Age = participant.Age
                });
            }

            RemovePart(part);
            ValidateNewPart();
            ValidateParticipantCount();
        }

        private void AddParticipant()
        {
            try
            {
                var participantDTO = new ComplexTourRequestParticipantDTO
                {
                    FirstName = NewParticipant.FirstName,
                    LastName = NewParticipant.LastName,
                    Age = NewParticipant.Age
                };

                CurrentParticipants.Add(participantDTO);

                NewParticipant = new ComplexTourRequestParticipantDTO();

                OnPropertyChanged(nameof(CurrentParticipants));
                ValidateParticipantCount();
                CommandManager.InvalidateRequerySuggested();

                StatusMessage = "Učesnik je uspešno dodat!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Greška pri dodavanju učesnika: {ex.Message}";
            }
        }

        private bool CanAddParticipant()
        {
            return NewPart?.NumberOfPeople > 0 &&
                   CurrentParticipants.Count < NewPart.NumberOfPeople &&
                   NewParticipant != null &&
                   !HasParticipantFirstNameError && !HasParticipantLastNameError && !HasParticipantAgeError &&
                   !string.IsNullOrWhiteSpace(NewParticipant.FirstName) &&
                   !string.IsNullOrWhiteSpace(NewParticipant.LastName) &&
                   NewParticipant.Age > 0 && NewParticipant.Age <= 120;
        }

        private void RemoveParticipant(ComplexTourRequestParticipantDTO participant)
        {
            if (participant != null && CurrentParticipants.Contains(participant))
            {
                CurrentParticipants.Remove(participant);
                OnPropertyChanged(nameof(CurrentParticipants));
                ValidateParticipantCount();
            }
        }

        private void RefreshRequests()
        {
            LoadRequests();
        }

        public void InitializeNewPart()
        {
            NewPart = new ComplexTourRequestPartDTO
            {
                DateFrom = DateTime.Now.AddDays(3),
                DateTo = DateTime.Now.AddDays(5),
                NumberOfPeople = 1
            };

            NewParticipant = new ComplexTourRequestParticipantDTO();
        }

        private void OnParticipantPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ValidateNewParticipant();
            CommandManager.InvalidateRequerySuggested();
        }

        private void LoadRequests()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Učitavam zahteve...";

                _requestService.CheckAndMarkExpiredRequests();

                _allRequests.Clear();
                var requests = _requestService.GetAllRequests();
                foreach (var request in requests)
                {
                    _allRequests.Add(request);
                }

                ApplyFilter();

                if (FilteredRequests.Count == 0)
                {
                    StatusMessage = SelectedFilter == "Moji"
                        ? "Nemate kreiranih zahteva za složene ture."
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

        private bool ValidateComplexRequest()
        {
            ValidateRequestParts();
            return CurrentParts.Count > 0 && !HasRequestPartsError;
        }

        private void ValidateRequestParts()
        {
            if (CurrentParts.Count == 0)
            {
                RequestPartsError = "Dodajte najmanje jedan deo složene ture.";
            }
            else
            {
                RequestPartsError = "";
            }
        }

        private void ValidateNewPart()
        {
            if (NewPart == null) return;

            
            CityError = string.IsNullOrWhiteSpace(NewPart.City) ? "Molimo unesite grad." : "";

            
            CountryError = string.IsNullOrWhiteSpace(NewPart.Country) ? "Molimo unesite državu." : "";

            
            DescriptionError = string.IsNullOrWhiteSpace(NewPart.Description) ? "Molimo unesite opis." : "";

           
            LanguageError = string.IsNullOrWhiteSpace(NewPart.Language) ? "Molimo izaberite jezik." : "";

           
            NumberOfPeopleError = NewPart.NumberOfPeople <= 0 ? "Broj ljudi mora biti veći od 0." : "";

           
            if (NewPart.DateFrom == default(DateTime))
            {
                DateFromError = "Molimo izaberite datum početka.";
            }
            else if (NewPart.DateFrom <= DateTime.Now.AddDays(2))
            {
                DateFromError = "Datum početka mora biti najmanje 3 dana unapred.";
            }
            else
            {
                DateFromError = "";
            }

           
            if (NewPart.DateTo == default(DateTime))
            {
                DateToError = "Molimo izaberite datum završetka.";
            }
            else if (NewPart.DateTo <= NewPart.DateFrom)
            {
                DateToError = "Datum završetka mora biti nakon datuma početka.";
            }
            else
            {
                DateToError = "";
            }

            ValidateParticipantCount();
        }

        private void ValidateParticipantCount()
        {
            if (NewPart?.NumberOfPeople > 0 && CurrentParticipants.Count != NewPart.NumberOfPeople)
            {
                ParticipantCountError = $"Morate dodati tačno {NewPart.NumberOfPeople} učesnika. Trenutno imate {CurrentParticipants.Count}.";
            }
            else
            {
                ParticipantCountError = "";
            }
        }

        private void ValidateNewParticipant()
        {
            if (NewParticipant == null) return;

            
            if (NewPart?.NumberOfPeople <= 0)
            {
                ParticipantFirstNameError = "Prvo morate uneti broj ljudi za ovaj deo ture.";
                return;
            }

           
            ParticipantFirstNameError = string.IsNullOrWhiteSpace(NewParticipant.FirstName) ? "Molimo unesite ime učesnika." : "";

           
            ParticipantLastNameError = string.IsNullOrWhiteSpace(NewParticipant.LastName) ? "Molimo unesite prezime učesnika." : "";

            
            if (NewParticipant.Age <= 0 || NewParticipant.Age > 120)
            {
                ParticipantAgeError = "Molimo unesite validne godine učesnika (1-120).";
            }
            else
            {
                ParticipantAgeError = "";
            }
        }

        private void ClearAllErrors()
        {
            CityError = "";
            CountryError = "";
            DescriptionError = "";
            LanguageError = "";
            NumberOfPeopleError = "";
            DateFromError = "";
            DateToError = "";
            ParticipantFirstNameError = "";
            ParticipantLastNameError = "";
            ParticipantAgeError = "";
            ParticipantCountError = "";
            RequestPartsError = "";
        }
    }
}