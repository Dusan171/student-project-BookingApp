using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BookingApp.Domain.Model;

namespace BookingApp.View.Guide
{
    public partial class ReservedTouristsWindow : Window
    {
        private List<ReservationGuest> guests;
        private List<KeyPoint> passedKeyPoints;
        private List<TouristAttendance> attendance;


        public ReservedTouristsWindow(Tour tour, List<KeyPoint> passedKeyPoints, List<ReservationGuest> guests, List<TouristAttendance> attendance)
        {
            InitializeComponent();

            this.passedKeyPoints = passedKeyPoints;
            this.attendance = attendance;
            this.guests = guests;

            Title = $"Turisti za turu: {tour.Name}";
            PopulateGuests();
        }

        private void PopulateGuests()
        {
            GuestsPanel.Children.Clear();

            foreach (var guest in guests)
            {
                var att = attendance.FirstOrDefault(a => a.GuestId == guest.Id);
                if (att == null)
                {
                    att = new TouristAttendance
                    {
                        GuestId = guest.Id,
                        HasAppeared = false,
                        KeyPointJoinedAt = -1
                    };
                    attendance.Add(att);
                }

                var stack = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

                var cb = new CheckBox
                {
                    Content = guest.FirstName + " " + guest.LastName,
                    IsChecked = att.HasAppeared,
                    Tag = guest.Id,
                    Width = 150
                };

                var combo = new ComboBox
                {
                    ItemsSource = passedKeyPoints,
                    DisplayMemberPath = "Name",
                    SelectedValuePath = "Id",
                    Width = 150,
                    Margin = new Thickness(10, 0, 0, 0),
                    IsEnabled = att.HasAppeared
                };

                if (att.HasAppeared && att.KeyPointJoinedAt != -1)
                {
                    combo.SelectedValue = att.KeyPointJoinedAt;
                }

                cb.Checked += (s, e) =>
                {
                    combo.IsEnabled = true;
                    att.HasAppeared = true;
                    if (combo.SelectedValue is int kpId)
                        att.KeyPointJoinedAt = kpId;
                };

                cb.Unchecked += (s, e) =>
                {
                    combo.IsEnabled = false;
                    att.HasAppeared = false;
                    att.KeyPointJoinedAt = -1;
                };

                combo.SelectionChanged += (s, e) =>
                {
                    if (combo.SelectedValue is int kpId && cb.IsChecked == true)
                    {
                        att.HasAppeared = true;
                        att.KeyPointJoinedAt = kpId;
                    }
                };

                stack.Children.Add(cb);
                stack.Children.Add(combo);
                GuestsPanel.Children.Add(stack);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (StackPanel row in GuestsPanel.Children)
            {
                var cb = row.Children.OfType<CheckBox>().FirstOrDefault();
                var combo = row.Children.OfType<ComboBox>().FirstOrDefault();
                if (cb == null || combo == null) continue;

                int guestId = (int)cb.Tag;
                var att = attendance.FirstOrDefault(a => a.GuestId == guestId);
                if (att != null)
                {
                    att.HasAppeared = cb.IsChecked == true;
                    att.KeyPointJoinedAt = (att.HasAppeared && combo.SelectedValue != null)
                                            ? (int)combo.SelectedValue
                                            : -1;
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
