using BookingApp.Presentation.View.Guest; // Potrebno za otvaranje prozora
using BookingApp.Utilities;
using BookingApp.Domain.Interfaces;
using System.Windows.Input;
using BookingApp.Services;

namespace BookingApp.Presentation.ViewModel
{
    public class GuestMainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        #region Komande
        public ICommand OpenAccommodationsCommand { get; }
        public ICommand OpenMyReservationsCommand { get; }
        #endregion

        public GuestMainViewModel()
        {
            _navigationService = Injector.CreateInstance<INavigationService>();
            // Inicijalizacija komandi
            OpenAccommodationsCommand = new RelayCommand(OpenAccommodations);
            OpenMyReservationsCommand = new RelayCommand(OpenMyReservations);
        }

        #region Logika Komandi

        private void OpenAccommodations(object obj)
        {
            _navigationService.ShowAccommodations();
        }

        private void OpenMyReservations(object obj)
        {
            _navigationService.ShowMyReservations();
        }

        #endregion
    }
}