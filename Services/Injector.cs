using BookingApp.Domain.Interfaces;
using BookingApp.Presentation.ViewModel.Owner;
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
            // Repositories
            _implementations[typeof(IUserRepository)] = new UserRepository();
            _implementations[typeof(IAccommodationRepository)] = new AccommodationRepository();
            _implementations[typeof(IAccommodationImageRepository)] = new AccommodationImageRepository();
            _implementations[typeof(ICommentRepository)] = new CommentRepository();
            _implementations[typeof(ILocationRepository)] = new LocationRepository();
            _implementations[typeof(IGuestReviewRepository)] = new GuestReviewRepository();
            _implementations[typeof(IReservationRepository)] = new ReservationRepository();
            _implementations[typeof(IOccupiedDateRepository)] = new OccupiedDateRepository();
            _implementations[typeof(IAccommodationReviewRepository)] = new AccommodationReviewRepository();
            _implementations[typeof(IRescheduleRequestRepository)] = new RescheduleRequestRepository();

            // Services
            _implementations[typeof(IUserService)] = new UserService((IUserRepository)_implementations[typeof(IUserRepository)]);
            _implementations[typeof(IAccommodationService)] = new AccommodationService(
                (IAccommodationRepository)_implementations[typeof(IAccommodationRepository)],
                (ILocationRepository)_implementations[typeof(ILocationRepository)],
                (IAccommodationImageRepository)_implementations[typeof(IAccommodationImageRepository)]
            );
            _implementations[typeof(IAccommodationImageService)] = new AccommodationImageService((IAccommodationImageRepository)_implementations[typeof(IAccommodationImageRepository)]);
            _implementations[typeof(ICommentService)] = new CommentService((ICommentRepository)_implementations[typeof(ICommentRepository)]);
            _implementations[typeof(ILocationService)] = new LocationService((ILocationRepository)_implementations[typeof(ILocationRepository)]);
            _implementations[typeof(IGuestReviewService)] = new GuestReviewService((IGuestReviewRepository)_implementations[typeof(IGuestReviewRepository)]);
            _implementations[typeof(IReservationService)] = new ReservationService(
                (IReservationRepository)_implementations[typeof(IReservationRepository)],
                (IOccupiedDateRepository)_implementations[typeof(IOccupiedDateRepository)],
                (IAccommodationRepository)_implementations[typeof(IAccommodationRepository)]
            );
            _implementations[typeof(IAccommodationReviewService)] = new AccommodationReviewService((IAccommodationReviewRepository)_implementations[typeof(IAccommodationReviewRepository)]);
            _implementations[typeof(IRescheduleRequestService)] = new RescheduleRequestService(
                (IOccupiedDateRepository)_implementations[typeof(IOccupiedDateRepository)],
                (IRescheduleRequestRepository)_implementations[typeof(IRescheduleRequestRepository)],
                (IAccommodationRepository)_implementations[typeof(IAccommodationRepository)],
                (IReservationRepository)_implementations[typeof(IReservationRepository)]
            );
            _implementations[typeof(IAccommodationFilterService)] = new AccommodationFilterService((IAccommodationRepository)_implementations[typeof(IAccommodationRepository)]);
            _implementations[typeof(IReservationDisplayService)] = new ReservationDisplayService(
                (IReservationRepository)_implementations[typeof(IReservationRepository)],
                (IAccommodationRepository)_implementations[typeof(IAccommodationRepository)],
                (IRescheduleRequestRepository)_implementations[typeof(IRescheduleRequestRepository)],
                (IAccommodationReviewService)_implementations[typeof(IAccommodationReviewService)],
                (IGuestReviewService)_implementations[typeof(IGuestReviewService)]
            );
            _implementations[typeof(INavigationService)] = new NavigationService();
        }

        public static T CreateInstance<T>()
        {
            Type type = typeof(T);
            if (_implementations.ContainsKey(type))
            {
                return (T)_implementations[type];
            }
            throw new ArgumentException($"No implementation found for type {type}");
        }

        public static RegisterAccommodationViewModel CreateRegisterAccommodationViewModel()
        {
            var accommodationService = CreateInstance<IAccommodationService>();
            return new RegisterAccommodationViewModel(accommodationService);
        }

        public static ReviewsViewModel CreateReviewsViewModel()
        {
            var guestReviewService = CreateInstance<IGuestReviewService>();
            var accommodationReviewService = CreateInstance<IAccommodationReviewService>();
            return new ReviewsViewModel(guestReviewService, accommodationReviewService);
        }
        public static ImageGalleryViewModel CreateImageGalleryViewModel(List<string> imagePaths)
        {
            var service = CreateInstance<IAccommodationImageService>();
            return new ImageGalleryViewModel(service, imagePaths);
        }
        public static HomeViewModel CreateHomeViewModel()
        {
            
            return new HomeViewModel();
        }
        public static RequestsViewModel CreateRequestsViewModel()
        {
            var rescheduleRequestService = CreateInstance<IRescheduleRequestService>();
            var accommodationService = CreateInstance<IAccommodationService>();
            var reservationService = CreateInstance<IReservationService>();

            return new RequestsViewModel(rescheduleRequestService, accommodationService, reservationService);
        }
    }
}
