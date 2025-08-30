using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using System.Xml.Linq;
using BookingApp.Serializer;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel;

namespace BookingApp.Domain.Model
{
    public enum AccommodationType { APARTMENT, HOUSE, COTTAGE }
    public class Accommodation : ISerializable, INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Location GeoLocation { get; set; }
        public AccommodationType Type { get; set; }
        public int? MaxGuests { get; set; }
        public int? MinReservationDays { get; set; }
        public int CancellationDeadlineDays { get; set; }
        public List<AccommodationImage> Images { get; set; }
        public Accommodation() { CancellationDeadlineDays = 1; Images = new List<AccommodationImage>(); }
        public Accommodation(int id, string name, Location geolocation, AccommodationType type, int maxguests, int minreservationdays, int cancellationdeadlinedays = 1)
        {
            Id = id;
            Name = name;
            GeoLocation = geolocation;
            Type = type;
            MaxGuests = maxguests;
            MinReservationDays = minreservationdays;
            CancellationDeadlineDays = cancellationdeadlinedays;
            Images = new List<AccommodationImage>();
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public string[] ToCSV()
        {
            string[] csvValues = {
                Id.ToString(),
                Name,
                GeoLocation.Id.ToString(),
                GeoLocation.City,
                GeoLocation.Country,
                Type.ToString(),
                MaxGuests.ToString(),
                MinReservationDays.ToString(),
                CancellationDeadlineDays.ToString(),

            };
            return csvValues;
        }
        public void FromCSV(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Name = values[1];
            GeoLocation = new Location
            {
                Id = int.Parse(values[2]),
                City = values[3],
                Country = values[4]
            };
            Type = Enum.Parse<AccommodationType>(values[5]);
            MaxGuests = int.Parse(values[6]);
            MinReservationDays = int.Parse(values[7]);
            CancellationDeadlineDays = int.Parse(values[8]);
        }
    }
}
