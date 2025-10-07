using System;
using BookingApp.Serializer;

namespace BookingApp.Domain.Model
{
    public class ForumNotification : ISerializable
    {
        public int Id { get; set; }
        public int ForumId { get; set; }
        public int OwnerId { get; set; }
        public int LocationId { get; set; }
        public string ForumTitle { get; set; }
        public string LocationName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; }

        public ForumNotification() { }

        public string[] ToCSV()
        {
            return new[]
            {
                Id.ToString(),
                ForumId.ToString(),
                OwnerId.ToString(),
                LocationId.ToString(),
                ForumTitle,
                LocationName,
                CreatedDate.ToString("O"),
                IsRead.ToString()
            };
        }

        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            ForumId = int.Parse(values[1]);
            OwnerId = int.Parse(values[2]);
            LocationId = int.Parse(values[3]);
            ForumTitle = values[4];
            LocationName = values[5];
            CreatedDate = DateTime.Parse(values[6]);
            IsRead = bool.Parse(values[7]);
        }
    }
}