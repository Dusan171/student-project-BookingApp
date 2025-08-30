using System;
using System.Collections.Generic;
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
        public bool HasAppeared { get; set; }
        public int KeyPointJoinedAt { get; set; }

        
        public void FromCSV(string[] values)
        {
            if (values.Length < 5)
                throw new ArgumentException("CSV values are missing for TouristAttendance");

            Id = int.Parse(values[0]);
            GuestId = int.Parse(values[1]);
            TourId = int.Parse(values[2]);
            HasAppeared = bool.Parse(values[3]);
            KeyPointJoinedAt = int.Parse(values[4]);
        }
        public string[] ToCSV()
        {
            return new string[]
            {
            Id.ToString(),
            GuestId.ToString(),
            TourId.ToString(),
            HasAppeared.ToString(),
            KeyPointJoinedAt.ToString()
            };
        }
    }
}

