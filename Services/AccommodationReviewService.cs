using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Domain;
using BookingApp.Domain.Interfaces;

namespace BookingApp.Services
{
    public class AccommodationReviewService : IAccommodationReviewService
    {
        // Sada zavisi SAMO od jednog repozitorijuma!
        private readonly IAccommodationReviewRepository _accommodationReviewRepository;
        private const int DaysToLeaveReview = 5;

        public AccommodationReviewService(IAccommodationReviewRepository accommodationReviewRepository)
        {
            _accommodationReviewRepository = accommodationReviewRepository;
        }

        public bool IsReviewPeriodExpired(Reservation reservation)
        {
            return (DateTime.Now - reservation.EndDate).TotalDays > DaysToLeaveReview;
        }

        public void Create(Reservation reservation, int cleanliness, int ownerRating, string comment, string imagePaths)
        {
            if (IsReviewPeriodExpired(reservation))
            {
                throw new Exception($"You can only leave a review within {DaysToLeaveReview} days after your stay.");
            }

            var review = new AccommodationReview
            {
                ReservationId = reservation.Id,
                CleanlinessRating = cleanliness,
                OwnerRating = ownerRating,
                Comment = comment,
                ImagePaths = imagePaths,
                CreatedAt = DateTime.Now
            };

            _accommodationReviewRepository.Save(review);
        }

        public bool HasGuestRated(int reservationId)
        {
            return _accommodationReviewRepository.HasGuestRated(reservationId);
        }
        public AccommodationReview GetByReservationId(int reservationId)
        {
            // 1. Pozivamo repozitorijum da pronađe SVE recenzije za dati ID rezervacije.
            //    Repozitorijum će verovatno vratiti listu (List<AccommodationReview>).
            var reviewsForReservation = _accommodationReviewRepository.GetByReservationId(reservationId);

            // 2. Pošto znamo da gost može ostaviti samo JEDNU recenziju po rezervaciji,
            //    bezbedno je uzeti prvi (i jedini) element iz liste.
            //    Koristimo FirstOrDefault() da bismo izbegli grešku ako recenzija ne postoji (tada će vratiti null).
            return reviewsForReservation.FirstOrDefault();
        }
    }
}
