using BookingApp.Domain.Interfaces;
using BookingApp.Repositories;
using System;
using System.Collections.Generic;

namespace BookingApp.Services
{
    public class Injector
    {
        private static readonly Dictionary<Type, object> _implementations = new Dictionary<Type, object>();

        static Injector()
        {
            // --- FAZA 1: Kreiranje instanci repozitorijuma ---
            // Kreiramo ih kao lokalne promenljive da bismo ih mogli proslediti servisima.
            IUserRepository userRepository = new UserRepository();
            IAccommodationRepository accommodationRepository = new AccommodationRepository();
            IAccommodationImageRepository accommodationImageRepository = new AccommodationImageRepository();
            ICommentRepository commentRepository = new CommentRepository();
            ILocationRepository locationRepository = new LocationRepository();
            IReservationRepository reservationRepository = new ReservationRepository();
            IOccupiedDateRepository occupiedDateRepository = new OccupiedDateRepository();
            IRescheduleRequestRepository rescheduleRequestRepository = new RescheduleRequestRepository();
            IAccommodationReviewRepository accommodationReviewRepository = new AccommodationReviewRepository();
            IGuestReviewRepository guestReviewRepository = new GuestReviewRepository();

            // --- FAZA 2: Kreiranje instanci servisa, prosleđujući im repozitorijume ---
            IUserService userService = new UserService(userRepository);
            IAccommodationService accommodationService = new AccommodationService(accommodationRepository);
            IAccommodationImageService accommodationImageService = new AccommodationImageService(accommodationImageRepository);
            ICommentService commentService = new CommentService(commentRepository);
            ILocationService locationService = new LocationService(locationRepository);

            // Vaši servisi
            IAccommodationFilterService accommodationFilterService = new AccommodationFilterService(accommodationRepository);
            IReservationService reservationService = new ReservationService(reservationRepository, occupiedDateRepository,accommodationRepository);
            IAccommodationReviewService accommodationReviewService = new AccommodationReviewService(accommodationReviewRepository);
            IGuestReviewService guestReviewService = new GuestReviewService(guestReviewRepository);
            IReservationDisplayService reservationDisplayService = new ReservationDisplayService(reservationRepository, accommodationRepository, rescheduleRequestRepository, accommodationReviewService, guestReviewService);
            IRescheduleRequestService rescheduleRequestService = new RescheduleRequestService(occupiedDateRepository, rescheduleRequestRepository, accommodationRepository, reservationRepository);

            // --- FAZA 3: Popunjavanje rečnika sa već kreiranim instancama ---
            // Repozitorijumi
            _implementations.Add(typeof(IUserRepository), userRepository);
            _implementations.Add(typeof(IAccommodationRepository), accommodationRepository);
            _implementations.Add(typeof(IAccommodationImageRepository), accommodationImageRepository);
            _implementations.Add(typeof(ICommentRepository), commentRepository);
            _implementations.Add(typeof(ILocationRepository), locationRepository);
            _implementations.Add(typeof(IReservationRepository), reservationRepository);
            _implementations.Add(typeof(IOccupiedDateRepository), occupiedDateRepository);
            _implementations.Add(typeof(IRescheduleRequestRepository), rescheduleRequestRepository);
            _implementations.Add(typeof(IAccommodationReviewRepository), accommodationReviewRepository);
            _implementations.Add(typeof(IGuestReviewRepository), guestReviewRepository);

            // Servisi
            _implementations.Add(typeof(IUserService), userService);
            _implementations.Add(typeof(IAccommodationService), accommodationService);
            _implementations.Add(typeof(IAccommodationImageService), accommodationImageService);
            _implementations.Add(typeof(ICommentService), commentService);
            _implementations.Add(typeof(ILocationService), locationService);
            _implementations.Add(typeof(IAccommodationFilterService), accommodationFilterService);
            _implementations.Add(typeof(IReservationService), reservationService);
            _implementations.Add(typeof(IAccommodationReviewService), accommodationReviewService);
            _implementations.Add(typeof(IGuestReviewService), guestReviewService);
            _implementations.Add(typeof(IReservationDisplayService), reservationDisplayService);
            _implementations.Add(typeof(IRescheduleRequestService), rescheduleRequestService);
        }

        public static T CreateInstance<T>()
        {
            Type type = typeof(T);

            if (_implementations.ContainsKey(type))
            {
                return (T)_implementations[type];
            }

            throw new ArgumentException($"No implementation found for type {type} in Injector.");
        }
    }
}
