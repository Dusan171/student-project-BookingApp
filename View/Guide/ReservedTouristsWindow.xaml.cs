using System.Collections.Generic;
using System.Windows;
using BookingApp.Domain;
using System.Windows.Controls;
using System.Linq;
using BookingApp.Domain.Model;

namespace BookingApp.View.Guide
{
    public partial class ReservedTouristsWindow : Window
    {
        private List<ReservationGuest> guests;
        private KeyPoint currentKeyPoint;
        private List<TouristAttendance> attendance;

        public ReservedTouristsWindow(Tour tour, KeyPoint keyPoint, List<ReservationGuest> guests, List<TouristAttendance> attendance)
        {
            InitializeComponent();

            currentKeyPoint = keyPoint;
            Title = $"Reserved Tourists at: {keyPoint.Name}";
            this.attendance = attendance;
            this.guests = guests;  

            PopulateGuests();
        }


        private void PopulateGuests()
        {
            GuestsPanel.Children.Clear();

            foreach (var guest in guests)
            {
                var att = attendance.FirstOrDefault(a => a.GuestId == guest.Id);
                var cb = new CheckBox
                {
                    Content = guest.FirstName + " " + guest.LastName,
                    IsChecked = att != null && att.HasAppeared && att.KeyPointJoinedAt == currentKeyPoint.Id,
                    Tag = guest.Id
                };
                GuestsPanel.Children.Add(cb);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox cb in GuestsPanel.Children)
            {
                int guestId = (int)cb.Tag;
                var att = attendance.FirstOrDefault(a => a.GuestId == guestId);
                if (att != null)
                {
                    att.HasAppeared = cb.IsChecked == true;
                    att.KeyPointJoinedAt = att.HasAppeared ? currentKeyPoint.Id : -1;
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

        public List<TouristAttendance> GetUpdatedAttendance()
        {
            return attendance;
        }
    }
}
