using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Serializer;


namespace BookingApp.Domain.Model
{
    public class Images : ISerializable
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string FullPath => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path);

        public Images()
        {
        }

        public Images(int id, string path)
        {
            Id = id;
            Path = path;
        }

        public string[] ToCSV()
        {
            return new string[] { $"{Id}|{Path}" };
        }

        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            Path = values[1];
        }
    }
}