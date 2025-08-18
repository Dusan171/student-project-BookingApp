using System;
using BookingApp.Domain;
using BookingApp.Repositories;

namespace BookingApp.Services
{
    public class ReviewService
    {
        private readonly OwnerReviewRepository _ownerReviewRepository;
        private const int DaysToLeaveReview = 5; //sta???

        public ReviewService(OwnerReviewRepository ownerReviewRepository)
        {
            _ownerReviewRepository = ownerReviewRepository;
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
            var review = new OwnerReview
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
    }
  }