using BookingApp.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services.DTO
{
    public class GuestReviewDTO : INotifyPropertyChanged

    {
 
        private int _id;
        private int _reservationId;
        private int _cleanlinessRating;
        private int _ruleRespectingRating;
        private string _comment;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
       
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public int ReservationId
        {
            get => _reservationId;
            set
            {
                if (_reservationId != value)
                {
                    _reservationId = value;
                    OnPropertyChanged();
                }
            }
        }

        public int CleanlinessRating
        {
            get => _cleanlinessRating;
            set
            {
                if (_cleanlinessRating != value)
                {
                    _cleanlinessRating = value;
                    OnPropertyChanged();
                }
            }
        }

        public int RuleRespectingRating
        {
            get => _ruleRespectingRating;
            set
            {
                if (_ruleRespectingRating != value)
                {
                    _ruleRespectingRating = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Comment
        {
            get => _comment;
            set
            {
                if (_comment != value)
                {
                    _comment = value;
                    OnPropertyChanged();
                }
            }
        }

     
        public GuestReviewDTO() { }

        public GuestReviewDTO(GuestReview r)
        {
            _id = r.Id;
            _reservationId = r.ReservationId;
            _cleanlinessRating = r.CleanlinessRating;
            _ruleRespectingRating = r.RuleRespectingRating;
            _comment = r.Comment;
        }

        public GuestReviewDTO FromModel(GuestReview model)
        {
            return new GuestReviewDTO
            {
                Id = model.Id,
                ReservationId = model.ReservationId,
                CleanlinessRating = model.CleanlinessRating,
                RuleRespectingRating = model.RuleRespectingRating,
                Comment = model.Comment
            };
        }


        public GuestReview ToGuestReview()
        {
            return new GuestReview
            {
                Id = this.Id,
                ReservationId = this.ReservationId,
                CleanlinessRating = this.CleanlinessRating,
                RuleRespectingRating = this.RuleRespectingRating,
                Comment = this.Comment
            };
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

