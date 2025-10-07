using BookingApp.Domain.Model;
using BookingApp.Repositories;
using BookingApp.Services;
using BookingApp.Services.DTO;
using BookingApp.Utilities;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Navigation;

namespace BookingApp.Presentation.View.Guide
{
    public class TourRequestDetailsViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Language { get; set; }
        public int NumberOfPeople { get; set; }
        public string Description { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public string RequestedBy { get; set; }
        public List<TourRequestParticipant> Participants { get; set; }
        public event Action<TourRequest>? TourAccepted;

        public ICommand AcceptCommand { get; }
        private TourRequest req;

        public TourRequestDetailsViewModel(TourRequest request)
        {
            City = request.City;
            Country = request.Country;
            Language = request.Language;
            NumberOfPeople = request.NumberOfPeople;
            Description = request.Description;
            req = request;

            DateFrom = request.DateFrom;
            DateTo = request.DateTo;

            RequestedBy = request.TouristName;
            Participants = request.Participants;

            AcceptCommand = new RelayCommand(OnAccept);
        }

        private void OnAccept()
        {
            var guideAvailabilityService = new GuideAvailabilityService();

            var acceptWindow = new Window
            {
                Title = "Accept Tour",
                Width = 320,
                Height = 300,  
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Background = System.Windows.Media.Brushes.White,
                Owner = Application.Current.MainWindow
            };

            var mainPanel = new StackPanel
            {
                Margin = new Thickness(16),
                VerticalAlignment = VerticalAlignment.Center
            };

            var durationPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 12)
            };

