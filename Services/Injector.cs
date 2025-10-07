using System;
using System.Collections.Generic;
using System.Windows.Navigation;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Interfaces.ServiceInterfaces;
using BookingApp.Presentation.View.Owner;
using BookingApp.Presentation.ViewModel.Owner;
using BookingApp.Presentation.ViewModel.Tourist;
using BookingApp.Repositories;
using BookingApp.Services;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Services
{
    public static class Injector
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
            _implementations[typeof(INotificationRepository)] = new NotificationRepository();
            _implementations[typeof(IKeyPointRepository)] = new KeyPointRepository();
            _implementations[typeof(ITouristAttendanceRepository)] = new TouristAttendanceRepository();

            _implementations[typeof(AccommodationValidationService)] = new AccommodationValidationService();

            // Tour modules
            _implementations[typeof(ITourRepository)] = new TourRepository();
            _implementations[typeof(IStartTourTimeRepository)] = new StartTourTimeRepository();
            _implementations[typeof(ITourReservationRepository)] = new TourReservationRepository();
            _implementations[typeof(IReservationGuestRepository)] = new ReservationGuestRepository();
            _implementations[typeof(ITourReviewRepository)] = new TourReviewRepository();

            // Tour modules - novi
            _implementations[typeof(ITourPresenceRepository)] = new TourPresenceRepository();
            _implementations[typeof(ITourRequestRepository)] = new TourRequestRepository();
            _implementations[typeof(ITourRequestParticipantRepository)] = new TourRequestParticipantRepository();

            // NOVI - Tour Presence Notification moduli (dodati PRE servisa)
            _implementations[typeof(ITourPresenceNotificationRepository)] = new TourPresenceNotificationRepository();

            _implementations[typeof(IForumRepository)] = new ForumRepository();
            _implementations[typeof(IForumCommentRepository)] = new ForumCommentRepository();
            _implementations[typeof(ISystemNotificationRepository)] = new SystemNotificationRepository();

            // ------------------- Services -------------------
            _implementations[typeof(IUserService)] = new UserService(CreateInstance<IUserRepository>());
            _implementations[typeof(IAccommodationService)] = new AccommodationService(
                (IAccommodationRepository)_implementations[typeof(IAccommodationRepository)],
                (ILocationRepository)_implementations[typeof(ILocationRepository)],
                (IAccommodationImageRepository)_implementations[typeof(IAccommodationImageRepository)],
                (AccommodationValidationService)_implementations[typeof(AccommodationValidationService)]
            );
            _implementations[typeof(IAccommodationImageService)] = new AccommodationImageService((IAccommodationImageRepository)_implementations[typeof(IAccommodationImageRepository)]);
            _implementations[typeof(ICommentService)] = new CommentService((ICommentRepository)_implementations[typeof(ICommentRepository)]);
            _implementations[typeof(ILocationService)] = new LocationService((ILocationRepository)_implementations[typeof(ILocationRepository)]);

            _implementations[typeof(IGuestReviewService)] = new GuestReviewService(
                (IGuestReviewRepository)_implementations[typeof(IGuestReviewRepository)]);

            _implementations[typeof(IReservationService)] = new ReservationService(
                (IReservationRepository)_implementations[typeof(IReservationRepository)],
                (IOccupiedDateRepository)_implementations[typeof(IOccupiedDateRepository)],
                (IAccommodationRepository)_implementations[typeof(IAccommodationRepository)],
                (IGuestReviewRepository)_implementations[typeof(IGuestReviewRepository)]
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
                CreateInstance<IRescheduleRequestRepository>(),
                CreateInstance<IAccommodationReviewService>(),
                CreateInstance<IGuestReviewService>()
            );
    
            _implementations[typeof(INotificationService)] = new NotificationService((INotificationRepository)_implementations[typeof(INotificationRepository)], (IReservationService)_implementations[typeof(IReservationService)]);
            _implementations[typeof(RequestsDisplayService)] = new RequestsDisplayService(
                            (IAccommodationService)_implementations[typeof(IAccommodationService)],
                            (IReservationService)_implementations[typeof(IReservationService)]);

            // NOVI - Tour Presence Notification Service (registrovati PRE TourPresenceService)
            _implementations[typeof(ITourPresenceNotificationService)] = new TourPresenceNotificationService(
                CreateInstance<ITourPresenceNotificationRepository>()
            );

            // ------------------- Tour Services - postojeći -------------------
            _implementations[typeof(ITourService)] = new TourService(
                CreateInstance<ITourRepository>(),
                CreateInstance<ILocationRepository>(),
                CreateInstance<IUserRepository>(),
                CreateInstance<ITourReservationRepository>()
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
                CreateInstance<ITourReviewService>(),
                CreateInstance<IReservationGuestRepository>()
            );
            _implementations[typeof(IReservationGuestService)] = new ReservationGuestService(CreateInstance<IReservationGuestRepository>());

            // ------------------- Tour Services - novi (AŽURIRANO) -------------------
            _implementations[typeof(ITourPresenceService)] = new TourPresenceService(
                CreateInstance<ITourPresenceRepository>(),
                CreateInstance<ITourRepository>(),
                CreateInstance<IUserRepository>(),
                CreateInstance<INotificationService>(),
                CreateInstance<ITourReservationRepository>(),
                CreateInstance<IReservationGuestRepository>(),
                CreateInstance<ITourPresenceNotificationService>()
            );

            // DODATO: ITouristAttendanceService
            _implementations[typeof(ITouristAttendanceService)] = new TouristAttendanceService(
                CreateInstance<ITouristAttendanceRepository>()
            );

            _implementations[typeof(ITourRequestService)] = new TourRequestService(
                CreateInstance<ITourRequestRepository>(),
                CreateInstance<ITourRequestParticipantRepository>(),
                CreateInstance<IUserRepository>(),
                CreateInstance<INotificationService>()
            );
            var displayServiceDependencies = new AssemblerDependencies(
                CreateInstance<IUserRepository>(),
                CreateInstance<ILocationRepository>(),
                CreateInstance<ICommentRepository>(),
                CreateInstance<IForumCommentRepository>(),
                CreateInstance<IReservationRepository>(),
                CreateInstance<IAccommodationRepository>()
             );
            _implementations[typeof(IForumDisplayService)] = new ForumDisplayService(displayServiceDependencies);

            _implementations[typeof(IForumService)] = new ForumService(
                CreateInstance<IForumRepository>(),
                CreateInstance<IForumDisplayService>()
            );

            _implementations[typeof(IForumManagementService)] = new ForumManagementService(
                CreateInstance<IForumRepository>(),
                CreateInstance<ICommentRepository>(),
                CreateInstance<IForumCommentRepository>(),
                CreateInstance<ILocationRepository>()
            );
            _implementations[typeof(IReservationCreationService)] = new ReservationCreationService(
                CreateInstance<IReservationRepository>(),
                CreateInstance<IOccupiedDateRepository>(),
                CreateInstance<IAccommodationRepository>()
            );
            _implementations[typeof(IReservationCancellationService)] = new ReservationCancellationService(
                CreateInstance<IReservationRepository>(),
                CreateInstance<IAccommodationRepository>(),
                CreateInstance<IOccupiedDateRepository>(),
                CreateInstance<ISystemNotificationRepository>(),
                CreateInstance<IUserRepository>()
            );
            _implementations[typeof(IAnywhereSearchService)] = new AnywhereSearchService(
                  CreateInstance<IAccommodationRepository>(),
                  CreateInstance<IReservationRepository>()
                );
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

        public static HomeViewModel CreateHomeViewModel()
        {
            var notificationService = CreateInstance<INotificationService>();
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
            var accommodationService = CreateInstance<IAccommodationService>();
            var reservationService = CreateInstance<IReservationService>();
            var requestsDisplayService = new RequestsDisplayService(accommodationService, reservationService);
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

        // ------------------- Tourist ViewModel Factories - novi -------------------
        public static TourPresenceViewModel CreateTourPresenceViewModel(int userId)
        {
            var presenceService = CreateInstance<ITourPresenceService>();
            var tourService = CreateInstance<ITourService>();
            return new TourPresenceViewModel(presenceService, tourService, userId);
        }

        public static TourRequestViewModel CreateTourRequestViewModel(int userId)
        {
            var requestService = CreateInstance<ITourRequestService>();
            return new TourRequestViewModel(requestService, userId);
        }
    }
}