using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTOs;
using BookingApp.Utilities;

namespace BookingApp.Services
{
    public class ReservationDisplayService : IReservationDisplayService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IRescheduleRequestRepository _rescheduleRequestRepository;
        private readonly IAccommodationReviewService _accommodationReviewService;
        private readonly IGuestReviewService _guestReviewService;

        public ReservationDisplayService(IReservationRepository reservationRepository, IAccommodationRepository accommodationRepository, IRescheduleRequestRepository rescheduleRequestRepository, IAccommodationReviewService accommodationReviewService, IGuestReviewService guestReviewService)
        {
            _reservationRepository = reservationRepository;
            _accommodationRepository = accommodationRepository;
            _rescheduleRequestRepository = rescheduleRequestRepository;
            _accommodationReviewService = accommodationReviewService;
            _guestReviewService = guestReviewService;
        }

        //sva ova logika je bila u View a to ne smije
        public List<ReservationDetailsDTO> GetReservationsForGuest(int guestId)
        {
            // 1. Dobavljamo sve neophodne podatke na početku
            var allAccommodations = _accommodationRepository.GetAll();
            var myReservations = _reservationRepository.GetByGuestId(guestId)
                                                      .OrderByDescending(r => r.StartDate)
                                                      .ToList();

            // Inicijalizujemo praznu listu DTO objekata koju ćemo vratiti
            var dtoList = new List<ReservationDetailsDTO>();

            // 2. Prolazimo kroz svaku rezervaciju pojedinačno
            foreach (var reservation in myReservations)
            {
                // Dobavljamo dodatne informacije za svaku rezervaciju
                var accommodation = allAccommodations.FirstOrDefault(a => a.Id == reservation.AccommodationId);
                var request = _rescheduleRequestRepository.GetByReservationId(reservation.Id);

                // 3. Kreiramo osnovni DTO objekat sa podacima za prikaz
                var dto = new ReservationDetailsDTO
                {
                    ReservationId = reservation.Id,
                    StartDate = reservation.StartDate,
                    EndDate = reservation.EndDate,
                    GuestsNumber = reservation.GuestsNumber,
                    AccommodationName = accommodation?.Name ?? "N/A", // "??" osigurava da program ne pukne ako smeštaj nije nađen
                    RequestStatusText = request?.Status.ToString() ?? "Not requested",
                    OwnerComment = request?.OwnerComment ?? "",
                    OriginalReservation = reservation
                };

                // 4. Implementiramo POSLOVNU LOGIKU za omogućavanje/onemogućavanje dugmića

                // --- Logika za dugme "Reschedule" ---
                bool hasPendingRequest = (request != null && request.Status == RequestStatus.Pending);
                // Uslov: Rezervacija još nije počela I ne postoji već zahtev koji čeka odgovor.
                dto.IsRescheduleEnabled = reservation.StartDate > DateTime.Now && !hasPendingRequest;


                // --- Logika za dugme "Rate" ---
                // Uslov 1: Rezervacija je završena (danas je posle datuma završetka).
                bool isReservationFinished = reservation.EndDate < DateTime.Now;
                // Uslov 2: Nije prošlo više od 5 dana od završetka rezervacije.
                bool isWithinReviewPeriod = (DateTime.Now - reservation.EndDate).TotalDays <= 5;
                // Uslov 3: Gost još uvek nije ocenio ovu rezervaciju.
                //           Ovo zahteva poziv servisa za recenzije.
                bool hasGuestAlreadyRated = _accommodationReviewService.HasGuestRated(reservation.Id);

                // Finalni uslov za "Rate": Sva tri uslova moraju biti ispunjena.
                dto.IsRatingEnabled = isReservationFinished && isWithinReviewPeriod && !hasGuestAlreadyRated;


                // --- Logika za dugme "View Review" ---
                // Uslov 1: Gost je već ocenio vlasnika/smeštaj (koristimo rezultat od malopre).
                // (hasGuestAlreadyRated je već izračunat)
                // Uslov 2: Vlasnik je ocenio gosta (proveravamo da li recenzija uopšte postoji).
                //           Ovo zahteva poziv drugog servisa za recenzije.
                var reviewFromOwner = _guestReviewService.GetReviewForReservation(reservation.Id);
                bool hasOwnerRatedGuest = reviewFromOwner != null;

                // Finalni uslov za "View Review": Oba uslova moraju biti ispunjena.
                dto.IsGuestReviewVisible = hasGuestAlreadyRated && hasOwnerRatedGuest;


                // 5. Dodajemo kompletno popunjen DTO u listu
                dtoList.Add(dto);
            }

            // 6. Vraćamo listu spremnu za prikaz u ViewModel-u
            return dtoList;
        }
    }
}
