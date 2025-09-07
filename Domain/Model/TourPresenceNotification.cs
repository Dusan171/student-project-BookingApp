using System;
using System.ComponentModel;
using BookingApp.Serializer;

namespace BookingApp.Domain.Model
{
    public class TourPresenceNotification : ISerializable, INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; } = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string[] ToCSV()
        {
            return new string[]
            {
                Id.ToString(),
                TourId.ToString(),
                UserId.ToString(),
                Message ?? "",
                CreatedAt.ToString("o"),
                IsRead.ToString()
            };
        }

        public void FromCSV(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            TourId = Convert.ToInt32(values[1]);
            UserId = Convert.ToInt32(values[2]);
            Message = values[3];
            CreatedAt = DateTime.Parse(values[4], null, System.Globalization.DateTimeStyles.RoundtripKind);
            IsRead = Convert.ToBoolean(values[5]);
        }
    }
}