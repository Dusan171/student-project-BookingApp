using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface ITourReservationService
    {
        List<TourReservationDTO> GetAllReservations();
        TourReservationDTO? GetReservationById(int id);
        TourReservationDTO CreateReservation(TourReservationDTO reservationDTO);
        TourReservationDTO UpdateReservation(TourReservationDTO reservationDTO);
        void CancelReservation(int reservationId);
        void CompleteReservation(int reservationId);

        List<TourReservationDTO> GetReservationsByTourist(int touristId);
        List<TourReservationDTO> GetReservationsForTour(int tourId);
        List<TourReservationDTO> GetTodaysReservations();
        List<TourReservationDTO> GetCompletedReservationsByTourist(int touristId);
        List<TourReservationDTO> GetUnreviewedCompletedReservations(int touristId);

        bool CanCancelReservation(int reservationId);
        bool ValidateReservation(TourReservationDTO reservationDTO);
        int GetAvailableSpotsForTour(int tourId);

        List<AlternativeTourDTO> GetAlternativeToursForLocation(int tourId);
        bool IsTourFullyBooked(int tourId);
    }
}