using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace BookingApp.Utilities // Proverite namespace
{
    public static class DatePickerHelper
    {
        public static readonly DependencyProperty BlackoutDatesSourceProperty =
            DependencyProperty.RegisterAttached(
                "BlackoutDatesSource",
                typeof(CalendarBlackoutDatesCollection),
                typeof(DatePickerHelper),
                new PropertyMetadata(null, OnBlackoutDatesSourceChanged));

        public static CalendarBlackoutDatesCollection GetBlackoutDatesSource(DependencyObject obj)
        {
            return (CalendarBlackoutDatesCollection)obj.GetValue(BlackoutDatesSourceProperty);
        }

        public static void SetBlackoutDatesSource(DependencyObject obj, CalendarBlackoutDatesCollection value)
        {
            obj.SetValue(BlackoutDatesSourceProperty, value);
        }

        private static void OnBlackoutDatesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DatePicker datePicker)
            {
                var newDates = e.NewValue as CalendarBlackoutDatesCollection;

                // Očistimo stare datume
                datePicker.BlackoutDates.Clear();

                if (newDates != null)
                {
                    // Dodamo nove datume
                    foreach (var range in newDates)
                    {
                        datePicker.BlackoutDates.Add(range);
                    }
                }
            }
        }
    }
}