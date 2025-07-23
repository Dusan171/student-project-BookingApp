using System.Collections.Generic;
using System.Windows;
using BookingApp.Model;
using System.Windows.Controls;

namespace BookingApp.View.Guide
{
    public partial class ReservedTouristsWindow : Window
    {
        private List<ReservationGuest> guests;
        private KeyPoint currentKeyPoint;

        public ReservedTouristsWindow(Tour tour, KeyPoint keyPoint, List<ReservationGuest> guests)
        {
            InitializeComponent();

            currentKeyPoint = keyPoint;
            Title = $"Reserved Tourists at: {keyPoint.Name}";

            this.guests = guests;  

            PopulateGuests();
        }


        private void PopulateGuests()
        {
            GuestsPanel.Children.Clear();

            foreach (var guest in guests)
            {
                var cb = new CheckBox
                {
                    Content = guest.FullName,
                    IsChecked = guest.HasAppeared && guest.KeyPointJoinedAt == currentKeyPoint.Id,
                    Tag = guest
                };
                GuestsPanel.Children.Add(cb);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Ažuriraj goste na osnovu checkboxova
            foreach (CheckBox cb in GuestsPanel.Children)
            {
                var guest = cb.Tag as ReservationGuest;
                if (guest != null)
                {
                    guest.HasAppeared = cb.IsChecked == true;
                    if (guest.HasAppeared)
                    {
                        guest.KeyPointJoinedAt = currentKeyPoint.Id;
                    }
                    else
                    {
                        guest.KeyPointJoinedAt = -1;
                    }
                }
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public List<ReservationGuest> GetUpdatedGuests()
        {
            return guests;
        }
    }
}
