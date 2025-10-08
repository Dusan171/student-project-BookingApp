using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BookingApp.Domain.Model;
using BookingApp.Repositories;
using BookingApp.Services;
using BookingApp.Services.DTO;
using BookingApp.Utilities;
using BookingApp.Presentation.View.Guide;
using MvvmHelpers;
using MvvmHelpers.Commands;

namespace BookingApp.Presentation.ViewModel.Guide
{
    public class ComplexTourPartDetailsViewModel : BaseViewModel
    {
        private ComplexTourRequestPart _part;
        private bool _isAccepted;
        private bool _canAccept;
        private string _statusText = string.Empty;
        private string _acceptedByGuide = string.Empty;
        private string _acceptedDate = string.Empty;
        private string _scheduledDate = string.Empty;

        public string Name { get; }
        public string Location { get; }
        public DateTime DateFrom { get; }
        public DateTime DateTo { get; }
        public string Language { get; }
        public int NumberOfParticipants { get; }
        public string Description { get; }
        public string RequestBy { get; }
        
        public bool IsAccepted
        {
            get => _isAccepted;
            private set => SetProperty(ref _isAccepted, value);
        }
        
        public bool CanAccept
        {
            get => _canAccept;
            private set => SetProperty(ref _canAccept, value);
        }
        
        public string StatusText
        {
            get => _statusText;
            private set => SetProperty(ref _statusText, value);
        }
        
        public string AcceptedByGuide
        {
            get => _acceptedByGuide;
            private set => SetProperty(ref _acceptedByGuide, value);
        }
        
        public string AcceptedDate
        {
            get => _acceptedDate;
            private set => SetProperty(ref _acceptedDate, value);
        }
        
        public string ScheduledDate
        {
            get => _scheduledDate;
            private set => SetProperty(ref _scheduledDate, value);
        }

        public ICommand AcceptCommand { get; }
        public event Action<ComplexTourRequest>? TourAccepted;
        public event Action<ComplexTourRequestPart>? PartUpdated; 
        public ObservableCollection<ComplexTourRequestParticipant> Participants { get; }

        public ComplexTourPartDetailsViewModel(ComplexTourRequestPart part)
        {
            _part = part;
            
            // Load participants from repository if they're not already loaded
            if (part.Participants == null || part.Participants.Count == 0)
            {
                var participantRepository = new ComplexTourRequestParticipantRepository();
                part.Participants = participantRepository.GetByPartId(part.Id);
            }
            
            Name = part.City;
            Location = $"{part.City}, {part.Country}";
            DateFrom = part.DateFrom;
            DateTo = part.DateTo;
            Language = part.Language;
            NumberOfParticipants = part.Participants.Count;
            Description = part.Description;
            RequestBy = GetTouristName(part.TouristId);
            Participants = new ObservableCollection<ComplexTourRequestParticipant>(part.Participants);

            AcceptCommand = new RelayCommand(OnAccept);
            
            RefreshStatus();
        }

        private void RefreshStatus()
        {
            IsAccepted = _part.Status == TourRequestStatus.ACCEPTED;
            CanAccept = _part.Status == TourRequestStatus.PENDING;
            
            StatusText = _part.Status switch
            {
                TourRequestStatus.ACCEPTED => "ACCEPTED",
                TourRequestStatus.PENDING => "PENDING",
                TourRequestStatus.INVALID => "INVALID",
                _ => "UNKNOWN"
            };

            if (IsAccepted && _part.AcceptedByGuideId.HasValue)
            {
                AcceptedByGuide = GetGuideName(_part.AcceptedByGuideId.Value);
                AcceptedDate = _part.AcceptedDate?.ToString("dd.MM.yyyy HH:mm") ?? "Unknown";
                ScheduledDate = _part.ScheduledDate?.ToString("dd.MM.yyyy HH:mm") ?? "Not scheduled";
            }
            else
            {
                AcceptedByGuide = string.Empty;
                AcceptedDate = string.Empty;
                ScheduledDate = string.Empty;
            }
        }

        private string GetTouristName(int id)
        {
            UserRepository repo = new UserRepository();
            var user = repo.GetById(id);
            if (user != null)
            {
                return $"{user.FirstName} {user.LastName}";
            }
            return $"Unknown Tourist {id}!";
        }

        private string GetGuideName(int id)
        {
            UserRepository repo = new UserRepository();
            var user = repo.GetById(id);
            if (user != null)
            {
                return $"{user.FirstName} {user.LastName}";
            }
            return $"Unknown Guide {id}";
        }

        private void OnAccept()
        {
            if (!CanAccept)
            {
                MessageBox.Show("This tour part has already been accepted.", 
                    "Already Accepted", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var acceptWindow = new ComplexTourAcceptWindow(_part)
            {
                Owner = Application.Current.MainWindow
            };

            if (acceptWindow.ShowDialog() == true)
            {
                var scheduledTime = acceptWindow.ViewModel.ScheduledDateTime;
                
                try 
                {
                    ComplexTourRequestPartRepository partRepository = new ComplexTourRequestPartRepository();
                    var updatedPart = partRepository.GetById(_part.Id);
                    
                    if (updatedPart != null)
                    {
                        ComplexTourRequestParticipantRepository participantRepository = new ComplexTourRequestParticipantRepository();
                        updatedPart.Participants = participantRepository.GetByPartId(updatedPart.Id);
                        
                        _part = updatedPart;
                        
                        Participants.Clear();
                        foreach (var participant in _part.Participants)
                        {
                            Participants.Add(participant);
                        }
                    }
                    
                    RefreshStatus();
                    
                    PartUpdated?.Invoke(_part);
                    
                    MessageBox.Show($"Complex tour part successfully accepted!\n\nScheduled for: {scheduledTime:dd.MM.yyyy} at {scheduledTime:HH:mm}\nLocation: {Location}\nParticipants: {NumberOfParticipants}",
                        "Tour Part Accepted", MessageBoxButton.OK, MessageBoxImage.Information);

                    ComplexTourRequestRepository complexRepository = new ComplexTourRequestRepository();
                    var complexRequest = complexRepository.GetById(_part.ComplexTourRequestId);

                    ComplexTourRequestPartRepository complexPartRepo = new ComplexTourRequestPartRepository();
                    var allParts = complexPartRepo.GetAll().Where(p => p.ComplexTourRequestId == _part.ComplexTourRequestId).ToList();
                    if (complexRequest != null && allParts.All(p => p.Status == TourRequestStatus.ACCEPTED))
                    {
                        complexRequest.Status = ComplexTourRequestStatus.ACCEPTED;
                        complexRepository.Update(complexRequest);
                    }

                    TourAccepted?.Invoke(complexRequest);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error refreshing tour part data: {ex.Message}", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
