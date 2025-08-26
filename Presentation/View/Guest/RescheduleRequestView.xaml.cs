using BookingApp.Domain;
using BookingApp.Presentation.ViewModel; // Namespace gde je ViewModel
using System.Windows;

namespace BookingApp.Presentation.View.Guest
{
    public partial class RescheduleRequestView : Window
    {
        public RescheduleRequestView(Reservation reservation)
        {
            InitializeComponent();

            // Kreiramo ViewModel i prosleđujemo mu podatke
            var viewModel = new RescheduleRequestViewModel(reservation);

            // Povezujemo View sa ViewModel-om
            DataContext = viewModel;

            // Dajemo ViewModel-u način da zatvori prozor
            viewModel.CloseAction = () => { this.DialogResult = true; this.Close(); };
        }
    }
}