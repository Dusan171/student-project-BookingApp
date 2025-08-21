using System.Linq;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain;
using BookingApp.Presentation.View.Guest;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel
{
    public class OwnerReviewDetailsViewModel : ViewModelBase
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

        public OwnerReviewDetailsViewModel(GuestReview review)
        {
            if (review != null)
            {
                // Popunjavamo svojstva na osnovu modela
                CleanlinessRating = review.CleanlinessRating;
                RuleRespectingRating = review.RuleRespectingRating;
                Comment = review.Comment;
            }

            // Inicijalizacija komande
            OkCommand = new RelayCommand(CloseWindow);
        }

        private void CloseWindow(object obj)
        {
            // Zatvaramo prozor koji je trenutno aktivan i pripada ovom tipu
            Application.Current.Windows.OfType<OwnerReviewDetailsView>().FirstOrDefault()?.Close();
        }
    }
}