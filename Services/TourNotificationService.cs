using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Services.DTO;

namespace BookingApp.Services
{
    public class TourNotificationService : ITourNotificationService
    {
        private readonly ITourNotificationRepository _notificationRepository;
        private readonly ITourRequestRepository _tourRequestRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IUserRepository _userRepository;

        public TourNotificationService(
            ITourNotificationRepository notificationRepository,
            ITourRequestRepository tourRequestRepository,
            ITourRepository tourRepository,
            IUserRepository userRepository)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _tourRequestRepository = tourRequestRepository ?? throw new ArgumentNullException(nameof(tourRequestRepository));
            _tourRepository = tourRepository ?? throw new ArgumentNullException(nameof(tourRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public List<TourNotificationDTO> GetNotificationsByTourist(int touristId)
        {
            try
            {
                var notifications = _notificationRepository.GetByTouristId(touristId);
                var dtos = new List<TourNotificationDTO>();

                foreach (var notification in notifications)
                {
                    var tour = _tourRepository.GetById(notification.TourId);
                    if (tour != null)
                    {
                        dtos.Add(new TourNotificationDTO
                        {
                            Id = notification.Id,
                            Title = notification.Title,
                            Message = notification.Message,
                            CreatedAt = notification.CreatedAt,
                            TourId = notification.TourId,
                            IsRead = notification.IsRead,
                            Tour = CreateNotifiedTourDTO(tour)
                        });
                    }
                }

                return dtos;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Greška pri preuzimanju notifikacija: {ex.Message}", ex);
            }
        }

        public TourNotificationDTO GetLatestNotification(int touristId)
        {
            try
            {
                var unreadNotifications = _notificationRepository.GetUnreadByTouristId(touristId);
                var latest = unreadNotifications.FirstOrDefault();

                if (latest == null)
                    return null;

                var tour = _tourRepository.GetById(latest.TourId);
                if (tour == null)
                    return null;

                return new TourNotificationDTO
                {
                    Id = latest.Id,
                    Title = latest.Title,
                    Message = latest.Message,
                    CreatedAt = latest.CreatedAt,
                    TourId = latest.TourId,
                    IsRead = latest.IsRead,
                    Tour = CreateNotifiedTourDTO(tour)
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Greška pri preuzimanju poslednje notifikacije: {ex.Message}", ex);
            }
        }

        public void CreateNotificationsForNewTour(int tourId, string tourName, string location, string language)
        {
            try
            {
                var tour = _tourRepository.GetById(tourId);
                if (tour == null)
                    return;

                var allRequests = _tourRequestRepository.GetAll();

                var neverFulfilledRequests = allRequests
                    .Where(r => r.Status == TourRequestStatus.PENDING || r.Status == TourRequestStatus.INVALID)
                    .ToList();

                if (neverFulfilledRequests.Count == 0)
                    return;

                var matchingRequests = new List<TourRequest>();

                foreach (var request in neverFulfilledRequests)
                {
                    bool matches = false;

                    if (MatchesLocation(location, request.City, request.Country))
                    {
                        matches = true;
                    }

                    if (MatchesLanguage(language, request.Language))
                    {
                        matches = true;
                    }

                    if (matches)
                    {
                        matchingRequests.Add(request);
                    }
                }

                if (matchingRequests.Count == 0)
                    return;

                var uniqueTourists = matchingRequests
                    .GroupBy(r => r.TouristId)
                    .Select(g => g.First())
                    .ToList();

                foreach (var request in uniqueTourists)
                {
                    var notification = new TourNotification
                    {
                        TouristId = request.TouristId,
                        TourId = tourId,
                        Title = "Nova tura kreirana za vaš zahtev!",
                        Message = $"Kreirana je nova tura '{tourName}' koja odgovara vašem zahtevu za ture na {language} jeziku u lokaciji {location}.",
                        CreatedAt = DateTime.Now,
                        IsRead = false
                    };

                    _notificationRepository.Save(notification);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Greška pri kreiranju notifikacija: {ex.Message}", ex);
            }
        }

        public void SendTourAcceptanceNotification(int touristId, int tourId, string tourName, string location, DateTime scheduledDate, string guideName)
        {
            try
            {
                var notification = new TourNotification
                {
                    TouristId = touristId,
                    TourId = tourId,
                    Title = "Vaš zahtev za turu je prihvaćen!",
                    Message = $"Vaš zahtev za turu '{tourName}' u {location} je prihvaćen! Tura je zakazana za {scheduledDate:dd.MM.yyyy} u {scheduledDate:HH:mm}. Vodič: {guideName}",
                    CreatedAt = DateTime.Now,
                    IsRead = false
                };

                _notificationRepository.Save(notification);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Greška pri slanju obaveštenja o prihvaćenoj turi: {ex.Message}", ex);
            }
        }

        public void MarkAsRead(int notificationId)
        {
            try
            {
                var notification = _notificationRepository.GetById(notificationId);
                if (notification != null)
                {
                    notification.IsRead = true;
                    _notificationRepository.Update(notification);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Greška pri označavanju notifikacije kao pročitane: {ex.Message}", ex);
            }
        }

        private bool MatchesLocation(string tourLocation, string requestCity, string requestCountry)
        {
            if (string.IsNullOrWhiteSpace(tourLocation))
                return false;

            string requestLocation = $"{requestCity}, {requestCountry}".Trim();

            if (string.IsNullOrWhiteSpace(requestLocation) || requestLocation == ", ")
                return false;

            bool exactMatch = tourLocation.Equals(requestLocation, StringComparison.OrdinalIgnoreCase);

            bool cityMatch = !string.IsNullOrWhiteSpace(requestCity) &&
                             tourLocation.Contains(requestCity, StringComparison.OrdinalIgnoreCase);

            return exactMatch || cityMatch;
        }

        private bool MatchesLanguage(string tourLanguage, string requestLanguage)
        {
            if (string.IsNullOrWhiteSpace(tourLanguage) || string.IsNullOrWhiteSpace(requestLanguage))
                return false;

            string normalizedTourLanguage = NormalizeLanguage(tourLanguage);
            string normalizedRequestLanguage = NormalizeLanguage(requestLanguage);

            return normalizedTourLanguage.Equals(normalizedRequestLanguage, StringComparison.OrdinalIgnoreCase);
        }

        private string NormalizeLanguage(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
                return string.Empty;

            string normalized = language.Replace("System.Windows.Controls.ComboBoxItem:", "").Trim().ToLower();

            var languageMap = new Dictionary<string, string>
            {
                { "srpski", "srpski" },
                { "serbian", "srpski" },
                { "engleski", "engleski" },
                { "english", "engleski" },
                { "francuski", "francuski" },
                { "french", "francuski" },
                { "nemacki", "nemacki" },
                { "nemački", "nemacki" },
                { "german", "nemacki" },
                { "ceski", "ceski" },
                { "češki", "ceski" },
                { "czech", "ceski" },
                { "hrvatski", "hrvatski" },
                { "croatian", "hrvatski" }
            };

            return languageMap.ContainsKey(normalized) ? languageMap[normalized] : normalized;
        }

        private NotifiedTourDTO CreateNotifiedTourDTO(Tour tour)
        {
            var guide = _userRepository.GetById(tour.GuideId);

            return new NotifiedTourDTO
            {
                Id = tour.Id,
                Name = tour.Name,
                Location = tour.Location != null ? $"{tour.Location.City}, {tour.Location.Country}" : "N/A",
                Language = tour.Language,
                Date = tour.StartTimes?.FirstOrDefault()?.Time ?? DateTime.Now,
                GuideName = guide != null ? $"{guide.FirstName} {guide.LastName}" : "N/A",
                MaxGuests = tour.MaxTourists,
                Description = tour.Description
            };
        }
    }
}