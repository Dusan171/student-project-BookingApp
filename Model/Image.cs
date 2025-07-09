using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Serializer;
using BookingApp.Model;


namespace BookingApp.Model
{
    public class Image : ISerializable
    {
        public int Id { get; set; }
        public string Path { get; set; }

        public Image()
        {
        }

        public Image(int id, string path)
        {
            Id = id;
            Path = path;
        }

        public string[] ToCSV()
        {
            return new string[] { $"{Id},{Path}" };
        }

        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            Path = values[1];
        }
    }
}
