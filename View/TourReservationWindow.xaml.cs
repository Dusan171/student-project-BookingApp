using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using BookingApp.Domain;
using BookingApp.Repositories;

namespace BookingApp.View
{
    public partial class TourReservationWindow : Window
    {
        public Tour Tour { get; set; }
        public Tourist CurrentTourist { get; set; }
        public ObservableCollection<ReservationGuest> Guests { get; set; }

        private readonly TourReservationRepository _reservationRepo;
        private readonly ReservationGuestRepository _guestRepo;
        private readonly TourRepository _tourRepo;

        public TourReservationWindow(Tour selectedTour, Tourist loggedInTourist)
        {
            InitializeComponent();
            Tour = selectedTour;
            CurrentTourist = loggedInTourist;
            Guests = new ObservableCollection<ReservationGuest>();

            _reservationRepo = new TourReservationRepository();
            _guestRepo = new ReservationGuestRepository();
            _tourRepo = new TourRepository();

            DataContext = this;
            GuestsList.ItemsSource = Guests;
        }

        private void AddGuest_Click(object sender, RoutedEventArgs e)
        {
            Guests.Add(new ReservationGuest());
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            int totalRequested = Guests.Count;
            int available = Tour.MaxTourists - Tour.ReservedSpots;

            if (totalRequested == 0)
            {
                MessageBox.Show("Dodajte barem jednog gosta.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (totalRequested > available)
            {
                MessageBox.Show($"Nema dovoljno mesta (dostupno: {available}).", "Tura popunjena", MessageBoxButton.OK, MessageBoxImage.Warning);

                var alternativeTours = _tourRepo.GetAll()
                    .Where(t => t.Location.City == Tour.Location.City &&
                                t.Id != Tour.Id &&
                                (t.MaxTourists - t.ReservedSpots) >= totalRequested)
                    .ToList();

                if (alternativeTours.Count > 0)
                {
                    var alternativesWindow = new AlternativeToursWindow(alternativeTours, CurrentTourist);
                    alternativesWindow.Owner = this;
                    alternativesWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Nema dostupnih alternativnih tura na istoj lokaciji.", "Nema alternativa", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                return;
            }

            var newReservation = new TourReservation
            {
                Id = _reservationRepo.NextId(),
                Tour = Tour,
                Tourist = CurrentTourist,
                Guests = Guests.ToList(),
                ReservationTime = DateTime.Now,
                IsActive = true
            };

            _reservationRepo.Save(newReservation);

            foreach (var guest in Guests)
            {
                guest.Id = _guestRepo.NextId();
                guest.ReservationId = newReservation.Id;
                _guestRepo.Save(guest);
            }

            Tour.ReservedSpots += totalRequested;
            _tourRepo.Update(Tour);

            MessageBox.Show("Uspešno rezervisano!", "Potvrda", MessageBoxButton.OK, MessageBoxImage.Information);

            if (Application.Current.Windows.OfType<TourSearch>().FirstOrDefault() is TourSearch mainWindow)
            {
                mainWindow.LoadAllTours();
            }

            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
