using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Serializer;
namespace BookingApp.Domain
{
    public class Notification : BookingApp.Serializer.ISerializable, INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public int GuestId { get; set; }
        public DateTime Deadline { get; set; }
        public bool IsRead { get; set; } = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string[] ToCSV()
        {
            string[] csvValues = {
            Id.ToString(),
            ReservationId.ToString(),
            GuestId.ToString(),
            Deadline.ToString("o"), // ISO 8601 format da se datum lepo upiše
            IsRead.ToString()
         };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            ReservationId = Convert.ToInt32(values[1]);
            GuestId = Convert.ToInt32(values[2]);
            Deadline = DateTime.Parse(values[3], null, System.Globalization.DateTimeStyles.RoundtripKind);
            IsRead = Convert.ToBoolean(values[4]);
        }

    }
}
