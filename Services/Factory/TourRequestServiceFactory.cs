using BookingApp.Domain.Interfaces;
using BookingApp.Services.Validation;
using BookingApp.Services.Management;
using BookingApp.Services.Enrichment;

namespace BookingApp.Services.Factory
{
    public class TourRequestServiceFactory
    {
        public static (TourRequestValidator validator, 
                      TourRequestParticipantManager participantManager, 
                      TourRequestEnricher enricher, 
                      TourRequestStatusManager statusManager) 
            CreateDependencies(
                ITourRequestRepository requestRepository,
                ITourRequestParticipantRepository participantRepository,
                IUserRepository userRepository,
                INotificationService notificationService)
        {
            return (
                new TourRequestValidator(),
                new TourRequestParticipantManager(participantRepository),
                new TourRequestEnricher(participantRepository, userRepository),
                new TourRequestStatusManager(requestRepository, notificationService)
            );
        }
    }
}