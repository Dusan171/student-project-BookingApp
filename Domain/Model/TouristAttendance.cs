using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Serializer;

namespace BookingApp.Domain.Model
{
    public class TouristAttendance: ISerializable
    {
        public int Id { get; set; }
        public int GuestId { get; set; }
        public int TourId { get; set; }
        public DateTime StartTourTime { get; set; }
        public bool HasAppeared { get; set; }
        public int KeyPointJoinedAt { get; set; }

        public TouristAttendance() { }
        public TouristAttendance(int id, int guestId, int tourId, DateTime startTourTime, bool hasAppeared, int keyPointJoinedAt)
        {
            Id = id;
            GuestId = guestId;
            TourId = tourId;
            StartTourTime = startTourTime;
            HasAppeared = hasAppeared;
            KeyPointJoinedAt = keyPointJoinedAt;
        }

        public void FromCSV(string[] values)
        {
            if (values.Length < 6)
                throw new ArgumentException("CSV values are missing for TouristAttendance");

            Id = int.Parse(values[0]);
            GuestId = int.Parse(values[1]);
            TourId = int.Parse(values[2]);

            // Pobrinuti se da booleans budu na pravom mestu
            HasAppeared = bool.Parse(values[3]);
            KeyPointJoinedAt = int.Parse(values[4]);

            // Parse DateTime sa poznatim formatom i invariant culture
            StartTourTime = DateTime.ParseExact(values[5], "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
        }
        public string[] ToCSV()
        {
            return new string[]
            {
        Id.ToString(),
        GuestId.ToString(),
        TourId.ToString(),
        HasAppeared.ToString(),
        KeyPointJoinedAt.ToString(),
        StartTourTime.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
            };
        }
    }
}

