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
using BookingApp.Presentation.ViewModel;
using static BookingApp.Services.PDFAccommodationReportService;
using GalaSoft.MvvmLight.Views;

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
            _implementations[typeof(IForumNotificationRepository)] = new ForumNotificationRepository();

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


           
            _implementations[typeof(IForumRepository)] = new ForumRepository();
            _implementations[typeof(IForumCommentRepository)] = new ForumCommentRepository();
            _implementations[typeof(ISystemNotificationRepository)] = new SystemNotificationRepository();



            _implementations[typeof(ITourPresenceNotificationRepository)] = new TourPresenceNotificationRepository();
            _implementations[typeof(ITourNotificationRepository)] = new TourNotificationRepository();
            _implementations[typeof(IComplexTourRequestRepository)] = new ComplexTourRequestRepository();
            _implementations[typeof(IComplexTourRequestPartRepository)] = new ComplexTourRequestPartRepository();
            _implementations[typeof(IComplexTourRequestParticipantRepository)] = new ComplexTourRequestParticipantRepository();


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

            _implementations[typeof(IReservationService)] = new ReservationService(
                (IReservationRepository)_implementations[typeof(IReservationRepository)],
                (IOccupiedDateRepository)_implementations[typeof(IOccupiedDateRepository)],
                (IAccommodationRepository)_implementations[typeof(IAccommodationRepository)],
                (IGuestReviewRepository)_implementations[typeof(IGuestReviewRepository)]
            );

            _implementations[typeof(IAccommodationReviewService)] = new AccommodationReviewService(CreateInstance<IAccommodationReviewRepository>(), CreateInstance<IAccommodationRepository>(), CreateInstance<IReservationRepository>());
            _implementations[typeof(AccommodationRatingPdfGenerator)] = new AccommodationRatingPdfGenerator();
            _implementations[typeof(IPDFReportService)] = new PDFAccommodationReportService(CreateInstance<IAccommodationReviewService>(), CreateInstance<IAccommodationService>(), CreateInstance<AccommodationRatingPdfGenerator>()
);

            _implementations[typeof(IRescheduleRequestService)] = new RescheduleRequestService(
                CreateInstance<IOccupiedDateRepository>(),
                CreateInstance<IRescheduleRequestRepository>(),
                CreateInstance<IAccommodationRepository>(),
                CreateInstance<IReservationRepository>()
            );

            _implementations[typeof(IAccommodationFilterService)] = new AccommodationFilterService(CreateInstance<IAccommodationRepository>());

            // MOVED: IGuestReviewService registrovan RANIJE
            _implementations[typeof(IGuestReviewService)] = new GuestReviewService(
                (IGuestReviewRepository)_implementations[typeof(IGuestReviewRepository)],
                (IReservationService)_implementations[typeof(IReservationService)],
                (IUserService)_implementations[typeof(IUserService)],
                (IAccommodationService)_implementations[typeof(IAccommodationService)]
            );

            _implementations[typeof(IReservationDisplayService)] = new ReservationDisplayService(
                CreateInstance<IReservationRepository>(),
                CreateInstance<IAccommodationRepository>(),
                CreateInstance<IRescheduleRequestRepository>(),
                CreateInstance<IAccommodationReviewService>(),
                CreateInstance<IGuestReviewService>()
            );

            _implementations[typeof(IAccommodationStatisticsService)] = new AccommodationStatisticsService(
                CreateInstance<IReservationRepository>(),
                CreateInstance<IRescheduleRequestRepository>()
            );

            _implementations[typeof(IAccommodationSummaryService)] = new AccommodationSummaryService(
                    CreateInstance<IReservationRepository>(),
                    CreateInstance<IAccommodationStatisticsService>() // <--- OVA LINIJA JE KLJUČNA IZMENA
            );
            _implementations[typeof(BookingApp.Domain.Interfaces.INavigationService)] = new NavigationService();

            _implementations[typeof(INotificationService)] = new NotificationService((INotificationRepository)_implementations[typeof(INotificationRepository)], (IReservationService)_implementations[typeof(IReservationService)]);

            _implementations[typeof(RequestsDisplayService)] = new RequestsDisplayService(
                (IAccommodationService)_implementations[typeof(IAccommodationService)],
                (IReservationService)_implementations[typeof(IReservationService)]);


            // System Suggestions Service
            _implementations[typeof(ISystemSuggestionsService)] = new SystemSuggestionsService(
                CreateInstance<IAccommodationRepository>(),
                CreateInstance<IReservationRepository>(),
                CreateInstance<ILocationRepository>()
            );
            
            _implementations[typeof(IForumNotificationService)] = new ForumNotificationService(
                CreateInstance<IForumNotificationRepository>(),
                CreateInstance<IAccommodationRepository>()
            );

            _implementations[typeof(IImageFileHandler)] = new ImageFileHandler();

            _implementations[typeof(IAccommodationValidationService)] = new AccommodationValidationService();

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

            _implementations[typeof(ICommentReportRepository)] = new CommentReportRepository();


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

            
            _implementations[typeof(ITouristAttendanceService)] = new TouristAttendanceService(
                CreateInstance<ITouristAttendanceRepository>()
            );

            _implementations[typeof(ITourRequestService)] = new TourRequestService(
                CreateInstance<ITourRequestRepository>(),
                CreateInstance<ITourRequestParticipantRepository>(),
                CreateInstance<IUserRepository>(),
                CreateInstance<INotificationService>()
            );


            _implementations[typeof(IHomeStatisticsService)] = new HomeStatisticsService(
               CreateInstance<IAccommodationService>(),
               CreateInstance<IAccommodationReviewService>(),
               CreateInstance<IReservationService>()
           );


            var displayServiceDependencies = new AssemblerDependencies(
                CreateInstance<IUserRepository>(),
                CreateInstance<ILocationRepository>(),
                CreateInstance<ICommentRepository>(),
                CreateInstance<IForumCommentRepository>(),
                CreateInstance<IReservationRepository>(),
                CreateInstance<IAccommodationRepository>()
                );

            _implementations[typeof(ICommentReportService)] = new CommentReportService(
                CreateInstance<ICommentReportRepository>());
   
            _implementations[typeof(IForumDisplayService)] = new ForumDisplayService(
             displayServiceDependencies,
              CreateInstance<ICommentReportService>() 
);
            _implementations[typeof(IForumService)] = new ForumService(
                CreateInstance<IForumRepository>(),
                CreateInstance<IForumDisplayService>()
            );

            _implementations[typeof(IForumManagementService)] = new ForumManagementService(
                CreateInstance<IForumRepository>(),
                CreateInstance<ICommentRepository>(),
                CreateInstance<IForumCommentRepository>(),
                CreateInstance<ILocationRepository>(),
                CreateInstance<IForumNotificationService>()
            );
            _implementations[typeof(ISuggestionService)] = new SuggestionService(
                CreateInstance<IOccupiedDateRepository>()
            );

            _implementations[typeof(IReservationCreationService)] = new ReservationCreationService(
                CreateInstance<IReservationRepository>(),
                CreateInstance<IOccupiedDateRepository>(),
                CreateInstance<IAccommodationRepository>(),
                CreateInstance<ISuggestionService>()
            );

            _implementations[typeof(IReservationCancellationService)] = new ReservationCancellationService(
                CreateInstance<IReservationRepository>(),
                CreateInstance<IAccommodationRepository>(),
                CreateInstance<IOccupiedDateRepository>(),
                CreateInstance<ISystemNotificationRepository>(),
                CreateInstance<IUserRepository>()
            );

            _implementations[typeof(IOwnerForumService)] = new OwnerForumService(
                CreateInstance<IAccommodationRepository>(),
                CreateInstance<IForumManagementService>(),
                CreateInstance<IForumRepository>(),
                CreateInstance<ILocationRepository>()
            );


            _implementations[typeof(ITourRequestStatisticsService)] = new TourRequestStatisticsService(
                CreateInstance<ITourRequestRepository>()
            );

            _implementations[typeof(ITourNotificationService)] = new TourNotificationService(
                CreateInstance<ITourNotificationRepository>(),
                CreateInstance<ITourRequestRepository>(),
                CreateInstance<ITourRepository>(),
                CreateInstance<IUserRepository>()
            );

            _implementations[typeof(IComplexTourRequestService)] = new ComplexTourRequestService(
                CreateInstance<IComplexTourRequestRepository>(),
                CreateInstance<IComplexTourRequestPartRepository>(),
                CreateInstance<IComplexTourRequestParticipantRepository>()
            );
            _implementations[typeof(IAnywhereSearchService)] = new AnywhereSearchService(
                CreateInstance<IAccommodationRepository>(),
                CreateInstance<IReservationRepository>()
            );
            _implementations[typeof(IReservationOrchestratorService)] = new ReservationOrchestratorService(
                CreateInstance<IReservationCreationService>()
            );
            _implementations[typeof(IReportGenerationService)] = new ReportGenerationService();

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
            var statisticsService = CreateInstance<IHomeStatisticsService>();
            var userService = CreateInstance<IUserService>();
            return new HomeViewModel(reservationService, guestReviewService, statisticsService, userService);
        }

        public static RegisterAccommodationViewModel CreateRegisterAccommodationViewModel(Action navigateBack = null)
        {
            var accommodationService = CreateInstance<IAccommodationService>();
            var imageFileHandler = CreateInstance<IImageFileHandler>();
            var validationService = CreateInstance<IAccommodationValidationService>();

            return new RegisterAccommodationViewModel(accommodationService, imageFileHandler,validationService, navigateBack);
        }

        public static ReviewsViewModel CreateReviewsViewModel()
        {
            var guestReviewService = CreateInstance<IGuestReviewService>();
            var accommodationReviewService = CreateInstance<IAccommodationReviewService>();
            var reservationService = CreateInstance<IReservationService>();
            var accommodationService = CreateInstance<IAccommodationService>();
            var userService = CreateInstance<IUserService>();

            return new ReviewsViewModel(
                guestReviewService,
                accommodationReviewService,
                reservationService,
                accommodationService,
                userService);
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
            var forumNotificationService = CreateInstance<IForumNotificationService>(); // DODAJ
            return new OwnerDashboardViewModel(closeAction, notificationService, forumNotificationService); // DODAJ parametar
        }

        public static UnratedGuestsViewModel CreateUnratedGuestsViewModel(Action goBackAction)
        {
            var reservationService = CreateInstance<IReservationService>();
            var guestReviewService = CreateInstance<IGuestReviewService>();
            return new UnratedGuestsViewModel(reservationService, guestReviewService, goBackAction);
        }

        public static AccommodationsViewModel CreateAccommodationsViewModel(Action goBackAction, Action navigateToAddAction)
        {
            var accommodationService = CreateInstance<IAccommodationService>();
            var imageService = CreateInstance<IAccommodationImageService>();
            var userService = CreateInstance<IUserService>();
            return new AccommodationsViewModel(accommodationService, imageService, userService, goBackAction, navigateToAddAction); // DODAJ imageService
        }

        // ------------------- Tourist ViewModel Factories - novi -------------------
        public static TourPresenceViewModel CreateTourPresenceViewModel(int userId)
        {
            var presenceService = CreateInstance<ITourPresenceService>();
            var tourService = CreateInstance<ITourService>();
            return new TourPresenceViewModel(userId);
        }

        public static TourRequestViewModel CreateTourRequestViewModel(int userId)
        {
            var requestService = CreateInstance<ITourRequestService>();
            return new TourRequestViewModel(requestService, userId);
        }


        public static StatisticViewModel CreateStatisticViewModel()
        {
            return new StatisticViewModel(
                CreateInstance<IAccommodationService>(),
                CreateInstance<IAccommodationStatisticsService>(),
                CreateInstance<IAccommodationSummaryService>(),
                CreateInstance<IUserService>(),
                CreateInstance<IPDFReportService>(),
                CreateInstance<IAccommodationReviewService>()
            );
        }

        // NOVO: Factory za PDFSettingsViewModel
        public static PDFSettingsViewModel CreatePDFSettingsViewModel(Action closeWindowAction)
        {
            var pdfReportService = CreateInstance<IPDFReportService>();
            var accommodationService = CreateInstance<IAccommodationService>();
            var accommodationReviewService = CreateInstance<IAccommodationReviewService>();
            return new PDFSettingsViewModel(closeWindowAction, pdfReportService, accommodationService, accommodationReviewService);
        }

        public static ForumViewModel CreateForumViewModel()
        {
            return new ForumViewModel(CreateInstance<IForumService>());
        }

        public static ForumCommentsViewModel CreateForumCommentsViewModel(int forumId)
        {
            return new ForumCommentsViewModel(
                CreateInstance<IForumService>(),
                CreateInstance<IOwnerForumService>(),
                CreateInstance<ICommentReportService>(), 
                forumId
            );
        }

        public static SystemSuggestionsViewModel CreateSuggestionsViewModel(int ownerId, Action navigateBack = null, Action<HighDemandLocationDTO> navigateToAddAccommodation = null)
        {
            var suggestionsService = CreateInstance<ISystemSuggestionsService>();
            var accommodationService = CreateInstance<IAccommodationService>();

            return new SystemSuggestionsViewModel(
                suggestionsService,
                accommodationService,
                ownerId,
                navigateBack,
                navigateToAddAccommodation);
        }

            public static ComplexTourRequestViewModel CreateComplexTourRequestViewModel(int userId)
        {
            var requestService = CreateInstance<IComplexTourRequestService>();
            return new ComplexTourRequestViewModel(requestService, userId);

        }
    }
}
