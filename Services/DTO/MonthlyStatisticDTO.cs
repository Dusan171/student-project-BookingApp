using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services.DTO
{
    public class MonthlyStatisticDTO : INotifyPropertyChanged
    {
        private int _month;
        private string _monthName;
        private int _year;
        private int _reservationCount;
        private int _cancellationCount;
        private int _rescheduleCount;
        private double _occupancyRate;

        public int Month
        {
            get => _month;
            set
            {
                _month = value;
                OnPropertyChanged();
            }
        }

        public string MonthName
        {
            get => _monthName;
            set
            {
                _monthName = value;
                OnPropertyChanged();
            }
        }

        public int Year
        {
            get => _year;
            set
            {
                _year = value;
                OnPropertyChanged();
            }
        }

        public int ReservationCount
        {
            get => _reservationCount;
            set
            {
                _reservationCount = value;
                OnPropertyChanged();
            }
        }

        public int CancellationCount
        {
            get => _cancellationCount;
            set
            {
                _cancellationCount = value;
                OnPropertyChanged();
            }
        }

        public int RescheduleCount
        {
            get => _rescheduleCount;
            set
            {
                _rescheduleCount = value;
                OnPropertyChanged();
            }
        }

        public double OccupancyRate
        {
            get => _occupancyRate;
            set
            {
                _occupancyRate = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
