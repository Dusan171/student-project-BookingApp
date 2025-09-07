using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Repositories;
using BookingApp.Services.DTO;

namespace BookingApp.Services
{
    public class TourPresenceService : ITourPresenceService
    {
        
        private readonly ITourPresenceRepository _presenceRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly ITourReservationRepository _tourReservationRepository;
        private readonly IReservationGuestRepository _guestRepository;
        private readonly ITourPresenceNotificationService _tourPresenceNotificationService;

        public TourPresenceService(ITourPresenceRepository presenceRepository,
                                 ITourRepository tourRepository,
                                 IUserRepository userRepository,
                                 INotificationService notificationService,
                                 ITourReservationRepository tourReservationRepository,
                                 IReservationGuestRepository guestRepository,
                                 ITourPresenceNotificationService tourPresenceNotificationService)
        {
            _presenceRepository = presenceRepository ?? throw new ArgumentNullException(nameof(presenceRepository));
            _tourRepository = tourRepository ?? throw new ArgumentNullException(nameof(tourRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _tourReservationRepository = tourReservationRepository ?? throw new ArgumentNullException(nameof(tourReservationRepository));
            _guestRepository = guestRepository ?? throw new ArgumentNullException(nameof(guestRepository));
            _tourPresenceNotificationService = tourPresenceNotificationService ?? throw new ArgumentNullException(nameof(tourPresenceNotificationService));
        }

        public List<TourPresenceDTO> GetAllPresences()
        {
            var presences = _presenceRepository.GetAll();
            return EnrichPresencesAndConvertToDTO(presences);
        }

        public TourPresenceDTO GetPresenceById(int id)
        {
            var presence = _presenceRepository.GetById(id);
            if (presence == null) return null;

            return EnrichPresenceAndConvertToDTO(presence);
        }

        public TourPresenceDTO CreatePresence(TourPresenceDTO presenceDTO)
        {
            if (presenceDTO == null)
                throw new ArgumentNullException(nameof(presenceDTO));

            var presence = presenceDTO.ToTourPresence();
            var savedPresence = _presenceRepository.Save(presence);

            return EnrichPresenceAndConvertToDTO(savedPresence);
        }

        public TourPresenceDTO UpdatePresence(TourPresenceDTO presenceDTO)
        {
            if (presenceDTO == null)
                throw new ArgumentNullException(nameof(presenceDTO));

            var presence = presenceDTO.ToTourPresence();
            presence.LastUpdated = DateTime.Now;
            var updatedPresence = _presenceRepository.Update(presence);

            return EnrichPresenceAndConvertToDTO(updatedPresence);
        }

        public void DeletePresence(int id)
        {
            var presence = _presenceRepository.GetById(id);
            if (presence != null)
            {
                _presenceRepository.Delete(presence);
            }
        }

        public List<TourPresenceDTO> GetPresencesByTour(int tourId)
        {
            var presences = _presenceRepository.GetByTourId(tourId);
            return EnrichPresencesAndConvertToDTO(presences);
        }

        public List<TourPresenceDTO> GetPresencesByUser(int userId)
        {
            var presences = _presenceRepository.GetByUserId(userId);
            return EnrichPresencesAndConvertToDTO(presences);
        }

        public TourPresenceDTO GetUserPresenceOnTour(int tourId, int userId)
        {
            var presence = _presenceRepository.GetByTourAndUser(tourId, userId);
            if (presence == null) return null;

            return EnrichPresenceAndConvertToDTO(presence);
        }

        public TourPresenceDTO JoinTour(int tourId, int userId)
        {
            var existingPresence = _presenceRepository.GetByTourAndUser(tourId, userId);
            if (existingPresence != null)
            {
                existingPresence.IsPresent = true;
                existingPresence.LastUpdated = DateTime.Now;
                var updatedPresence = _presenceRepository.Update(existingPresence);
                return EnrichPresenceAndConvertToDTO(updatedPresence);
            }

            var newPresence = new TourPresence
            {
                TourId = tourId,
                UserId = userId,
                JoinedAt = DateTime.Now,
                IsPresent = true,
                CurrentKeyPointIndex = 0,
                LastUpdated = DateTime.Now
            };

            var savedPresence = _presenceRepository.Save(newPresence);
            return EnrichPresenceAndConvertToDTO(savedPresence);
        }

        public TourPresenceDTO UpdateKeyPointProgress(int tourId, int userId, int keyPointIndex)
        {
            var presence = _presenceRepository.GetByTourAndUser(tourId, userId);
            if (presence == null) return null;

            presence.CurrentKeyPointIndex = keyPointIndex;
            presence.LastUpdated = DateTime.Now;

            var updatedPresence = _presenceRepository.Update(presence);
            return EnrichPresenceAndConvertToDTO(updatedPresence);
        }

        public List<TourPresenceDTO> GetActiveTourPresences(int userId)
        {
            var userReservations = _tourReservationRepository.GetByTouristId(userId);
            var activeReservations = userReservations
                .Where(r => r.Status == TourReservationStatus.ACTIVE)
                .ToList();

            var activePresences = new List<TourPresenceDTO>();

            foreach (var reservation in activeReservations)
            {
                var tour = _tourRepository.GetById(reservation.TourId);
                if (tour == null) continue;

                if (tour.Status == TourStatus.ACTIVE)
                {
                    var existingPresence = _presenceRepository.GetByTourAndUser(tour.Id, userId);

                    if (existingPresence != null)
                    {
                        var dto = EnrichPresenceAndConvertToDTO(existingPresence);

                        var currentKeyPointIndex = GetCurrentTourKeyPointIndex(tour.Id);
                        if (currentKeyPointIndex != existingPresence.CurrentKeyPointIndex)
                        {
                            existingPresence.CurrentKeyPointIndex = currentKeyPointIndex;

                            var currentKeyPoint = tour.KeyPoints?.ElementAtOrDefault(currentKeyPointIndex);
                            if (currentKeyPoint != null)
                            {
                                existingPresence.KeyPointId = currentKeyPoint.Id;
                            }

                            existingPresence.LastUpdated = DateTime.Now;
                            _presenceRepository.Update(existingPresence);

                            dto = EnrichPresenceAndConvertToDTO(existingPresence);
                        }

                        activePresences.Add(dto);
                    }
                    else
                    {
                        var currentKeyPointIndex = GetCurrentTourKeyPointIndex(tour.Id);
                        var keyPoints = _tourRepository.GetKeyPointsForTour(tour.Id);

                        var keyPointId = keyPoints != null && currentKeyPointIndex >= 0 && currentKeyPointIndex < keyPoints.Count
                            ? keyPoints[currentKeyPointIndex].Id
                            : (keyPoints?.FirstOrDefault()?.Id ?? 0);

                        var dto = new TourPresenceDTO
                        {
                            TourId = tour.Id,
                            UserId = userId,
                            TourName = tour.Name,
                            JoinedAt = reservation.ReservationDate,
                            CurrentKeyPointIndex = currentKeyPointIndex,
                            IsPresent = true,
                            LastUpdated = DateTime.Now,
                            KeyPointId = keyPointId
                        };

                        dto.SetKeyPointsFromTour(tour);
                        dto.RefreshKeyPointName();
                        activePresences.Add(dto);
                    }
                }
            }

            return activePresences;
        }

        private int GetCurrentTourKeyPointIndex(int tourId)
        {
            var allPresences = _presenceRepository.GetByTourId(tourId)
                .Where(p => p.IsPresent)
                .ToList();

            if (allPresences.Any())
            {
                var maxKeyPointIndex = allPresences.Max(p => p.CurrentKeyPointIndex);
                return maxKeyPointIndex;
            }

            return 0;
        }

        public void UpdateTourKeyPointProgress(int tourId, int keyPointIndex)
        {
            var allPresences = _presenceRepository.GetByTourId(tourId)
                .Where(p => p.IsPresent)
                .ToList();

            foreach (var presence in allPresences)
            {
                presence.CurrentKeyPointIndex = keyPointIndex;
                presence.LastUpdated = DateTime.Now;
                _presenceRepository.Update(presence);
            }
        }

        public void MarkUserAsPresent(int tourId, int userId, bool isPresent)
        {
            var presence = _presenceRepository.GetByTourAndUser(tourId, userId);
            if (presence != null)
            {
                presence.IsPresent = isPresent;
                presence.LastUpdated = DateTime.Now;
                _presenceRepository.Update(presence);
            }
        }

        public void NotifyUsersAboutPresence(int tourId, List<int> presentUserIds)
        {
            var tour = _tourRepository.GetById(tourId);
            if (tour == null) return;

            foreach (var userId in presentUserIds)
            {
                var user = _userRepository.GetById(userId);
                if (user == null) continue;

                // Pronađi rezervaciju ovog korisnika za ovu turu
                var reservation = _tourReservationRepository.GetByTouristId(userId)
                    .FirstOrDefault(r => r.TourId == tourId && r.Status == TourReservationStatus.ACTIVE);

                if (reservation == null) continue;

                // Učitaj sve goste vezane za ovu rezervaciju
                var guests = _guestRepository.GetByReservationId(reservation.Id);

                // Kreiraj listu imena svih prisutnih
                var presentNames = new List<string>();
                presentNames.Add($"{user.FirstName} {user.LastName}");

                if (guests != null)
                {
                    foreach (var guest in guests)
                    {
                        presentNames.Add($"{guest.FirstName} {guest.LastName}");
                    }
                }

                var guestList = string.Join(", ", presentNames);
                var message = $"Vodič je zabeležio prisustvo na turi '{tour.Name}' za: {guestList}";

                // Koristi NOVI servis za tour prisustvo obaveštenja
                _tourPresenceNotificationService.CreatePresenceNotification(tourId, userId, message);
            }
        }

        public void MarkGuestsAsPresent(int tourId, List<int> guestIds)
        {
            var tour = _tourRepository.GetById(tourId);
            if (tour == null) return;

            var guestsByReservation = new Dictionary<int, List<ReservationGuest>>();

            foreach (var guestId in guestIds)
            {
                var guest = _guestRepository.GetById(guestId);
                if (guest == null) continue;

                if (!guestsByReservation.ContainsKey(guest.TouristId))
                {
                    guestsByReservation[guest.TouristId] = new List<ReservationGuest>();
                }
                guestsByReservation[guest.TouristId].Add(guest);
            }

            SendPresenceNotifications(tourId, guestsByReservation);
        }

        public Dictionary<int, List<ReservationGuestDTO>> GetGuestsGroupedByReservation(int tourId)
        {
            var reservations = _tourReservationRepository.GetByTourId(tourId)
                .Where(r => r.Status == TourReservationStatus.ACTIVE);

            var result = new Dictionary<int, List<ReservationGuestDTO>>();

            foreach (var reservation in reservations)
            {
                var guests = _guestRepository.GetByReservationId(reservation.Id);
                var mainTourist = _userRepository.GetById(reservation.TouristId);

                var guestDTOs = guests.Select(g =>
                {
                    var dto = ReservationGuestDTO.FromDomain(g);
                    dto.IsMainContact = !string.IsNullOrEmpty(g.Email) ||
                                       (mainTourist != null &&
                                        g.FirstName == mainTourist.FirstName &&
                                        g.LastName == mainTourist.LastName);
                    return dto;
                }).ToList();

                if (guestDTOs.Any())
                {
                    result[reservation.TouristId] = guestDTOs;
                }
            }

            return result;
        }

        public List<GuestAttendanceStatusDTO> GetGuestAttendanceStatus(int tourId)
        {
            TouristAttendanceRepository attendanceRepository = new TouristAttendanceRepository();
            var guestsGrouped = GetGuestsGroupedByReservation(tourId);
            var result = new List<GuestAttendanceStatusDTO>();

            foreach (var kvp in guestsGrouped)
            {
                int touristId = kvp.Key;
                var guests = kvp.Value;
                var mainTourist = _userRepository.GetById(touristId);

                foreach (var guest in guests)
                {
                    result.Add(new GuestAttendanceStatusDTO
                    {
                        GuestId = guest.Id,
                        GuestName = guest.FullName,
                        IsMainContact = guest.IsMainContact,
                        MainTouristName = mainTourist != null ? $"{mainTourist.FirstName} {mainTourist.LastName}" : "",
                        HasAppeared = attendanceRepository.GetByGuestId(guest.Id).FirstOrDefault().HasAppeared,
                        KeyPointJoinedAt = attendanceRepository.GetByGuestId(guest.Id).FirstOrDefault().KeyPointJoinedAt
                    });
                }
            }

            return result;
        }

        private void SendPresenceNotifications(int tourId, Dictionary<int, List<ReservationGuest>> guestsByReservation)
        {
            var tour = _tourRepository.GetById(tourId);
            if (tour == null) return;

            foreach (var kvp in guestsByReservation)
            {
                int touristId = kvp.Key;
                var presentGuests = kvp.Value;

                var mainTourist = _userRepository.GetById(touristId);
                if (mainTourist == null) continue;

                var presentNames = new List<string>();

                var mainTouristGuest = presentGuests.FirstOrDefault(g =>
                    !string.IsNullOrEmpty(g.Email) ||
                    (g.FirstName == mainTourist.FirstName && g.LastName == mainTourist.LastName));

                if (mainTouristGuest != null)
                {
                    presentNames.Add($"{mainTourist.FirstName} {mainTourist.LastName} (Vi)");
                }

                foreach (var guest in presentGuests.Where(g => g != mainTouristGuest))
                {
                    presentNames.Add($"{guest.FirstName} {guest.LastName}");
                }

                if (presentNames.Any())
                {
                    var guestList = string.Join(", ", presentNames);
                    var message = $"Vodič je zabeležio prisustvo na turi '{tour.Name}' za: {guestList}";

                    _tourPresenceNotificationService.CreatePresenceNotification(tourId, touristId, message);
                }
            }
        }

        private List<TourPresenceDTO> EnrichPresencesAndConvertToDTO(List<TourPresence> presences)
        {
            var dtos = new List<TourPresenceDTO>();
            foreach (var presence in presences)
            {
                dtos.Add(EnrichPresenceAndConvertToDTO(presence));
            }
            return dtos;
        }

        private TourPresenceDTO EnrichPresenceAndConvertToDTO(TourPresence presence)
        {
            var dto = TourPresenceDTO.FromDomain(presence);

            var tour = _tourRepository.GetById(presence.TourId);
            if (tour != null)
            {
                dto.TourName = tour.Name;
                dto.SetKeyPointsFromTour(tour);

                dto.KeyPointId = presence.KeyPointId;
                dto.RefreshKeyPointName();
            }

            return dto;
        }

    }
}