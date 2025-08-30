using BookingApp.Domain.Interfaces;
using BookingApp.Services;
using BookingApp.Services.DTO;
using BookingApp.Utilities;
using BookingApp.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BookingApp.Presentation.ViewModel.Owner
{
    public class OwnerDashboardViewModel : INotifyPropertyChanged
    {
        private readonly INotificationService _notificationService;
        private readonly Action _closeWindowAction;

        private bool _isNotificationPopupVisible;
        public bool IsNotificationPopupVisible
        {
            get => _isNotificationPopupVisible;
            set
            {
                _isNotificationPopupVisible = value;
                OnPropertyChanged();
            }
        }
        private object _currentViewModel;
        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }
        public ICommand NavigateCommand { get; }
        public ICommand LogoutCommand { get; }

        public OwnerDashboardViewModel(Action closeWindowAction, INotificationService notificationService)
        {
           
            _closeWindowAction = closeWindowAction;
            _notificationService = notificationService;
            ShowPendingNotifications();

            NavigateCommand = new RelayCommand(ExecuteNavigate);
            LogoutCommand = new RelayCommand(ExecuteLogout);
            CurrentViewModel = Injector.CreateHomeViewModel();
        }
        private async void ShowPendingNotifications()
        {
            _notificationService.CheckAndGenerateNotifications();

            var unreadNotifications = _notificationService.GetAll().Where(n => !n.IsRead).ToList();

            if (unreadNotifications.Any())
            {
                IsNotificationPopupVisible = true;
                await Task.Delay(3000);
                IsNotificationPopupVisible = false;

                foreach (var notification in unreadNotifications)
                {
                    _notificationService.MarkAsRead(notification.Id);
                }
            }
        }
        private void ExecuteNavigate(object parameter)
        {
            if (parameter is string viewName)
            {
                switch (viewName)
                {
                    case "Home":
                        CurrentViewModel = Injector.CreateHomeViewModel();
                        break;
                    case "RegisterAccommodation":
                        CurrentViewModel = Injector.CreateRegisterAccommodationViewModel();
                        break;
                    case "Requests":
                        CurrentViewModel = Injector.CreateRequestsViewModel(); 
                        break;
                    case "Statistic":
                      //  CurrentViewModel = Injector.CreateStatisticViewModel(); 
                        break;
                    case "Reviews":
                        CurrentViewModel = Injector.CreateReviewsViewModel();
                        break;
                    case "PDFReports":
                       // CurrentViewModel = Injector.CreatePdfReportsViewModel(); 
                        break;
                    case "Demo":
                        //CurrentViewModel = Injector.CreateDemoViewModel(); 
                        break;
                }
            }
        }
        private void ExecuteLogout(object parameter)
        {
          
            Session.CurrentUser = null;
            _closeWindowAction.Invoke();
            var signInWindow = new SignInForm();
            signInWindow.Show();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
