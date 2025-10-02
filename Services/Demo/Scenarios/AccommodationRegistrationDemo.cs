using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using BookingApp.Domain.Interfaces;
using BookingApp.Presentation.ViewModel.Owner;
using BookingApp.Services.DTO;

namespace BookingApp.Services.Demo.Scenarios
{
    public class AccommodationRegistrationDemo : IDemoScenario
    {
        private OwnerDashboardViewModel dashboardViewModel;
        private RegisterAccommodationViewModel registerViewModel;

        public AccommodationRegistrationDemo()
        {
        }

        public void SetDashboardViewModel(OwnerDashboardViewModel viewModel)
        {
            this.dashboardViewModel = viewModel;
        }

        public void Initialize()
        {
            dashboardViewModel?.NavigateCommand?.Execute("RegisterAccommodation");
            registerViewModel = dashboardViewModel?.CurrentViewModel as RegisterAccommodationViewModel;
        }

        public bool ExecuteStep(int step)
        {
            if (registerViewModel == null) return false;

            switch (step)
            {
                case 0:
                    ShowMessage("📝 DEMO: Registering new accommodation");
                    return true;
                case 1:
                    ShowMessage("Entering property name...");
                    SimulateTyping(() => registerViewModel.Accommodation.Name = "Luxury Downtown Apartment");
                    return true;
                case 2:
                    ShowMessage("Setting location...");
                    SimulateLocationInput();
                    return true;
                case 3:
                    ShowMessage("Selecting property type...");
                    SimulateTyping(() => registerViewModel.Accommodation.Type = "APARTMENT");
                    return true;
                case 4:
                    ShowMessage("Setting maximum guests...");
                    SimulateTyping(() => registerViewModel.Accommodation.MaxGuests = 4);
                    return true;
                case 5:
                    ShowMessage("Setting minimum reservation days...");
                    SimulateTyping(() => registerViewModel.Accommodation.MinReservationDays = 2);
                    return true;
                case 6:
                    ShowMessage("Setting cancellation deadline...");
                    SimulateTyping(() => registerViewModel.Accommodation.CancellationDeadlineDays = 3);
                    return true;
                case 7:
                    ShowMessage("Adding property images...");
                    SimulateAddImages();
                    return true;
                case 8:
                    ShowMessage("Saving new accommodation...");
                    Task.Delay(1500).ContinueWith(_ =>
                    {
                        Application.Current.Dispatcher.BeginInvoke(() =>
                        {
                            ShowMessage("✅ Accommodation successfully created!");
                        });
                    });
                    return true;
                case 9:
                    ShowMessage("Returning to accommodations list...");
                    return false; 
                default:
                    return false;
            }
        }

        private void SimulateTyping(Action action)
        {
            Task.Delay(500).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.BeginInvoke(action);
            });
        }

        private void SimulateLocationInput()
        {
            Task.Delay(500).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (registerViewModel?.Accommodation != null)
                    {
                        registerViewModel.Accommodation.GeoLocation = new LocationDTO
                        {
                            City = "Belgrade",
                            Country = "Serbia"
                        };
                    }
                });
            });
        }

        private void SimulateAddImages()
        {
            Task.Delay(500).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (registerViewModel?.ImagePaths != null)
                    {
                        registerViewModel.ImagePaths.Clear();

                        var demoImages = new[]
                        {
                            new AccommodationImageDTO { Path = "4416c2b3-1e55-4a05-bbb9-e9e29cfe828e.jpg" },
                            
                        };

                        foreach (var image in demoImages)
                        {
                            registerViewModel.ImagePaths.Add(image);
                        }
                    }
                });
            });
        }


        private void ShowMessage(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[DEMO] {message}");
        }

        public void Cleanup()
        {
            if (registerViewModel?.ImagePaths != null)
            {
                registerViewModel.ImagePaths.Clear();
            }
        }
    }
}