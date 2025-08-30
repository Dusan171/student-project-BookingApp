using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Services
{
    public class ReservationGuestService : IReservationGuestService
    {
        private readonly IReservationGuestRepository _guestRepository;

        public ReservationGuestService(IReservationGuestRepository guestRepository)
        {
            _guestRepository = guestRepository ?? throw new ArgumentNullException(nameof(guestRepository));
        }

        public List<ReservationGuestDTO> GetAllGuests()
        {
            var guests = _guestRepository.GetAll();
            return guests.Select(g => ReservationGuestDTO.FromDomain(g)).ToList();
        }

        public ReservationGuestDTO? GetGuestById(int id)
        {
            var guest = _guestRepository.GetById(id);
            return guest != null ? ReservationGuestDTO.FromDomain(guest) : null;
        }

        public ReservationGuestDTO AddGuest(ReservationGuestDTO guestDTO)
        {
            if (guestDTO == null)
                throw new ArgumentNullException(nameof(guestDTO));

            if (!ValidateGuest(guestDTO))
                throw new ArgumentException("Guest validation failed");

            var guest = guestDTO.ToReservationGuest();
            var savedGuest = _guestRepository.Add(guest);
            return ReservationGuestDTO.FromDomain(savedGuest);
        }

        public ReservationGuestDTO UpdateGuest(ReservationGuestDTO guestDTO)
        {
            if (guestDTO == null)
                throw new ArgumentNullException(nameof(guestDTO));

            if (!ValidateGuest(guestDTO))
                throw new ArgumentException("Guest validation failed");

            var guest = guestDTO.ToReservationGuest();
            var updatedGuest = _guestRepository.Update(guest);
            return ReservationGuestDTO.FromDomain(updatedGuest);
        }

        public void DeleteGuest(int id)
        {
            _guestRepository.Delete(id);
        }

        public List<ReservationGuestDTO> GetGuestsByReservation(int reservationId)
        {
            var guests = _guestRepository.GetByReservationId(reservationId);
            return guests.Select(g => ReservationGuestDTO.FromDomain(g)).ToList();
        }

        public void RemoveGuestsByReservation(int reservationId)
        {
            _guestRepository.RemoveByReservationId(reservationId);
        }

        public List<ReservationGuestDTO> GetAppearedGuests()
        {
            var guests = _guestRepository.GetAppearedGuests();
            return guests.Select(g => ReservationGuestDTO.FromDomain(g)).ToList();
        }

        public bool MarkGuestAppearance(int guestId, bool hasAppeared, int keyPointJoinedAt = -1)
        {
            return _guestRepository.UpdateAppearanceStatus(guestId, hasAppeared, keyPointJoinedAt);
        }

        public bool ValidateGuest(ReservationGuestDTO guestDTO)
        {
            if (guestDTO == null) return false;

            return guestDTO.IsValidGuest() &&
                   guestDTO.ReservationId > 0;
        }

    }
}