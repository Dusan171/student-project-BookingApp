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

        public TourPresenceService(ITourPresenceRepository presenceRepository,
                                 ITourRepository tourRepository,
                                 IUserRepository userRepository,
                                 INotificationService notificationService,
                                 ITourReservationRepository tourReservationRepository)

        {
            _presenceRepository = presenceRepository ?? throw new ArgumentNullException(nameof(presenceRepository));
            _tourRepository = tourRepository ?? throw new ArgumentNullException(nameof(tourRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _tourReservationRepository = tourReservationRepository ?? throw new ArgumentNullException(nameof(tourReservationRepository));
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
            // Proverimo da li već postoji prisustvo za ovog korisnika na turi
            var existingPresence = _presenceRepository.GetByTourAndUser(tourId, userId);
            if (existingPresence != null)
            {
                existingPresence.IsPresent = true;
                existingPresence.LastUpdated = DateTime.Now;
                var updatedPresence = _presenceRepository.Update(existingPresence);
                return EnrichPresenceAndConvertToDTO(updatedPresence);
            }

            // Kreiraj novo prisustvo
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
            // 1. Učitaj sve rezervacije korisnika
            var userReservations = _tourReservationRepository.GetByTouristId(userId);

            // 2. Filtriraj samo ACTIVE rezervacije
            var activeReservations = userReservations
                .Where(r => r.Status == TourReservationStatus.ACTIVE)
                .ToList();

            // 3. Napravi DTO-eve za prisustvo
            var activePresences = new List<TourPresenceDTO>();

            foreach (var reservation in activeReservations)
            {
                var tour = _tourRepository.GetById(reservation.TourId);
                if (tour != null)
                {
                    // Pretpostavljam da CurrentKeyPointIndex i CurrentKeyPointName mogu biti dobijeni iz Tour ili TourPresence logike
                    var currentIndex = reservation.CurrentKeyPointIndex; // dodaj ovo polje u TourReservation ako već ne postoji
                    var currentKeyPointName = tour.KeyPoints.ElementAtOrDefault(currentIndex)?.Name ?? "Početak";

                    var dto = new TourPresenceDTO
                    {
                        TourId = tour.Id,
                        TourName = tour.Name,
                        OriginalTour = tour,
                        JoinedAt = reservation.ReservationDate,
                        CurrentKeyPointIndex = currentIndex,
                        CurrentKeyPointName = currentKeyPointName,
                        ProgressText = $"{currentIndex}/{tour.KeyPoints.Count}",
                        IsPresent = true, // ili neka logika prema prisustvu korisnika
                        LastUpdated = DateTime.Now
                    };

                    activePresences.Add(dto);
                }
            }

            return activePresences;
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
                if (user != null)
                {
                    // Ovde bi trebalo kreirati notifikaciju
                    // ali posto NotificationService radi sa ReservationId, 
                    // možda treba modifikacija ili nova vrsta notifikacije
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

            // Obogati sa detaljima ture
            var tour = _tourRepository.GetById(presence.TourId);
            if (tour != null)
            {
                dto.TourName = tour.Name;
                dto.OriginalTour = tour;

                // Postaviti trenutnu ključnu tačku
                if (tour.KeyPoints != null && tour.KeyPoints.Count > presence.CurrentKeyPointIndex)
                {
                    dto.CurrentKeyPointName = tour.KeyPoints[presence.CurrentKeyPointIndex].Name;
                }
            }

            return dto;
        }
    }
}
