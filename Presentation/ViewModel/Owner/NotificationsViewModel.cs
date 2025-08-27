using BookingApp.Domain;
using BookingApp.Domain.Model;
using BookingApp.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookingApp.Presentation.ViewModel.Owner
{
    public class NotificationsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Notification> _notifications;
        public ObservableCollection<Notification> Notifications
        {
            get => _notifications;
            set
            {
                _notifications = value;
                OnPropertyChanged();
            }
        }

        public ICommand RateGuestCommand { get; }
        public event Action<int, int> RateGuestRequested;

        public NotificationsViewModel()
        {
            Notifications = new ObservableCollection<Notification>();
            RateGuestCommand = new RelayCommand(ExecuteRateGuest, CanExecuteRateGuest);
        }

        public void AddNotifications(List<Notification> notifications)
        {
            Notifications.Clear();
            if (notifications != null)
            {
                foreach (var notification in notifications)
                {
                    Notifications.Add(notification);
                }
            }
        }

        private void ExecuteRateGuest(object parameter)
        {
            if (parameter is Notification selectedNotification)
            {
                RateGuestRequested?.Invoke(selectedNotification.ReservationId, selectedNotification.GuestId);
            }
        }

        private bool CanExecuteRateGuest(object parameter)
        {
            return parameter is Notification;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
