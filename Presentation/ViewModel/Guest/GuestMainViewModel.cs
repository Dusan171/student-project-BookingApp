using BookingApp.Presentation.View.Guest; // Potrebno za otvaranje prozora
using BookingApp.Utilities;
using System.Windows.Input;

namespace BookingApp.Presentation.ViewModel
{
    public class GuestMainViewModel : ViewModelBase
    {
        #region Komande
        public ICommand OpenAccommodationsCommand { get; }
        public ICommand OpenMyReservationsCommand { get; }
        #endregion

        public GuestMainViewModel()
        {
            // Inicijalizacija komandi
            OpenAccommodationsCommand = new RelayCommand(OpenAccommodations);
            OpenMyReservationsCommand = new RelayCommand(OpenMyReservations);
        }

        #region Logika Komandi

        private void OpenAccommodations(object obj)
        {
            var accommodationsWindow = new AccommodationLookup();
            accommodationsWindow.ShowDialog();
        }

        private void OpenMyReservations(object obj)
        {
            var myReservationsWindow = new MyReservationsView();
            myReservationsWindow.ShowDialog();
        }

        #endregion
    }
}