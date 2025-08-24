using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain;
using BookingApp.Domain.Interfaces;

namespace BookingApp.Services
{
    public class GuestReviewService : IGuestReviewService
    {
        // JEDINA zavisnost servisa je interfejs repozitorijuma.
        private readonly IGuestReviewRepository _repository;

        // Konstruktor prima implementaciju repozitorijuma.
        public GuestReviewService(IGuestReviewRepository repository)
        {
            _repository = repository;
        }

        // --- Metoda koja je VAMA potrebna za ViewModel ---
        public GuestReview GetReviewForReservation(int reservationId)
        {
            // Servis delegira posao repozitorijumu i obrađuje rezultat.
            // Repozitorijum vraća listu, a servis uzima prvi element.
            return _repository.GetByReservationId(reservationId).FirstOrDefault();
        }


        // --- Metode koje su potrebne VAŠEM KOLEGI (Vlasnik) ---
        // One samo prosleđuju poziv direktno repozitorijumu.

        public List<GuestReview> GetAllReviews()
        {
            return _repository.GetAll();
        }

        public GuestReview AddReview(GuestReview review)
        {
            // Ovde bi mogla da dođe neka poslovna logika PRE čuvanja,
            // npr. provera da li je rok za ocenjivanje prošao.
            // Za sada je direktan poziv u redu.
            return _repository.Save(review);
        }

        public void DeleteReview(GuestReview review)
        {
            _repository.Delete(review);
        }

        public GuestReview UpdateReview(GuestReview review)
        {
            return _repository.Update(review);
        }

        // --- Stara metoda koja ostaje radi kompatibilnosti ---
        public List<GuestReview> GetReviewsByReservation(Reservation reservation)
        {
            // OVO JE ISPRAVKA ZA GREŠKU KOJA VAM SE JAVLJALA
            // Pozivamo metodu repozitorijuma koja prima ID, a ne ceo objekat.
            return _repository.GetByReservationId(reservation.Id);
        }
    }
}