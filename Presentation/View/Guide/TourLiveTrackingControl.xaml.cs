using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BookingApp.Domain;
using BookingApp.Repositories;
using BookingApp.Domain.Model;
using BookingApp.Services;
using BookingApp.Domain.Interfaces;

namespace BookingApp.Presentation.View.Guide
{
    public partial class TourLiveTrackingControl : UserControl
    {
        private Tour currentTour;
        private List<TourReservation> reservations;
        private TourRepository tourRepository = new TourRepository();
        private List<TouristAttendance> attendanceList = new List<TouristAttendance>();
        private TouristAttendanceRepository attendanceRepo = new TouristAttendanceRepository();
        private ReservationGuestRepository guestsRepository = new ReservationGuestRepository();
        private DateTime startTime;
        MainPage mainPage;

        private readonly TourPresenceService _tourPresenceService;
        private readonly ITouristAttendanceService _touristAttendanceService;

        public TourLiveTrackingControl(Tour tour, DateTime start, MainPage main,
                                     TourPresenceService tourPresenceService = null,
                                     ITouristAttendanceService touristAttendanceService = null)
        {
            InitializeComponent();
            mainPage = main;
            currentTour = tour ?? throw new ArgumentNullException(nameof(tour));

            _tourPresenceService = tourPresenceService;
            _touristAttendanceService = touristAttendanceService;
            currentTour.Status = TourStatus.ACTIVE;
            tourRepository.Update(currentTour);
            LoadReservations();
            startTime = start;
            LoadTourDetails(startTime);
            LoadKeyPoints();
        }

        private void LoadTourDetails(DateTime startTime)
        {
            if (currentTour == null)
            {
                MessageBox.Show("Tour is null!");
                return;
            }

            if (currentTour.StartTimes == null || !currentTour.StartTimes.Any())
            {
                MessageBox.Show("No start times for this tour!");
                return;
            }

            var tourStart = currentTour.StartTimes.FirstOrDefault(st => st.Time == startTime);
            if (tourStart == null)
            {
                MessageBox.Show("Start time not found!");
                return;
            }
            TourNameTextBlock.Text = currentTour.Name;
            TourDateTimeTextBlock.Text = $"Date & Time: {startTime:yyyy-MM-dd HH:mm}";
            TourLanguageTextBlock.Text = $"Language: {currentTour.Language}";
            if (currentTour?.Location != null)
            {
                TourLocationTextBlock.Text = $"Location: {currentTour.Location.City}, {currentTour.Location.Country}";
            }
            TourDurationTextBlock.Text = $"Duration: {currentTour.DurationHours}h";
        }

        private void LoadReservations()
        {
            var reservationRepository = new TourReservationRepository();
            reservations = reservationRepository.GetAll()
                          .Where(r => r.TourId == currentTour.Id && r.Status == TourReservationStatus.ACTIVE)
                          .ToList();

            attendanceList = reservations
                .SelectMany(r => r.Guests)
                .Select(g => new TouristAttendance
                {
                    TourId = currentTour.Id,
                    GuestId = g.Id,
                    HasAppeared = false,
                    KeyPointJoinedAt = -1
                })
                .ToList();
        }

        private void LoadKeyPoints()
        {
            KeyPointsPanel.Children.Clear();

            int count = 1;
            foreach (var kp in currentTour.KeyPoints)
            {
                var cb = new CheckBox
                {
                    Content = $"#{count} {kp.Name}",
                    Margin = new Thickness(0, 5, 0, 5)
                };

                cb.Tag = kp;
                cb.Click += KeyPointCheckBox_Click;

                KeyPointsPanel.Children.Add(cb);
                count++;
            }

            if (KeyPointsPanel.Children.Count > 0 && KeyPointsPanel.Children[0] is CheckBox firstCb)
                firstCb.IsChecked = true;
        }

        private void KeyPointCheckBox_Click(object sender, RoutedEventArgs e)
        {
            KeyPointChecked();
        }

        private bool AllKeyPointsVisited()
        {
            foreach (var child in KeyPointsPanel.Children)
            {
                if (child is CheckBox cb && cb.IsChecked != true)
                    return false;
            }
            return true;
        }

        private void TouristsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenTouristWindow();
        }

