using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace BookingApp.Utilities // Proverite namespace
{
    public static class DatePickerHelper
    {
        public static readonly DependencyProperty BlackoutDatesSourceProperty =
            DependencyProperty.RegisterAttached(
                "BlackoutDatesSource",
                typeof(List<DateTime>),
                typeof(DatePickerHelper),
                new PropertyMetadata(null, OnBlackoutDatesSourceChanged));

        public static List<DateTime> GetBlackoutDatesSource(DependencyObject obj)
        {
            return (List<DateTime>)obj.GetValue(BlackoutDatesSourceProperty);
        }

        public static void SetBlackoutDatesSource(DependencyObject obj, List<DateTime> value)
        {
            obj.SetValue(BlackoutDatesSourceProperty, value);
        }

        private static void OnBlackoutDatesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DatePicker datePicker)
            {
                var newDates = e.NewValue as List<DateTime>;

                // Očistimo stare datume
                datePicker.BlackoutDates.Clear();

                if (newDates != null)
                {
                    // Dodamo nove datume
                    foreach (var date in newDates)
                    {
                        datePicker.BlackoutDates.Add(new CalendarDateRange(date));
                    }
                }
            }
        }
    }
}