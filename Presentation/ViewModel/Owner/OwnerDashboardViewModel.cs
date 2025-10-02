using BookingApp.Domain.Interfaces;
using BookingApp.Services;
using BookingApp.Services.Demo;
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
        private readonly DemoManager _demoManager;

        private bool _isNotificationPopupVisible;
        private object _currentViewModel;
        private string _demoMessage;

        public bool IsNotificationPopupVisible
        {
            get => _isNotificationPopupVisible;
            set
            {
                _isNotificationPopupVisible = value;
                OnPropertyChanged();
            }
        }

        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public string DemoMessage
        {
            get => _demoMessage;
            set
            {
                _demoMessage = value;
                OnPropertyChanged();
            }
        }

        public bool IsDemoRunning => _demoManager?.IsRunning ?? false;

        public ICommand NavigateCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand StartDemoCommand { get; }
        public ICommand StopDemoCommand { get; }

        public OwnerDashboardViewModel(Action closeWindowAction, INotificationService notificationService)
        {
            _closeWindowAction = closeWindowAction;
            _notificationService = notificationService;
            
            // Initialize Demo Manager
            _demoManager = new DemoManager(this);
            _demoManager.OnDemoMessage += (message) => 
            {
                DemoMessage = message;
                OnPropertyChanged(nameof(IsDemoRunning));
            };
            
            ShowPendingNotifications();

            NavigateCommand = new RelayCommand(ExecuteNavigate);
            LogoutCommand = new RelayCommand(ExecuteLogout);
            StartDemoCommand = new RelayCommand(ExecuteStartDemo);
            StopDemoCommand = new RelayCommand(ExecuteStopDemo);
            
            var homeViewModel = Injector.CreateHomeViewModel();
            homeViewModel.NavigateCommand = this.NavigateCommand;
            CurrentViewModel = homeViewModel;
        }

        private object _previousViewModel; 
        private void ExecuteStartDemo(object parameter)
        {
            _previousViewModel = CurrentViewModel;

            _demoManager.StartDemo();
            OnPropertyChanged(nameof(IsDemoRunning));
        }

        private void ExecuteStopDemo(object parameter)
        {
            _demoManager.StopDemo();

            
            if (_previousViewModel != null)
            {
                CurrentViewModel = _previousViewModel;
                _previousViewModel = null; 
            }
            else
            {
                ExecuteNavigate("Home");
            }

            OnPropertyChanged(nameof(IsDemoRunning));
        }

        public void HandleKeyDown(Key key)
        {
            if (key == Key.Escape && IsDemoRunning)
            {
                _demoManager.StopDemo();
                OnPropertyChanged(nameof(IsDemoRunning));
            }
            else if (key == Key.F5)
            {
                if (IsDemoRunning)
                {
                    _demoManager.StopDemo();
                }
                else
                {
                    _demoManager.StartDemo();
                }
                OnPropertyChanged(nameof(IsDemoRunning));
            }
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
                        var homeViewModel = Injector.CreateHomeViewModel();
                        homeViewModel.NavigateCommand = this.NavigateCommand; 
                        CurrentViewModel = homeViewModel;
                        break;
                    case "Accommodations":
                        CurrentViewModel = Injector.CreateAccommodationsViewModel(
                            () => NavigateCommand?.Execute("Home"),
                            () => NavigateCommand?.Execute("RegisterAccommodation")
                        );
                        break;
                    case "Requests":
                        CurrentViewModel = Injector.CreateRequestsViewModel(); 
                        break;
                    case "Statistics":
                        CurrentViewModel = Injector.CreateStatisticViewModel(); 
                        break;
                    case "Reviews":
                        CurrentViewModel = Injector.CreateReviewsViewModel();
                        break;
                    case "Forums":
                        var forumViewModel = Injector.CreateForumViewModel();
                        forumViewModel.OnShowCommentsRequested += (forumId) => // Sada je int, ne DTO objekat
                        {
                            var commentsViewModel = Injector.CreateForumCommentsViewModel(forumId); // Šalješ int
                            commentsViewModel.OnBackToForumsRequested += () =>
                            {
                                forumViewModel.RefreshForums();
                                CurrentViewModel = forumViewModel;
                            };
                            CurrentViewModel = commentsViewModel;
                        };
                        CurrentViewModel = forumViewModel;
                        break;
                    case "Demo":
                        if (IsDemoRunning)
                        {
                            _demoManager.StopDemo();
                        }
                        else
                        {
                            _demoManager.StartDemo();
                        }
                        OnPropertyChanged(nameof(IsDemoRunning));
                        break;
                    case "UnratedGuests":  
                        CurrentViewModel = Injector.CreateUnratedGuestsViewModel(() => NavigateCommand?.Execute("Home"));
                        break;
                    case "RegisterAccommodation":
                        CurrentViewModel = Injector.CreateRegisterAccommodationViewModel(
                            () => NavigateCommand?.Execute("Accommodations")
                        );
                        break;
                    case "Suggestions":
                        CurrentViewModel = Injector.CreateSuggestionsViewModel(
                            Session.CurrentUser.Id, // ili LoggedUser.Id - zavisno kako čuvaš trenutnog korisnika
                            () => NavigateCommand?.Execute("Home"),
                            (location) => NavigateCommand?.Execute("RegisterAccommodation")
                        );
                        break;
                }
            }
        }

        
        private void ExecuteLogout(object parameter)
        {
            // Stop demo if running
            if (IsDemoRunning)
            {
                _demoManager.StopDemo();
            }

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