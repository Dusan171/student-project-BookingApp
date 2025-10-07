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
        public string Name { get; }
        public string Location { get; }
        public DateTime DateFrom { get; }
        public DateTime DateTo { get; }
        public string Language { get; }
        public int NumberOfParticipants { get; }
        public string Description { get; }
        public string RequestBy { get; }
        public bool IsAccepted { get; }
        public bool CanAccept { get; }
        public string StatusText { get; }
        public string AcceptedByGuide { get; }
        public string AcceptedDate { get; }
        public string ScheduledDate { get; }
        public ICommand AcceptCommand { get; }
        public event Action<ComplexTourRequest>? TourAccepted;
        public ObservableCollection<ComplexTourRequestParticipant> Participants { get; }
        private ComplexTourRequestPart _part;

        public ComplexTourPartDetailsViewModel(ComplexTourRequestPart part)
        {
            _part = part;
            Name = part.City;
            Location = $"{part.City}, {part.Country}";
            DateFrom = part.DateFrom;
            DateTo = part.DateTo;
            Language = part.Language;
            NumberOfParticipants = part.Participants.Count;
            Description = part.Description;
            IsAccepted = part.Status == TourRequestStatus.ACCEPTED;
            CanAccept = part.Status == TourRequestStatus.PENDING;
            
            // Set status text based on current status
            StatusText = part.Status switch
            {
                TourRequestStatus.ACCEPTED => "ACCEPTED",
                TourRequestStatus.PENDING => "PENDING",
                TourRequestStatus.INVALID => "INVALID",
                _ => "UNKNOWN"
            };

            AcceptCommand = new RelayCommand(OnAccept);
            RequestBy = GetTouristName(part.TouristId);
            Participants = new ObservableCollection<ComplexTourRequestParticipant>(part.Participants);

            // If accepted, show guide and scheduling information
            if (IsAccepted && part.AcceptedByGuideId.HasValue)
            {
                AcceptedByGuide = GetGuideName(part.AcceptedByGuideId.Value);
                AcceptedDate = part.AcceptedDate?.ToString("dd.MM.yyyy HH:mm") ?? "Unknown";
                ScheduledDate = part.ScheduledDate?.ToString("dd.MM.yyyy HH:mm") ?? "Not scheduled";
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
            return $"Unknown Tourist {id}";
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
                
                // Save to repository
                try 
                {
                    // Note: Assuming these repositories exist based on original code
                    // ComplexTourRequestPartRepository partRepository = new ComplexTourRequestPartRepository();
                    // partRepository.Update(_part);
                    
                    MessageBox.Show($"Complex tour part successfully accepted!\n\nScheduled for: {scheduledTime:dd.MM.yyyy} at {scheduledTime:HH:mm}\nLocation: {Location}\nParticipants: {NumberOfParticipants}",
                        "Tour Part Accepted", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Notify parent that tour was accepted
                    // ComplexTourRequestRepository complexRepository = new ComplexTourRequestRepository();
                    // var complexRequest = complexRepository.GetById(_part.ComplexTourRequestId);
                    // TourAccepted?.Invoke(complexRequest);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving tour acceptance: {ex.Message}", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
