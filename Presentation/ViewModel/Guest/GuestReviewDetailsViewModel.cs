using System;
using System.Windows.Input;
using BookingApp.Presentation.View.Guest;
using BookingApp.Utilities;
using BookingApp.Services.DTO;

namespace BookingApp.Presentation.ViewModel.Guest
{
    public class GuestReviewDetailsViewModel : ViewModelBase
    {
        #region Svojstva za prikaz



        public int CleanlinessRating { get; }
        public int RuleRespectingRating { get; }
        public string Comment { get; }

        #endregion

        #region Komande
        public ICommand OkCommand { get; }
        #endregion

        public Action CloseAction { get; set; }

        public GuestReviewDetailsViewModel(GuestReviewDTO reviewDto)
        {
            if (reviewDto == null)
                throw new ArgumentNullException(nameof(reviewDto), "Review cannot be null.");

            CleanlinessRating = reviewDto.CleanlinessRating;
            RuleRespectingRating = reviewDto.RuleRespectingRating;
            Comment = reviewDto.Comment;

            OkCommand = new RelayCommand(CloseWindow);
        }

        private void CloseWindow(object obj)
        {
            CloseAction?.Invoke();
        }
    }
}