        private void OpenTouristWindow()
        {
            var passedKeyPoints = currentTour.KeyPoints
                                    .Where(kp =>
                                        KeyPointsPanel.Children
                                        .OfType<CheckBox>()
                                        .Any(cb => cb.Tag == kp && cb.IsChecked == true))
                                    .ToList();

            if (!passedKeyPoints.Any())
            {
                MessageBox.Show("Nema označenih tačaka – turisti se ne mogu dodeliti.");
                return;
            }

            var guests = reservations.SelectMany(r => r.Guests).ToList();
            var dialog = new ReservedTouristsWindow(currentTour, passedKeyPoints, guests, attendanceList);

            if (dialog.ShowDialog() == true)
            {
                var presentGuestIds = dialog.GetPresentGuestIds();
                attendanceList = dialog.GetUpdatedAttendance();
                WriteAttendantsToFile(attendanceList);

                
                if (presentGuestIds.Any())
                {
                    try
                    {
                        CreatePresenceNotifications(presentGuestIds);
                        MessageBox.Show($"Prisustvo zabeleženo za {presentGuestIds.Count} gostiju. Obaveštenja poslata.",
                                      "Uspešno", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Greška pri slanju obaveštenja: {ex.Message}",
                                      "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                KeyPointChecked();
            }
        }

        private void CreatePresenceNotifications(List<int> presentGuestIds)
        {
            var tourPresenceNotificationRepo = new TourPresenceNotificationRepository();
            var guestRepository = new ReservationGuestRepository();
            var userRepository = new UserRepository();
            var reservationRepository = new TourReservationRepository();

            // Grupiši goste po rezervaciji (po glavnom turistu)
            var guestsByTourist = new Dictionary<int, List<ReservationGuest>>();

            foreach (var guestId in presentGuestIds)
            {
                var guest = guestRepository.GetById(guestId);
                if (guest == null) continue;

                // Pronađi rezervaciju za ovog gosta
                var reservation = reservationRepository.GetAll()
                    .FirstOrDefault(r => r.TourId == currentTour.Id &&
                                        r.Guests.Any(g => g.Id == guestId));

                if (reservation == null) continue;

                var touristId = reservation.TouristId;

                if (!guestsByTourist.ContainsKey(touristId))
                {
                    guestsByTourist[touristId] = new List<ReservationGuest>();
                }
                guestsByTourist[touristId].Add(guest);
            }

            // Kreiraj obaveštenje za svakog glavnog turista
            foreach (var kvp in guestsByTourist)
            {
                int touristId = kvp.Key;
                var presentGuests = kvp.Value;

                var mainTourist = userRepository.GetById(touristId);
                if (mainTourist == null) continue;

                // Kreiraj listu imena prisutnih
                var presentNames = new List<string>();

                // Dodaj glavnog turista ako je označen kao prisutan
                var mainTouristGuest = presentGuests.FirstOrDefault(g =>
                    g.FirstName == mainTourist.FirstName && g.LastName == mainTourist.LastName);

                if (mainTouristGuest != null)
                {
                    presentNames.Add($"{mainTourist.FirstName} {mainTourist.LastName} (Vi)");
                }

                // Dodaj ostale goste
                foreach (var guest in presentGuests.Where(g => g != mainTouristGuest))
                {
                    presentNames.Add($"{guest.FirstName} {guest.LastName}");
                }

                if (presentNames.Any())
                {
                    var guestList = string.Join(", ", presentNames);
                    var message = $"Vodič je zabeležio prisustvo na turi '{currentTour.Name}' za: {guestList}";

                    // Kreiraj obaveštenje
                    var notification = new TourPresenceNotification
                    {
                        TourId = currentTour.Id,
                        UserId = touristId,
                        Message = message,
                        CreatedAt = DateTime.Now,
                        IsRead = false
                    };

                    tourPresenceNotificationRepo.Save(notification);
                }
            }
        }
        private void RefreshAttendanceData()
        {
            if (_touristAttendanceService != null)
            {
                var allAttendance = _touristAttendanceService.GetByTourId(currentTour.Id);

                attendanceList = allAttendance.Select(dto => new TouristAttendance
                {
                    Id = dto.Id,
                    GuestId = dto.GuestId,
                    TourId = dto.TourId,
                    HasAppeared = dto.HasAppeared,
                    KeyPointJoinedAt = dto.KeyPointJoinedAt,
                    StartTourTime = dto.StartTourTime
                }).ToList();
            }
        }

        private bool touristsWindowOpened = false;

        private void KeyPointChecked()
        {
            var currentKeyPointIndex = GetCurrentKeyPointIndex();

            if (_tourPresenceService != null)
            {
                try
                {
                    _tourPresenceService.UpdateTourKeyPointProgress(currentTour.Id, currentKeyPointIndex);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Greška pri ažuriranju progresa: {ex.Message}");
                    UpdateTourProgressOldWay(currentKeyPointIndex);
                }
            }
            else
            {
                UpdateTourProgressOldWay(currentKeyPointIndex);
            }

            if (AllKeyPointsVisited())
            {
                if (!touristsWindowOpened)
                {
                    touristsWindowOpened = true;
                    OpenTouristWindow();
                }
                EndTour();
            }
        }

        private void UpdateTourProgressOldWay(int currentKeyPointIndex)
        {
            var tourPresenceRepo = new TourPresenceRepository();
            var activeTourists = tourPresenceRepo.GetByTourId(currentTour.Id)
                                                .Where(tp => tp.IsPresent)
                                                .ToList();

            var currentKeyPoint = currentTour.KeyPoints?.ElementAtOrDefault(currentKeyPointIndex);
            var keyPointId = currentKeyPoint?.Id ?? 0;

            foreach (var tourist in activeTourists)
            {
                tourist.CurrentKeyPointIndex = currentKeyPointIndex;
                tourist.KeyPointId = keyPointId;
                tourist.LastUpdated = DateTime.Now;
                tourPresenceRepo.Update(tourist);
            }
        }

        private int GetCurrentKeyPointIndex()
        {
            int index = 0;
            foreach (var child in KeyPointsPanel.Children)
            {
                if (child is CheckBox cb && cb.IsChecked == true)
                {
                    index++;

                }
                else
                {
                    break;
                }
            }
            return index - 1;

        }

        private void WriteAttendantsToFile(List<TouristAttendance> attendantsList)
        {
            var tourPresenceRepo = new TourPresenceRepository();

            foreach (var attendant in attendantsList)
            {
                var existing = attendanceRepo.GetAll().FirstOrDefault(a => a.Id == attendant.Id);
                if (existing == null)
                {
                    attendanceRepo.Save(attendant);
                }
                else
                {
                    existing.GuestId = attendant.GuestId;
                    existing.TourId = attendant.TourId;
                    existing.HasAppeared = attendant.HasAppeared;
                    existing.KeyPointJoinedAt = attendant.KeyPointJoinedAt;
                    existing.StartTourTime = startTime;
                    attendanceRepo.Update(existing);

                    var guest = guestsRepository.GetAll().FirstOrDefault(g => g.Id == attendant.GuestId);
                    if (guest != null)
                    {
                        guest.HasAppeared = attendant.HasAppeared;
                        guest.KeyPointJoinedAt = attendant.KeyPointJoinedAt;
                        guestsRepository.Update(guest);
                        guestsRepository.SaveAll();
                    }
                }

                if (attendant.HasAppeared)
                {
                    var existingPresence = tourPresenceRepo.GetByTourAndUser(currentTour.Id, attendant.GuestId);
                    var passedKeyPoints = currentTour.KeyPoints
                                    .Where(kp =>
                                        KeyPointsPanel.Children
                                        .OfType<CheckBox>()
                                        .Any(cb => cb.Tag == kp && cb.IsChecked == true))
                                    .ToList();

                    if (!passedKeyPoints.Any())
                    {
                        MessageBox.Show("Nema označenih tačaka – turisti se ne mogu dodeliti.");
                        return;
                    }

                    
                    var lastKeyPointId = passedKeyPoints.OrderBy(kp => kp.Id).Last().Id;

                    
                    var targetKeyPoint = currentTour.KeyPoints?.ElementAtOrDefault(attendant.KeyPointJoinedAt);
                    var keyPointId = targetKeyPoint?.Id ?? lastKeyPointId;

                    if (existingPresence == null)
                    {
                        var newPresence = new TourPresence
                        {
                            TourId = currentTour.Id,
                            UserId = attendant.GuestId,
                            JoinedAt = DateTime.Now,
                            IsPresent = true,
                            CurrentKeyPointIndex = attendant.KeyPointJoinedAt,
                            LastUpdated = DateTime.Now,
                            KeyPointId = keyPointId
                        };
                        tourPresenceRepo.Save(newPresence);
                    }
                    else
                    {
                        existingPresence.CurrentKeyPointIndex = attendant.KeyPointJoinedAt;
                        existingPresence.KeyPointId = keyPointId;
                        existingPresence.IsPresent = true;
                        existingPresence.LastUpdated = DateTime.Now;
                        tourPresenceRepo.Update(existingPresence);
                    }
                }
            }
        }

        private void StopTourButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Da li ste sigurni da želite da završite turu ranije?",
                                          "Potvrda",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                EndTour();
            }
        }

        private void EndTour()
        {
            currentTour.Status = TourStatus.FINISHED;
            tourRepository.Update(currentTour);

            var reservationRepo = new TourReservationRepository();
            foreach (var reservation in reservations)
            {
                reservation.Status = TourReservationStatus.COMPLETED;
                reservationRepo.Update(reservation);
            }

            MessageBox.Show("Sve ključne tačke su prošle. Tura je uspešno završena.",
                            "Kraj Ture", MessageBoxButton.OK, MessageBoxImage.Information);

            mainPage.ContentFrame.Content = new ToursControl(mainPage);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainPage.ContentFrame.CanGoBack)
                mainPage.ContentFrame.GoBack();
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Klikni na ključne tačke kako bi ih obeležio. Dugme 'Turisti' otvara listu turista i omogućava da označiš kada su se pridružili.");
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}