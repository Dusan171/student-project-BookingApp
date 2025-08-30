using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Interfaces.ServiceInterfaces;
using BookingApp.Presentation.ViewModel.Owner;
using BookingApp.Repositories;
using BookingApp.Services;
using BookingApp.Services.DTO;
using BookingApp.Utilities;
using System;
using System.Collections.Generic;
using BookingApp.Presentation.View.Owner;

namespace BookingApp.Services
{
    public static class Injector
    {
        private static readonly Dictionary<Type, object> _implementations = new Dictionary<Type, object>();

        static Injector()
        {
            // ------------------- Repositories -------------------
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

            // Tour modules
            _implementations[typeof(ITourRepository)] = new TourRepository();
            _implementations[typeof(IStartTourTimeRepository)] = new StartTourTimeRepository();
            _implementations[typeof(ITourReservationRepository)] = new TourReservationRepository();
            _implementations[typeof(IReservationGuestRepository)] = new ReservationGuestRepository();
            _implementations[typeof(ITourReviewRepository)] = new TourReviewRepository();

            // ------------------- Services -------------------
            _implementations[typeof(IUserService)] = new UserService(CreateInstance<IUserRepository>());
            _implementations[typeof(IAccommodationService)] = new AccommodationService(
                CreateInstance<IAccommodationRepository>(),
                CreateInstance<ILocationRepository>(),
                CreateInstance<IAccommodationImageRepository>(),
                new AccommodationValidationService()
            );
            _implementations[typeof(IAccommodationImageService)] = new AccommodationImageService(CreateInstance<IAccommodationImageRepository>());
            _implementations[typeof(ICommentService)] = new CommentService(CreateInstance<ICommentRepository>());
            _implementations[typeof(ILocationService)] = new LocationService(CreateInstance<ILocationRepository>());
            _implementations[typeof(IGuestReviewService)] = new GuestReviewService(CreateInstance<IGuestReviewRepository>());
            _implementations[typeof(IReservationService)] = new ReservationService(
                CreateInstance<IReservationRepository>(),
                CreateInstance<IOccupiedDateRepository>(),
                CreateInstance<IAccommodationRepository>(),
                CreateInstance<IGuestReviewRepository>()
            );
            _implementations[typeof(IAccommodationReviewService)] = new AccommodationReviewService(CreateInstance<IAccommodationReviewRepository>());
            _implementations[typeof(IRescheduleRequestService)] = new RescheduleRequestService(
                CreateInstance<IOccupiedDateRepository>(),
                CreateInstance<IRescheduleRequestRepository>(),
                CreateInstance<IAccommodationRepository>(),
                CreateInstance<IReservationRepository>()
            );
            _implementations[typeof(IAccommodationFilterService)] = new AccommodationFilterService(CreateInstance<IAccommodationRepository>());
            _implementations[typeof(IReservationDisplayService)] = new ReservationDisplayService(
                CreateInstance<IReservationRepository>(),
                CreateInstance<IAccommodationRepository>(),
                CreateInstance<IRescheduleRequestService>(),
                CreateInstance<IAccommodationReviewService>(),
                CreateInstance<IGuestReviewService>()
            );
            _implementations[typeof(INavigationService)] = new NavigationService();

            // ------------------- Tour Services -------------------
            _implementations[typeof(ITourService)] = new TourService(
                CreateInstance<ITourRepository>(),
                CreateInstance<ILocationRepository>(),
                CreateInstance<IUserRepository>()
            );
            _implementations[typeof(IStartTourTimeService)] = new StartTourTimeService(CreateInstance<IStartTourTimeRepository>());
            _implementations[typeof(ITourReviewService)] = new TourReviewService(
                CreateInstance<ITourReviewRepository>(),
                CreateInstance<ITourRepository>(),
                CreateInstance<IUserRepository>()
            );
            _implementations[typeof(ITourReservationService)] = new TourReservationService(
                CreateInstance<ITourReservationRepository>(),
                CreateInstance<ITourRepository>(),
                CreateInstance<IStartTourTimeRepository>(),
                CreateInstance<IUserRepository>(),
                CreateInstance<ITourReviewService>()
            );
            _implementations[typeof(IReservationGuestService)] = new ReservationGuestService(CreateInstance<IReservationGuestRepository>());

            // ------------------- Other Services -------------------
            _implementations[typeof(INotificationService)] = new NotificationService(
                CreateInstance<INotificationRepository>(),
                CreateInstance<IReservationService>()
            );
            _implementations[typeof(RequestsDisplayService)] = new RequestsDisplayService(
                CreateInstance<IAccommodationService>(),
                CreateInstance<IReservationService>()
            );
        }

        // ------------------- Helper -------------------
        public static T CreateInstance<T>()
        {
            Type type = typeof(T);
            if (_implementations.ContainsKey(type))
            {
                return (T)_implementations[type];
            }
            throw new ArgumentException($"No implementation found for type {type}");
        }

        // ------------------- ViewModel Factories -------------------
        public static HomeViewModel CreateHomeViewModel()
        {
            var reservationService = CreateInstance<IReservationService>();
            var guestReviewService = CreateInstance<IGuestReviewService>();
            return new HomeViewModel(reservationService, guestReviewService);
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

        public static RequestsViewModel CreateRequestsViewModel()
        {
            var rescheduleRequestService = CreateInstance<IRescheduleRequestService>();
            var requestsDisplayService = CreateInstance<RequestsDisplayService>();
            return new RequestsViewModel(rescheduleRequestService, requestsDisplayService);
        }

        public static RateGuestView CreateRateGuestView(int reservationId)
        {
            var view = new RateGuestView();
            var guestReviewService = CreateInstance<IGuestReviewService>();
            var viewModel = new RateGuestViewModel(guestReviewService, reservationId);
            view.DataContext = viewModel;
            viewModel.CloseAction = view.Close;
            return view;
        }

        public static OwnerDashboardViewModel CreateOwnerDashboardViewModel(Action closeAction)
        {
            var notificationService = CreateInstance<INotificationService>();
            return new OwnerDashboardViewModel(closeAction, notificationService);
        }

        public static UnratedGuestsViewModel CreateUnratedGuestsViewModel()
        {
            var reservationService = CreateInstance<IReservationService>();
            var guestReviewService = CreateInstance<IGuestReviewService>();
            return new UnratedGuestsViewModel(reservationService, guestReviewService);
        }
    }
}
