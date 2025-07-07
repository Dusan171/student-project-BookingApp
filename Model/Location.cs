using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Serializer;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace BookingApp.Model
{
    public class Location : ISerializable
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public Location() { }

        public Location(int id, string city, string country)
        {
            Id = id;
            City = city;
            Country = country;
        }


        public string[] ToCSV()
        {
            string[] csvValues = { Id.ToString(),City,Country};
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            Id=Convert.ToInt32(values[0]);
            City=values[1];
            Country=values[2];  
        }
    }
}
