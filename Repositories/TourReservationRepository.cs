using BookingApp.Domain;
using BookingApp.Repositories;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repositories
{
    public class TourReservationRepository
    {
        private const string FilePath = "../../../Resources/Data/tourReservations.csv";
        private readonly Serializer<TourReservation> _serializer;
        private List<TourReservation> _tourReservations;
        private readonly ReservationGuestRepository _guestRepository;
        private readonly TourRepository _tourRepository;
        private readonly UserRepository _userRepository;
        private readonly StartTourTimeRepository _startTourTimeRepository;

        public TourReservationRepository()
        {
            _serializer = new Serializer<TourReservation>();
            _tourReservations = _serializer.FromCSV(FilePath) ?? new List<TourReservation>();
            _guestRepository = new ReservationGuestRepository();
            _tourRepository = new TourRepository();
            _userRepository = new UserRepository();
            _startTourTimeRepository = new StartTourTimeRepository();
            LoadGuests();
            LoadTourDetails();
        }

        private void LoadTourDetails()
        {
            foreach (var reservation in _tourReservations)
            {
                reservation.Tour = _tourRepository.GetById(reservation.TourId);
                reservation.StartTourTime = _startTourTimeRepository.GetById(reservation.StartTourTimeId);
            }
        }

        public List<TourReservation> GetCompletedReservationsByTourist(int touristId)
        {
            return _tourReservations
                .Where(tr => tr.TouristId == touristId && tr.Status == TourReservationStatus.COMPLETED)
                .ToList();
        }

        private void LoadGuests()
        {
            foreach (var reservation in _tourReservations)
            {
                reservation.Guests = _guestRepository.GetByReservationId(reservation.Id);
            }
        }

        
        public int NextId()
        {
            if (_tourReservations.Count < 1)
                return 1;
            return _tourReservations.Max(r => r.Id) + 1;
        }

        public List<TourReservation> GetAll()
        {
            return _tourReservations;
        }

        public List<TourReservation> GetByTouristId(int touristId)
        {
            return _tourReservations.Where(tr => tr.TouristId == touristId).ToList();
        }

        
        public List<TourReservation> GetReservationsByTourist(int touristId)
        {
            return GetByTouristId(touristId);
        }

        public List<TourReservation> GetTodaysReservations()
        {
            var today = DateTime.Today;
            return _tourReservations
                .Where(tr => tr.ReservationDate.Date == today)
                .ToList();
        }



        public List<TourReservation> GetCompletedUnreviewedReservationsByTourist(int touristId)
        {
            var completedReservations = _tourReservations
                .Where(tr => tr.TouristId == touristId && tr.Status == TourReservationStatus.COMPLETED)
                .ToList();

            // Filtriranje već ocenjenih tura
            var reviewRepository = new TourReviewRepository();
            var existingReviews = reviewRepository.GetByTouristId(touristId);

            return completedReservations
                .Where(reservation => !existingReviews.Any(review => review.TourId == reservation.TourId))
                .ToList();
        }
        public TourReservation Add(TourReservation reservation)
        {
            reservation.Id = NextId();
            _tourReservations.Add(reservation);
            _serializer.ToCSV(FilePath, _tourReservations);

            foreach (var guest in reservation.Guests)
            {
                guest.ReservationId = reservation.Id;
                _guestRepository.Add(guest);
            }
            return reservation;
        }

        public void SaveAll()
        {
            _serializer.ToCSV(FilePath, _tourReservations);
        }
    }
}