using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Serializer;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel;

namespace BookingApp.Domain.Model
{
    public class Location : ISerializable, INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public Location() { }

        public Location(int id, string city, string country)
        {
            Id = id;
            City = city ?? string.Empty;
            Country = country ?? string.Empty;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public Location ToLocation()
        {
            return new Location(Id, City, Country);
        }

        public override string ToString()
        {
            return $"{City}, {Country}";
        }

        public string[] ToCSV()
        {
            string[] csvValues = { Id.ToString(), City ?? string.Empty, Country ?? string.Empty };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            if (values == null || values.Length < 3)
            {
                throw new ArgumentException("Invalid CSV data for Location");
            }

            Id = Convert.ToInt32(values[0]);
            City = values[1] ?? string.Empty;
            Country = values[2] ?? string.Empty;
        }
    }
}