            durationPanel.Children.Add(new TextBlock
            {
                Text = "Tour Duration (hours):",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 8, 0)
            });

            var durationCombo = new ComboBox
            {
                Width = 60,
                Margin = new Thickness(0, 0, 0, 0)
            };
            for (int h = 1; h <= 24; h++) 
            {
                durationCombo.Items.Add(h);
            }
            durationCombo.SelectedIndex = 1;
            durationPanel.Children.Add(durationCombo);
            mainPanel.Children.Add(durationPanel);

            mainPanel.Children.Add(new TextBlock
            {
                Text = "Select date and time:",
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                Foreground = System.Windows.Media.Brushes.Black,
                Margin = new Thickness(0, 0, 0, 8)
            });

            var dateCombo = new ComboBox
            {
                Margin = new Thickness(0, 0, 0, 12),
                Padding = new Thickness(4)
            };
            for (var d = DateFrom.Date; d <= DateTo.Date; d = d.AddDays(1))
                dateCombo.Items.Add(d.ToString("dd.MM.yyyy"));
            dateCombo.SelectedIndex = 0;
            mainPanel.Children.Add(dateCombo);

            var timePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 12)
            };

            var hourCombo = new ComboBox { Width = 60, Margin = new Thickness(0, 0, 4, 0), Padding = new Thickness(2) };
            for (int h = 0; h <= 24; h++) hourCombo.Items.Add(h.ToString("D2")); 
            hourCombo.SelectedIndex = 1; 
            timePanel.Children.Add(hourCombo);

            timePanel.Children.Add(new TextBlock { Text = ":", VerticalAlignment = VerticalAlignment.Center });

            var minuteCombo = new ComboBox { Width = 60, Margin = new Thickness(4, 0, 0, 0), Padding = new Thickness(2) };
            for (int m = 0; m < 60; m += 30) minuteCombo.Items.Add(m.ToString("D2"));
            minuteCombo.SelectedIndex = 0;
            timePanel.Children.Add(minuteCombo);

            mainPanel.Children.Add(timePanel);

            var availabilityText = new TextBlock
            {
                Margin = new Thickness(0, 0, 0, 12),
                Foreground = System.Windows.Media.Brushes.Red,
                TextWrapping = TextWrapping.Wrap
            };
            mainPanel.Children.Add(availabilityText);

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 12, 0, 0)
            };

            var confirmButton = new Button
            {
                Content = "Confirm",
                Width = 100,
                Margin = new Thickness(4),
                Background = System.Windows.Media.Brushes.MediumPurple,
                Foreground = System.Windows.Media.Brushes.White,
                Padding = new Thickness(6, 4, 6, 4),
                IsEnabled = false
            };

            var checkButton = new Button
            {
                Content = "Check Availability",
                Width = 120,
                Margin = new Thickness(4),
                Background = System.Windows.Media.Brushes.Green,
                Foreground = System.Windows.Media.Brushes.White,
                Padding = new Thickness(6, 4, 6, 4)
            };

            var cancelButton = new Button
            {
                Content = "Cancel",
                Width = 100,
                Margin = new Thickness(4),
                Background = System.Windows.Media.Brushes.Gray,
                Foreground = System.Windows.Media.Brushes.White,
                Padding = new Thickness(6, 4, 6, 4)
            };

            buttonPanel.Children.Add(checkButton);
            buttonPanel.Children.Add(confirmButton);
            buttonPanel.Children.Add(cancelButton);
            mainPanel.Children.Add(buttonPanel);

            acceptWindow.Content = mainPanel;

            void ResetAvailabilityStatus()
            {
                availabilityText.Text = "";
                confirmButton.IsEnabled = false;
            }

            durationCombo.SelectionChanged += (_, __) => ResetAvailabilityStatus();
            dateCombo.SelectionChanged += (_, __) => ResetAvailabilityStatus();
            hourCombo.SelectionChanged += (_, __) => ResetAvailabilityStatus();
            minuteCombo.SelectionChanged += (_, __) => ResetAvailabilityStatus();

            checkButton.Click += (_, __) =>
            {
                var selectedDateString = dateCombo.SelectedItem.ToString();
                var selectedDate = DateTime.ParseExact(selectedDateString, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                var selectedHour = int.Parse(hourCombo.SelectedItem.ToString());
                var selectedMinute = int.Parse(minuteCombo.SelectedItem.ToString());
                var duration = (int)durationCombo.SelectedItem;

                var startTime = selectedDate.Date.AddHours(selectedHour).AddMinutes(selectedMinute);
                MessageBox.Show($"Checking availability for {startTime:dd.MM.yyyy HH:mm} (Duration: {duration} hours)",
                    "Checking Availability", MessageBoxButton.OK, MessageBoxImage.Information);
                var endTime = startTime.AddHours(duration);

                if (guideAvailabilityService.IsAvailable(Session.CurrentUser.Id, startTime, endTime))
                {
                    availabilityText.Text = "✓ Time slot is available!";
                    availabilityText.Foreground = System.Windows.Media.Brushes.Green;
                    confirmButton.IsEnabled = true;
                }
                else
                {
                    availabilityText.Text = "✗ You already have a tour scheduled during this time.";
                    availabilityText.Foreground = System.Windows.Media.Brushes.Red;
                    confirmButton.IsEnabled = false;
                }
            };

            confirmButton.Click += (_, __) =>
            {
                var selectedDateString = dateCombo.SelectedItem.ToString();
                var selectedDate = DateTime.ParseExact(selectedDateString, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                var selectedHour = int.Parse(hourCombo.SelectedItem.ToString());
                var selectedMinute = int.Parse(minuteCombo.SelectedItem.ToString());
                var duration = (int)durationCombo.SelectedItem;

                var scheduledTime = selectedDate.Date.AddHours(selectedHour).AddMinutes(selectedMinute);

                var detailsWindow = new TourDetailsInputWindow(req.NumberOfPeople + 5);
                detailsWindow.Owner = acceptWindow;
                
                if (detailsWindow.ShowDialog() == true)
                {
                    var viewModel = detailsWindow.ViewModel;
                    
                    var keyPoints = viewModel.KeyPoints.ToList();
                    var images = viewModel.Images.ToList();
                    var maxTourists = int.Parse(viewModel.MaxTourists);

                    //oznacim zahtev kao prihvacen i popunim informacije
                    req.Status = TourRequestStatus.ACCEPTED;
                    req.AcceptedByGuideId = Session.CurrentUser.Id;
                    req.AcceptedDate = DateTime.Now;
                    req.ScheduledDate = scheduledTime;
                    TourRequestRepository tourRequestRepository = new TourRequestRepository();
                    tourRequestRepository.Update(req);

                    //napravim turu na osnovu zahteva
                    string tourName = "Tour of " + req.City + ", " + req.Country;
                    UserRepository repository = new UserRepository();
                    var newTour = CreateTourBasedOnRequest(tourName, Session.CurrentUser.Id, req.City, req.Country, req.Description, req.Language, maxTourists,
                        req.NumberOfPeople, keyPoints, scheduledTime, duration, images, TourStatus.NONE, repository.GetById(Session.CurrentUser.Id));

                    //posaljem notifikaciju turisti (sa datumom i vremenom)
                    var tourNotificationService = new TourNotificationService(
                        new TourNotificationRepository(),
                        new TourRequestRepository(),
                        new TourRepository(),
                        new UserRepository()
                    );
                    
                    var guide = repository.GetById(Session.CurrentUser.Id);
                    string guideName = guide != null ? $"{guide.FirstName} {guide.LastName}" : "Unknown Guide";
                    string location = $"{req.City}, {req.Country}";
                    
                    tourNotificationService.SendTourAcceptanceNotification(
                        req.TouristId, 
                        newTour.Id, 
                        tourName, 
                        location, 
                        scheduledTime, 
                        guideName
                    );

                    //napravim rezervaciju 
                    CreateAutomaticReservation(newTour, req);

                    MessageBox.Show($"Successfully accepted the tour on {scheduledTime:dd.MM.yyyy HH:mm}",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    acceptWindow.Close();
                    TourAccepted?.Invoke(req);
                }
            };

            cancelButton.Click += (_, __) => acceptWindow.Close();

            acceptWindow.ShowDialog();


        }

        private void CreateAutomaticReservation(Tour newTour, TourRequest request)
        {
            try
            {
                var tourReservationService = new TourReservationService(
                    new TourReservationRepository(),
                    new TourRepository(),
                    new StartTourTimeRepository(),
                    new UserRepository(),
                    new TourReviewService(
                        new TourReviewRepository(),
                        new TourRepository(),
                        new UserRepository()
                    ),
                    new ReservationGuestRepository()
                );

                var guestDTOs = new List<ReservationGuestDTO>();
                
                var mainTourist = new UserRepository().GetById(request.TouristId);
                if (mainTourist != null)
                {
                    var mainGuestDTO = new ReservationGuestDTO
                    {
                        FirstName = mainTourist.FirstName ?? "Tourist",
                        LastName = mainTourist.LastName ?? $"#{request.TouristId}",
                        Age = 21, 
                        Email = "", 
                        HasAppeared = false,
                        KeyPointJoinedAt = -1,
                        IsMainContact = true
                    };
                    guestDTOs.Add(mainGuestDTO);
                }

                foreach (var participant in request.Participants)
                {
                    var guestDTO = new ReservationGuestDTO
                    {
                        FirstName = participant.FirstName,
                        LastName = participant.LastName,
                        Age = participant.Age,
                        Email = participant.Email,
                        HasAppeared = false,
                        KeyPointJoinedAt = -1,
                        IsMainContact = false
                    };
                    guestDTOs.Add(guestDTO);
                }

                var reservationDTO = new TourReservationDTO
                {
                    TourId = newTour.Id,
                    StartTourTimeId = newTour.StartTimes.First().Id,
                    TouristId = request.TouristId,
                    NumberOfGuests = guestDTOs.Count, 
                    ReservationDate = DateTime.Now,
                    Status = TourReservationStatus.ACTIVE,
                    CurrentKeyPointIndex = 0,
                    Guests = guestDTOs
                };

                var createdReservation = tourReservationService.CreateReservation(reservationDTO);
                
                MessageBox.Show($"Reservation automatically created!\nReservation ID: {createdReservation.Id}\nGuests: {createdReservation.NumberOfGuests}", 
                    "Reservation Created", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating automatic reservation: {ex.Message}", 
                    "Reservation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private Tour CreateTourBasedOnRequest(string Name, int guideId, string city, string country, string desc, string language, int maxTourists, 
            int reservedSpots, List<KeyPoint> keypoints, DateTime dateTime, int duration, List<Images> images, TourStatus status, User guide)
        {
            //keypoints
            KeyPointRepository keyPointRepository = new KeyPointRepository();
            foreach (var kp in keypoints)
            {
                keyPointRepository.Save(kp);
            }
            //location
            Location location = new Location
            {
                City = city.Trim(),
                Country = country.Trim()
            };
            LocationRepository _locationRepository = new LocationRepository();
            _locationRepository.Save(location);
            //starttimes
            StartTourTime startTourTime = new StartTourTime
            {
                Time = dateTime
            };
            StartTourTimeRepository _startTimeRepository = new StartTourTimeRepository();
            _startTimeRepository.Add(startTourTime);
            List<StartTourTime> StartTimes = new List<StartTourTime> { startTourTime };
            //images
            ImageRepository _imageRepository = new ImageRepository();
            foreach (var img in images)
            {
                _imageRepository.Save(img);
            }

            var newTour = new Tour
            {
                GuideId = guideId,
                Name = Name.Trim(),
                Location = location,
                Description = desc?.Trim(),
                Language = language?.Trim(),
                MaxTourists = maxTourists,
                DurationHours = duration,
                StartTimes = StartTimes,
                KeyPoints = keypoints,
                Images = images
            };
            TourRepository _tourRepository = new TourRepository();
            _tourRepository.Save(newTour);
            foreach (var st in newTour.StartTimes)
            {
                st.TourId = newTour.Id;
                _startTimeRepository.Update(st);
            }

            MessageBox.Show("Tour based on request created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            
            return newTour;
        }

    }
}
