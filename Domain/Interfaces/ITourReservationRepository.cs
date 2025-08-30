using BookingApp.Domain.Model;
using System;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface ITourReservationRepository
    {
        List<TourReservation> GetAll();
        TourReservation? GetById(int id);
        TourReservation Add(TourReservation reservation);
        TourReservation Update(TourReservation reservation);
        void Delete(int id);
        void SaveAll();
        int GetNextId();

        List<TourReservation> GetByTouristId(int touristId);
        List<TourReservation> GetByTourId(int tourId);
        List<TourReservation> GetByStatus(TourReservationStatus status);
        List<TourReservation> GetByDateRange(DateTime startDate, DateTime endDate);
        List<TourReservation> GetTodaysReservations();
        List<TourReservation> GetCompletedReservationsByTourist(int touristId);

       
        List<TourReservation> GetReservationsByTourist(int touristId);
        List<TourReservation> GetCompletedUnreviewedReservationsByTourist(int touristId);
    }
}