using System;
using BookingApp.Serializer;

namespace BookingApp.Domain
{
    public class AccommodationReview : ISerializable
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public int CleanlinessRating { get; set; }
        public int OwnerRating { get; set; }
        public string Comment { get; set; }
        public string ImagePaths { get; set; }
        public DateTime CreatedAt { get; set; }

        public AccommodationReview() { }

        public AccommodationReview(int id, int reservationId, int cleanliness, int owner, string comment, string imagePaths)
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
                Comment ?? string.Empty,
                ImagePaths ?? string.Empty,
                CreatedAt.ToString("o")
            };
        }
        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            ReservationId = int.Parse(values[1]);
            CleanlinessRating = int.Parse(values[2]);
            OwnerRating = int.Parse(values[3]);

            Comment = values.Length > 4 ? values[4] : string.Empty;

            ImagePaths = values.Length > 5 ? values[5] : string.Empty;

            CreatedAt = values.Length > 6 ? DateTime.Parse(values[6]) : DateTime.Now;
        }
        //nigdje se ne koristi
        public int ImageCount
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ImagePaths))
                    return 0;

                return ImagePaths.Split(';', StringSplitOptions.RemoveEmptyEntries).Length;
            }
        }
    }
}