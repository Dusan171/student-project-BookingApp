using BookingApp.Serializer;
using System;

namespace BookingApp.Domain.Model
{
    public class SystemNotification : ISerializable
    {
        public int Id { get; set; }
        public int UserId { get; set; } 
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime Timestamp { get; set; }

        public SystemNotification() { }

        public string[] ToCSV()
        {
            return new string[] 
            { 
                Id.ToString(), 
                UserId.ToString(), 
                Message, 
                IsRead.ToString(), 
                Timestamp.ToString("o") 
            };
        }

        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            UserId = int.Parse(values[1]);
            Message = values[2];
            IsRead = bool.Parse(values[3]);
            Timestamp = DateTime.Parse(values[4]);
        }
    }
}
