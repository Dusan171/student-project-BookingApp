using System;
using BookingApp.Serializer;

namespace BookingApp.Model
{
    public class GuestReview : ISerializable
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }

        public int CleanlinessRating { get; set; }
        public int OwnerRating { get; set; }
        public string Comment { get; set; }
        public string ImagePaths { get; set; }
        public DateTime CreatedAt { get; set; }


        public GuestReview() { }

        public GuestReview(int id, int reservationId, int cleanliness, int owner, string comment, string imagePaths)
        {
            Id = id;
            ReservationId = reservationId;
            CleanlinessRating = cleanliness;
            OwnerRating = owner;
            Comment = comment;
            ImagePaths = imagePaths;
            CreatedAt = DateTime.Now;
        }
        public string[] ToCSV()
        {
            return new string[]
            {
                Id.ToString(),
                ReservationId.ToString(),
                CleanlinessRating.ToString(),
                OwnerRating.ToString(),
                Comment,
                ImagePaths,
            };
        }
        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            ReservationId= int.Parse(values[1]);
            CleanlinessRating = int.Parse(values[2]);
            OwnerRating = int.Parse(values[3]);
            Comment = values[4];
            ImagePaths = values[5];
            CreatedAt = DateTime.Parse(values[6]);
        }
    }
}
