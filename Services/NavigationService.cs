using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain.Interfaces;
using BookingApp.Presentation.View.Guest;

namespace BookingApp.Services
{
    public class NavigationService : INavigationService
    {
        public void ShowAccommodations()
        {
            var accommodationsWindow = new AccommodationLookup();
            accommodationsWindow.ShowDialog();
        }
        public void ShowMyReservations()
        {
            var myReservationsWindow = new MyReservationsView();
            myReservationsWindow.ShowDialog();
        }
    }
}
