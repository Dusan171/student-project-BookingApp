using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Presentation.ViewModel.Owner;
using BookingApp.Services.DTO;

namespace BookingApp.Services.Demo.Scenarios
{
    public class RequestManagementDemo : IDemoScenario
    {
        private OwnerDashboardViewModel dashboardViewModel;
        private RequestsViewModel requestsViewModel;

        public RequestManagementDemo()
        {
            // Empty constructor as required
        }

        public void SetDashboardViewModel(OwnerDashboardViewModel viewModel)
        {
            this.dashboardViewModel = viewModel;
        }

        public void Initialize()
        {
            // Navigate to requests view
            dashboardViewModel?.NavigateCommand?.Execute("Requests");
            requestsViewModel = dashboardViewModel?.CurrentViewModel as RequestsViewModel;
        }

        public bool ExecuteStep(int step)
        {
            if (requestsViewModel == null) return false;

            switch (step)
            {
                case 0:
                    ShowMessage("📋 DEMO: Managing reservation requests");
                    return true;
                case 1:
                    ShowMessage("Loading pending requests...");
                    LoadDemoRequests();
                    return true;
                case 2:
                    ShowMessage("Reviewing first request from Maria Petrovic...");
                    SelectRequest(0);
                    return true;
                case 3:
                    ShowMessage("Checking availability: ✅ Available");
                    return true;
                case 4:
                    ShowMessage("Approving request...");
                    SimulateApproveRequest();
                    return true;
                case 5:
                    ShowMessage("✅ Request approved and dates updated!");
                    return true;
                case 6:
                    ShowMessage("Loading next request from Stefan Nikolic...");
                    LoadSecondRequest();
                    return true;
                case 7:
                    ShowMessage("Checking availability: ❌ Unavailable - rejecting...");
                    SelectRequest(0);
                    return true;
                case 8:
                    ShowMessage("Adding rejection reason...");
                    SimulateRejectRequest();
                    return true;
                case 9:
                    ShowMessage("Sending rejection with explanation...");
                    SimulateSubmitReject();
                    return true;
                case 10:
                    ShowMessage("❌ Request rejected with explanation sent");
                    return false; 
                default:
                    return false;
            }
        }

        private void LoadDemoRequests()
        {
            Task.Delay(500).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (requestsViewModel?.Requests != null)
                    {
                        requestsViewModel.Requests.Clear();

                        var demoRequests = new[]
                        {
                            new RescheduleRequestDTO{ Id = 1,AccommodationName = "Luxury Downtown Apartment",OriginalStartDate = new DateTime(2024, 12, 15), OriginalEndDate = new DateTime(2024, 12, 20),NewStartDate = new DateTime(2024, 12, 22),NewEndDate = new DateTime(2024, 12, 27),AvailabilityStatus = "Available", Status = RequestStatus.Pending},
                            new RescheduleRequestDTO { Id = 2, AccommodationName = "Mountain Retreat Cottage",OriginalStartDate = new DateTime(2025, 1, 1), OriginalEndDate = new DateTime(2025, 1, 5), NewStartDate = new DateTime(2025, 1, 10), NewEndDate = new DateTime(2025, 1, 14), AvailabilityStatus = "Not available", Status = RequestStatus.Pending}
                        };
                        foreach (var request in demoRequests)
                        {
                            requestsViewModel.Requests.Add(request);
                        }
                    }
                });
            });
        }

        private void SelectRequest(int index)
        {
            Task.Delay(300).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (requestsViewModel?.Requests != null && requestsViewModel.Requests.Count > index)
                    {
                        requestsViewModel.SelectedRequest = requestsViewModel.Requests[index];
                    }
                });
            });
        }

        private void SimulateApproveRequest()
        {
            Task.Delay(500).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (requestsViewModel?.SelectedRequest != null && requestsViewModel.Requests != null)
                    {
                        requestsViewModel.SelectedRequest.Status = RequestStatus.Approved;

                        var approvedRequest = requestsViewModel.SelectedRequest;
                        requestsViewModel.Requests.Remove(approvedRequest);
                    }
                });
            });
        }

        private void LoadSecondRequest()
        {
            Task.Delay(800).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (requestsViewModel?.Requests != null)
                    {
                        requestsViewModel.Requests.Clear();
                        var unavailableRequest = new RescheduleRequestDTO{ Id = 2,AccommodationName = "Mountain Retreat Cottage", OriginalStartDate = new DateTime(2025, 1, 1),OriginalEndDate = new DateTime(2025, 1, 5), NewStartDate = new DateTime(2025, 1, 10), NewEndDate = new DateTime(2025, 1, 14),AvailabilityStatus = "Not available", Status = RequestStatus.Pending};
                        requestsViewModel.Requests.Add(unavailableRequest);
                    }
                });
            });
        }

        private void SimulateRejectRequest()
        {
            Task.Delay(500).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (requestsViewModel != null)
                    {
                        requestsViewModel.IsRejecting = true;

                        Task.Delay(800).ContinueWith(__ =>
                        {
                            Application.Current.Dispatcher.BeginInvoke(() =>
                            {
                                requestsViewModel.RejectReason = "Requested dates are booked.";
                            });
                        });
                    }
                });
            });
        }

        private void SimulateSubmitReject()
        {
            Task.Delay(800).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (requestsViewModel?.SelectedRequest != null && requestsViewModel.Requests != null)
                    {
                        // Simulate rejection process
                        requestsViewModel.SelectedRequest.Status = RequestStatus.Rejected;
                        requestsViewModel.SelectedRequest.OwnerComment = requestsViewModel.RejectReason;

                        // Clean up UI state
                        requestsViewModel.IsRejecting = false;
                        requestsViewModel.RejectReason = string.Empty;

                        // Remove from pending requests
                        var rejectedRequest = requestsViewModel.SelectedRequest;
                        requestsViewModel.Requests.Remove(rejectedRequest);
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
            if (requestsViewModel != null)
            {
                requestsViewModel.Requests?.Clear();
                requestsViewModel.SelectedRequest = null;
                requestsViewModel.IsRejecting = false;
                requestsViewModel.RejectReason = string.Empty;
            }
        }
    }
}