using System;
using System.ComponentModel;
using BookingApp.Serializer;

namespace BookingApp.Domain
{
    public class GuestReview : ISerializable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public int Id { get; set; }
        public int ReservationId { get; set; }
        public int CleanlinessRating { get; set; }
        public int RuleRespectingRating { get; set; }
        public string Comment { get; set; }
        public GuestReview() { }
        public GuestReview(int id, int reservationId, int cleanlinessRating, int ruleRespectingRating, string comment)
        {
            Id = id;
            ReservationId = reservationId;
            CleanlinessRating = cleanlinessRating;
            RuleRespectingRating = ruleRespectingRating;
            Comment = comment;
        }

        public void FromCSV(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            ReservationId = Convert.ToInt32(values[1]);
            CleanlinessRating = Convert.ToInt32(values[2]); ;
            RuleRespectingRating = Convert.ToInt32(values[3]);
            Comment = values[4];
        }

        public string[] ToCSV()
        {
            string[] csvValues =
            {
                Id.ToString(),
                ReservationId.ToString(),
                CleanlinessRating.ToString(),

                RuleRespectingRating.ToString(),
                Comment
            };
            return csvValues;
        }

        public bool IsValid(out string errorMessage)
        {
            if (CleanlinessRating < 1 || CleanlinessRating > 5)
            {
                errorMessage = "Cleanliness rating must be between 1 and 5.";
                return false;
            }

            if (RuleRespectingRating < 1 || RuleRespectingRating > 5)
            {
                errorMessage = "Rule respecting rating must be between 1 and 5.";
                return false;
            }

            errorMessage = null;
            return true;
        }


    }
}
