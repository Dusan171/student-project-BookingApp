using BookingApp.Domain.Model;
using System;

namespace BookingApp.Services.DTO
{
    public class ForumNotificationDTO
    {
        public int Id { get; set; }
        public int ForumId { get; set; }
        public int OwnerId { get; set; }
        public int LocationId { get; set; }
        public string ForumTitle { get; set; }
        public string LocationName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; }

        public ForumNotificationDTO() { }

        public ForumNotificationDTO(ForumNotification notification)
        {
            Id = notification.Id;
            ForumId = notification.ForumId;
            OwnerId = notification.OwnerId;
            LocationId = notification.LocationId;
            ForumTitle = notification.ForumTitle;
            LocationName = notification.LocationName;
            CreatedDate = notification.CreatedDate;
            IsRead = notification.IsRead;
        }

        public ForumNotification ToForumNotification()
        {
            return new ForumNotification
            {
                Id = Id,
                ForumId = ForumId,
                OwnerId = OwnerId,
                LocationId = LocationId,
                ForumTitle = ForumTitle,
                LocationName = LocationName,
                CreatedDate = CreatedDate,
                IsRead = IsRead
            };
        }
    }
}