using BookingApp.Domain.Model;
using BookingApp.Presentation.ViewModel; // Namespace gde je vaš novi ViewModel
using System.Windows;

namespace BookingApp.Presentation.View.Guest
{
    public partial class AccommodationReservationView : Window
    {
        public AccommodationReservationView(Accommodation accommodation)
        {
            InitializeComponent();

            // Kreiramo ViewModel i prosleđujemo mu podatke koji su mu potrebni
            var viewModel = new AccommodationReservationViewModel(accommodation);

            // Povezujemo View sa ViewModel-om
            DataContext = viewModel;

            // Dajemo ViewModel-u način da zatvori prozor, bez da on zna za prozor
            viewModel.CloseAction = new System.Action(this.Close);
        }
    }
}