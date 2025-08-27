using BookingApp.Domain;
using BookingApp.Domain.Model;
using BookingApp.Presentation.ViewModel; // Namespace gde je vaš novi ViewModel
using System.Windows;

namespace BookingApp.Presentation.View.Guest
{
    public partial class AccommodationReviewView : Window
    {
        public AccommodationReviewView(Reservation reservation)
        {
            InitializeComponent();

            // Kreiramo ViewModel i prosleđujemo mu podatke
            var viewModel = new AccommodationReviewViewModel(reservation);

            // Povezujemo View sa ViewModel-om
            DataContext = viewModel;

            // Dajemo ViewModel-u način da zatvori prozor
            viewModel.CloseAction = new System.Action(() => this.DialogResult = true);
            viewModel.CloseAction += new System.Action(this.Close);
        }
    }
}
