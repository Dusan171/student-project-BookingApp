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
                var notifications = _notificationRepository.GetByTouristId(touristId);
                var latest = notifications.FirstOrDefault();

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
                    Tour = CreateNotifiedTourDTO(tour)
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Greška pri preuzimanju poslednje notifikacije: {ex.Message}", ex);
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

        public void CreateNotificationsForNewTour(int tourId, string tourName, string location, string language)
        {
            try
            {
                var tour = _tourRepository.GetById(tourId);
                if (tour == null)
                    return;

                // Pronađi sve neispunjene zahteve koji se poklapaju sa ovom turom
                var allRequests = _tourRequestRepository.GetAll();
                var matchingRequests = allRequests
                    .Where(r => r.Status == TourRequestStatus.PENDING || r.Status == TourRequestStatus.INVALID)
                    .Where(r =>
                        r.Language.Equals(language, StringComparison.OrdinalIgnoreCase) ||
                        $"{r.City}, {r.Country}".Equals(location, StringComparison.OrdinalIgnoreCase))
                    .GroupBy(r => r.TouristId)
                    .Select(g => g.First())
                    .ToList();

                foreach (var request in matchingRequests)
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

        private NotifiedTourDTO CreateNotifiedTourDTO(Tour tour)
        {
            var guide = _userRepository.GetById(tour.GuideId);

            return new NotifiedTourDTO
            {
                Id = tour.Id,
                Name = tour.Name,
                Location = tour.Location != null
                    ? $"{tour.Location.City}, {tour.Location.Country}"
                    : "N/A",
                Language = tour.Language,
                Date = tour.StartTimes?.FirstOrDefault()?.Time ?? DateTime.Now,
                GuideName = guide != null ? $"{guide.FirstName} {guide.LastName}" : "N/A",
                MaxGuests = tour.MaxTourists,
                Description = tour.Description
            };
        }
    }
}