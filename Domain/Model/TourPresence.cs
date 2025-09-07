using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using BookingApp.Serializer;

namespace BookingApp.Domain.Model
{
    public class TourPresence : ISerializable
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public int UserId { get; set; }
        public int KeyPointId { get; set; }
        public DateTime JoinedAt { get; set; }
        public bool IsPresent { get; set; }
        public int CurrentKeyPointIndex { get; set; }
        public DateTime LastUpdated { get; set; }

        public TourPresence()
        {
            JoinedAt = DateTime.Now;
            LastUpdated = DateTime.Now;
            CurrentKeyPointIndex = 0;
            IsPresent = false;
        }

        public TourPresence(int id, int tourId, int userId, DateTime joinedAt, bool isPresent, int currentKeyPointIndex, int keyPointId)
        {
            Id = id;
            TourId = tourId;
            UserId = userId;
            JoinedAt = joinedAt;
            IsPresent = isPresent;
            CurrentKeyPointIndex = currentKeyPointIndex;
            LastUpdated = DateTime.Now;
            KeyPointId = keyPointId;
        }

        public string[] ToCSV()
        {
            return new string[]
            {
                Id.ToString(),
                TourId.ToString(),
                UserId.ToString(),
                JoinedAt.ToString("dd-MM-yyyy HH:mm:ss"),
                IsPresent.ToString(),
                CurrentKeyPointIndex.ToString(),
                LastUpdated.ToString("dd-MM-yyyy HH:mm:ss"),
                KeyPointId.ToString(),
            };
        }

        public void FromCSV(string[] values)
        {
            if (values == null || values.Length < 8)
                throw new ArgumentException("Invalid CSV data for TourPresence");

            Id = int.Parse(values[0]);
            TourId = int.Parse(values[1]);
            UserId = int.Parse(values[2]);
            JoinedAt = DateTime.ParseExact(values[3], "dd-MM-yyyy HH:mm:ss", null);
            IsPresent = bool.Parse(values[4]);
            CurrentKeyPointIndex = int.Parse(values[5]);
            LastUpdated = DateTime.ParseExact(values[6], "dd-MM-yyyy HH:mm:ss", null);
            KeyPointId = int.Parse(values[7]);

        }
    }
}
