using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Services
{
    public class ForumNotificationService : IForumNotificationService
    {
        private readonly IForumNotificationRepository _notificationRepository;
        private readonly IAccommodationRepository _accommodationRepository;

        public ForumNotificationService(
            IForumNotificationRepository notificationRepository,
            IAccommodationRepository accommodationRepository)
        {
            _notificationRepository = notificationRepository;
            _accommodationRepository = accommodationRepository;
        }

        public void NotifyOwnersAboutNewForum(int forumId, string forumTitle, int locationId, string locationName)
        {
            var ownerIds = GetOwnersWithAccommodationInLocation(locationId);

            foreach (var ownerId in ownerIds)
            {
                var notification = new ForumNotification
                {
                    ForumId = forumId,
                    OwnerId = ownerId,
                    LocationId = locationId,
                    ForumTitle = forumTitle,
                    LocationName = locationName,
                    CreatedDate = DateTime.Now,
                    IsRead = false
                };

                _notificationRepository.Save(notification);
            }
        }

        public List<ForumNotificationDTO> GetUnreadForOwner(int ownerId)
        {
            return _notificationRepository.GetUnreadByOwnerId(ownerId)
                .Select(n => new ForumNotificationDTO(n))
                .ToList();
        }

        public void MarkAsRead(int notificationId)
        {
            var notification = _notificationRepository.GetById(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                _notificationRepository.Update(notification);
            }
        }

        private List<int> GetOwnersWithAccommodationInLocation(int locationId)
        {
            return _accommodationRepository.GetAll()
                .Where(a => a.GeoLocation.Id == locationId)
                .Select(a => a.OwnerId)
                .Distinct()
                .ToList();
        }
    }
}