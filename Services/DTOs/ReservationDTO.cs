using BookingApp.Domain;
using System;

namespace BookingApp.Services.DTOs
{
    public class ReservationDTO
    {
        public int Id { get; set; }
        public int AccommodationId { get; set; }
        public int GuestId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int GuestsNumber { get; set; }
        public string Status { get; set; }

        public ReservationDTO() { }

        public ReservationDTO(Reservation reservation)
        {
            Id = reservation.Id;
            AccommodationId = reservation.AccommodationId;
            GuestId = reservation.GuestId;
            StartDate = reservation.StartDate;
            EndDate = reservation.EndDate;
            GuestsNumber = reservation.GuestsNumber;
            Status = reservation.Status.ToString();
        }

        public Reservation ToReservation()
        {
            Enum.TryParse<ReservationStatus>(Status, out var status);
            return new Reservation
            {
                Id = this.Id,
                AccommodationId = this.AccommodationId,
                GuestId = this.GuestId,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                GuestsNumber = this.GuestsNumber,
                Status = status
            };
        }
    }
}