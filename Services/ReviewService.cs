using System;
using System.Linq;
using BookingApp.Domain;
using BookingApp.Domain.Interfaces;
using BookingApp.Repositories;

namespace BookingApp.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IAccommodationReviewRepository _ownerReviewRepository;
        private readonly IGuestReviewRepository _guestReviewRepository;
        private const int DaysToLeaveReview = 5; //sta???

        public ReviewService(IAccommodationReviewRepository ownerReviewRepository, IGuestReviewRepository guestReviewRepository)
        {
            _ownerReviewRepository = ownerReviewRepository;
            _guestReviewRepository = guestReviewRepository;
        }
        //provjera da li je rok za ostavljenje recenzije vlasniku prosao
        public bool IsReviewPeriodExpired(Reservation reservation)
        {
            return (DateTime.Now - reservation.EndDate).TotalDays > DaysToLeaveReview; ;
        }
        //kreira se i cuva recenzija vlasnika
        public void CreateOwnerReview(Reservation reservation, int cleanliness, int ownerRating, string comment, string imagePaths)
        {
            // 1. Provera poslovnih pravila
            if (IsReviewPeriodExpired(reservation))
            {
                throw new Exception($"You can only leave a review within {DaysToLeaveReview} days after your stay.");
            }

            // 2. Kreiranje objekta
            var review = new AccommodationReview
            {
                // ID se sada dodeljuje unutar Save metode repozitorijuma
                ReservationId = reservation.Id,
                CleanlinessRating = cleanliness,
                OwnerRating = ownerRating,
                Comment = comment,
                ImagePaths = imagePaths,
                CreatedAt = DateTime.Now
            };

            // 3. Čuvanje
            _ownerReviewRepository.Save(review);
        }
        public bool HasGuestRated(int reservationId)
        {
            return _ownerReviewRepository.HasGuestRated(reservationId);
        }
       /* public GuestReview GetReviewFromOwner(Reservation reservation)
        {
            // Pretpostavka da GuestReviewRepository ima metodu GetByReservationId
            return _guestReviewRepository.GetByReservationId(reservation).FirstOrDefault();
        }*/
    }
  }