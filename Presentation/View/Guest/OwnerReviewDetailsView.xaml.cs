using BookingApp.Domain;
using BookingApp.Presentation.ViewModel; // Namespace gde je ViewModel
using System.Windows;

namespace BookingApp.Presentation.View.Guest
{
    public partial class OwnerReviewDetailsView : Window
    {
        public OwnerReviewDetailsView(GuestReview review)
        {
            InitializeComponent();
            // Kreiramo ViewModel, prosleđujemo mu podatke i postavljamo ga kao DataContext
            DataContext = new OwnerReviewDetailsViewModel(review);
        }
    }
}
