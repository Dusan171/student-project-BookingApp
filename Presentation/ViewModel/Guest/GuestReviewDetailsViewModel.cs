using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain;
using BookingApp.Presentation.View.Guest;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel
{
    public class GuestReviewDetailsViewModel : ViewModelBase
    {
        #region Svojstva za prikaz

        // Umesto da direktno izlažemo model, izlažemo samo podatke koji su nam potrebni.
        // Ovo je primer gde ViewModel deluje i kao DTO.

        public int CleanlinessRating { get; }
        public int RuleRespectingRating { get; }
        public string Comment { get; }

        #endregion

        #region Komande
        public ICommand OkCommand { get; }
        #endregion

        public Action CloseAction { get; set; }

        public GuestReviewDetailsViewModel(GuestReview review)
        {
            if (review == null)
                throw new ArgumentNullException(nameof(review), "Review cannot be null.");

            CleanlinessRating = review.CleanlinessRating;
            RuleRespectingRating = review.RuleRespectingRating;
            Comment = review.Comment;

            OkCommand = new RelayCommand(CloseWindow);
        }

        private void CloseWindow(object obj)
        {
            CloseAction?.Invoke();
        }
    }
}