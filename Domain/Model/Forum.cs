using BookingApp.Serializer;
using System;

namespace BookingApp.Domain.Model
{
    public class Forum : ISerializable
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int LocationId { get; set; }
        public Location Location { get; set; }
        public int CreatorId { get; set; }
        public User Creator { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsClosed { get; set; }

        public Forum() { }

        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            Title = values[1];
            LocationId = int.Parse(values[2]);
            CreatorId = int.Parse(values[3]);
            CreationDate = DateTime.Parse(values[4]);
            IsClosed = bool.Parse(values[5]);
        }

        public string[] ToCSV()
        {
            return new string[] 
            { 
                Id.ToString(),
                Title,
                LocationId.ToString(),
                CreatorId.ToString(),
                CreationDate.ToString("o"),
                IsClosed.ToString() 
            };
        }
    }
}