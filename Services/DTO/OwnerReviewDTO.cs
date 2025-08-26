using BookingApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Services.DTO
{
    public class OwnerReviewDTO
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public int CleanlinessRating { get; set; }
        public int OwnerRating { get; set; }
        public string Comment { get; set; }

        public List<string> ImagePaths { get; set; }

        public DateTime CreatedAt { get; set; }

        public OwnerReviewDTO()
        {
            ImagePaths = new List<string>();
        }

        public OwnerReviewDTO(AccommodationReview review)
        {
            Id = review.Id;
            ReservationId = review.ReservationId;
            CleanlinessRating = review.CleanlinessRating;
            OwnerRating = review.OwnerRating;
            Comment = review.Comment;
            CreatedAt = review.CreatedAt;

            ImagePaths = string.IsNullOrWhiteSpace(review.ImagePaths)
                ? new List<string>()
                : review.ImagePaths.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public AccommodationReview ToOwnerReview()
        {
            return new AccommodationReview
            {
                Id = this.Id,
                ReservationId = this.ReservationId,
                CleanlinessRating = this.CleanlinessRating,
                OwnerRating = this.OwnerRating,
                Comment = this.Comment,
                CreatedAt = this.CreatedAt,

                ImagePaths = string.Join(";", this.ImagePaths)
            };
        }
    }
}