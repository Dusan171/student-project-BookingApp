using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface IGuestReviewService
    {
        // Vraća JEDNU recenziju za datu rezervaciju, ili null ako ne postoji.
        GuestReview GetReviewForReservation(int reservationId);

        // -- ZADRŽAVAMO OSTALE METODE ZA KOLEGU --
        // On će ih koristiti u svom delu aplikacije.
        List<GuestReview> GetAllReviews();
        GuestReview AddReview(GuestReview review);
        void DeleteReview(GuestReview review);
        GuestReview UpdateReview(GuestReview review);

        //STARA METODA (može ostati kao pomoćna)
        List<GuestReview> GetReviewsByReservation(Reservation reservation);
        //List<GuestReview> GetByReservationId(int reservationId);
    }
}
