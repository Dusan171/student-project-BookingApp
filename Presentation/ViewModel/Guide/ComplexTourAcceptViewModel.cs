using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using BookingApp.Domain.Model;
using BookingApp.Repositories;
using BookingApp.Services;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Guide
{
    public class ComplexTourAcceptViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged(string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event EventHandler? RequestClose;

        private readonly ComplexTourRequestPart _part;
        private readonly GuideAvailabilityService _guideAvailabilityService;

        public string Location { get; }
        public string Language { get; }
        public int NumberOfParticipants { get; }
        public DateTime DateFrom { get; }
        public DateTime DateTo { get; }

        public ObservableCollection<string> AvailableDates { get; }
        public ObservableCollection<string> AvailableHours { get; }
        public ObservableCollection<string> AvailableMinutes { get; }

        private string _selectedDate = string.Empty;
        public string SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
                ResetAvailabilityStatus();
            }
        }

        private string _selectedHour = string.Empty;
        public string SelectedHour
        {
            get => _selectedHour;
            set
            {
                _selectedHour = value;
                OnPropertyChanged();
                ResetAvailabilityStatus();
            }
        }

        private string _selectedMinute = string.Empty;
        public string SelectedMinute
        {
            get => _selectedMinute;
            set
            {
                _selectedMinute = value;
                OnPropertyChanged();
                ResetAvailabilityStatus();
            }
        }

        private string _availabilityMessage = string.Empty;
        public string AvailabilityMessage
        {
            get => _availabilityMessage;
            set
            {
                _availabilityMessage = value;
                OnPropertyChanged();
            }
        }

        private bool _isAvailable;
        public bool IsAvailable
        {
            get => _isAvailable;
            set
            {
                _isAvailable = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanConfirm));
            }
        }

        private bool _hasCheckedAvailability;
        public bool HasCheckedAvailability
        {
            get => _hasCheckedAvailability;
            set
            {
                _hasCheckedAvailability = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanConfirm));
            }
        }

        public bool CanConfirm => HasCheckedAvailability && IsAvailable;

        public RelayCommand CheckAvailabilityCommand { get; }
        public RelayCommand ConfirmCommand { get; }
        public RelayCommand CancelCommand { get; }

        public bool DialogResult { get; private set; }
        public DateTime ScheduledDateTime { get; private set; }

        public ComplexTourAcceptViewModel(ComplexTourRequestPart part)
        {
            _part = part ?? throw new ArgumentNullException(nameof(part));
            _guideAvailabilityService = new GuideAvailabilityService();

            Location = $"{part.City}, {part.Country}";
            Language = part.Language;
            NumberOfParticipants = part.Participants.Count;
            DateFrom = part.DateFrom;
            DateTo = part.DateTo;

            AvailableDates = new ObservableCollection<string>();
            AvailableHours = new ObservableCollection<string>();
            AvailableMinutes = new ObservableCollection<string>();

            InitializeAvailableOptions();

            CheckAvailabilityCommand = new RelayCommand(CheckAvailability);
            ConfirmCommand = new RelayCommand(Confirm);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void InitializeAvailableOptions()
        {
            // Populate available dates
            for (var d = DateFrom.Date; d <= DateTo.Date; d = d.AddDays(1))
            {
                AvailableDates.Add(d.ToString("dd.MM.yyyy"));
            }
            SelectedDate = AvailableDates.FirstOrDefault() ?? string.Empty;

            // Populate available hours
            for (int h = 0; h <= 23; h++)
            {
                AvailableHours.Add(h.ToString("D2"));
            }
            SelectedHour = "09"; // Default 09:00

            // Populate available minutes
            for (int m = 0; m < 60; m += 30)
            {
                AvailableMinutes.Add(m.ToString("D2"));
            }
            SelectedMinute = "00"; // Default :00
        }

        private void CheckAvailability()
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedDate) || string.IsNullOrEmpty(SelectedHour) || string.IsNullOrEmpty(SelectedMinute))
                {
                    AvailabilityMessage = "Please select date and time.";
                    IsAvailable = false;
                    HasCheckedAvailability = false;
                    return;
                }

                var selectedDate = DateTime.ParseExact(SelectedDate, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                var selectedHour = int.Parse(SelectedHour);
                var selectedMinute = int.Parse(SelectedMinute);

                var startTime = selectedDate.Date.AddHours(selectedHour).AddMinutes(selectedMinute);
                var endTime = startTime.AddHours(2); // Assume 2-hour duration

                bool isGuideAvailable = _guideAvailabilityService.IsAvailable(Session.CurrentUser.Id, startTime, endTime);
                bool isComplexTourConflictFree = _guideAvailabilityService.IsComplexTourPartConflictFree(_part.ComplexTourRequestId, _part.Id, startTime, endTime);

                HasCheckedAvailability = true;

                if (isGuideAvailable && isComplexTourConflictFree)
                {
                    AvailabilityMessage = "Time slot is available!";
                    IsAvailable = true;
                    ScheduledDateTime = startTime;
                }
                else if (!isGuideAvailable)
                {
                    AvailabilityMessage = "You already have a tour scheduled during this time.\nPlease select a different time slot.";
                    IsAvailable = false;
                }
                else
                {
                    AvailabilityMessage = "Another part of this complex tour is already scheduled during this time.\nPlease select a different time slot.";
                    IsAvailable = false;
                }
            }
            catch (Exception ex)
            {
                AvailabilityMessage = $"Error checking availability: {ex.Message}";
                IsAvailable = false;
                HasCheckedAvailability = false;
            }
        }

        private void ResetAvailabilityStatus()
        {
            AvailabilityMessage = string.Empty;
            IsAvailable = false;
            HasCheckedAvailability = false;
        }

        private void Confirm()
        {
            if (!CanConfirm)
            {
                MessageBox.Show("Please check availability first.", "Cannot Confirm", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Update the complex tour part
                _part.Status = TourRequestStatus.ACCEPTED;
                _part.AcceptedByGuideId = Session.CurrentUser.Id;
                _part.AcceptedDate = DateTime.Now;
                _part.ScheduledDate = ScheduledDateTime;

                // Save changes to repository
                // Note: Assuming these repositories exist based on original code
                ComplexTourRequestPartRepository partRepository = new ComplexTourRequestPartRepository();
                partRepository.Update(_part);

                DialogResult = true;
                RequestClose?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error accepting tour part: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {
            DialogResult = false;
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}