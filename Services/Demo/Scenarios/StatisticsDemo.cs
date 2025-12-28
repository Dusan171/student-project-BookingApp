using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using BookingApp.Domain.Interfaces;
using BookingApp.Presentation.ViewModel.Owner;
using BookingApp.Services.DTO;
using LiveCharts;
using LiveCharts.Wpf;

namespace BookingApp.Services.Demo.Scenarios
{
    public class StatisticsDemo : IDemoScenario
    {
        private OwnerDashboardViewModel dashboardViewModel;
        private StatisticViewModel statisticsViewModel;

        public StatisticsDemo()
        {
            // Empty constructor as required
        }

        public void SetDashboardViewModel(OwnerDashboardViewModel viewModel)
        {
            this.dashboardViewModel = viewModel;
        }

        public void Initialize()
        {
            // Navigate to statistics view
            dashboardViewModel?.NavigateCommand?.Execute("Statistics");
            statisticsViewModel = dashboardViewModel?.CurrentViewModel as StatisticViewModel;
        }

        public bool ExecuteStep(int step)
        {
            if (statisticsViewModel == null) return false;

            switch (step)
            {
                case 0:
                    ShowMessage("📊 DEMO: Viewing accommodation statistics");
                    return true;
                case 1:
                    ShowMessage("Loading accommodations list...");
                    LoadDemoAccommodations();
                    return true;
                case 2:
                    ShowMessage("Selecting accommodation for analysis...");
                    SimulateAccommodationSelection();
                    return true;
                case 3:
                    ShowMessage("Loading statistics data...");
                    LoadDemoStatistics();
                    return true;
                case 4:
                    ShowMessage("Displaying yearly statistics...");
                    return true;
                case 5:
                    ShowMessage("Selecting specific year for details...");
                    SimulateYearSelection();
                    return true;
                case 6:
                    ShowMessage("📈 Showing monthly breakdown for 2024");
                    LoadDemoMonthlyData();
                    return true;
                case 7:
                    ShowMessage("🏆 Peak season: August (95% occupancy)");
                    return true;
                case 8:
                    ShowMessage("Analysis complete - ready for PDF report generation");
                    return false; // End this scenario
                default:
                    return false;
            }
        }

        private void LoadDemoAccommodations()
        {
            Task.Delay(500).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (statisticsViewModel?.Accommodations != null)
                    {
                        statisticsViewModel.Accommodations.Clear();
                        var demoAccommodations = new[]
                        {
                            new AccommodationDTO{ Id = 1,Name = "Luxury Downtown Apartment",GeoLocation = new LocationDTO { City = "Belgrade", Country = "Serbia" },Type = "APARTMENT"},
                            new AccommodationDTO{ Id = 2, Name = "Mountain Retreat Cottage", GeoLocation = new LocationDTO { City = "Zlatibor", Country = "Serbia" },Type = "COTTAGE"},
                            new AccommodationDTO{ Id = 3, Name = "City Center House", GeoLocation = new LocationDTO { City = "Novi Sad", Country = "Serbia" }, Type = "HOUSE"}
                        };

                        foreach (var accommodation in demoAccommodations)
                        {
                            statisticsViewModel.Accommodations.Add(accommodation);
                        }
                    }
                });
            });
        }

        private void SimulateAccommodationSelection()
        {
            Task.Delay(500).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (statisticsViewModel?.Accommodations != null && statisticsViewModel.Accommodations.Count > 0)
                    {
                        statisticsViewModel.SelectedAccommodation = statisticsViewModel.Accommodations[0];
                    }
                });
            });
        }

        private void LoadDemoStatistics()
        {
            Task.Delay(800).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (statisticsViewModel != null)
                    {
                        var reservations = new ChartValues<int> { 28, 45, 62 };
                        var cancellations = new ChartValues<int> { 2, 3, 5 };
                        var reschedules = new ChartValues<int> { 4, 7, 12 };

                        statisticsViewModel.YearlyChartSeries = new SeriesCollection
                        {
                            new LineSeries{ Title = "Reservations",Values = reservations,Stroke = System.Windows.Media.Brushes.Blue,Fill = System.Windows.Media.Brushes.Transparent,PointGeometry = DefaultGeometries.Circle},
                            new LineSeries{ Title = "Cancellations", Values = cancellations, Stroke = System.Windows.Media.Brushes.Red, Fill = System.Windows.Media.Brushes.Transparent,PointGeometry = DefaultGeometries.Square},
                            new LineSeries{Title = "Reschedules", Values = reschedules,Stroke = System.Windows.Media.Brushes.Orange,Fill = System.Windows.Media.Brushes.Transparent,PointGeometry = DefaultGeometries.Triangle}
                        };
                        statisticsViewModel.YearLabels = new[] { "2022", "2023", "2024" };
                        if (statisticsViewModel.AvailableYears != null)
                        {
                            statisticsViewModel.AvailableYears.Clear();
                            for (int year = 2022; year <= 2024; year++)
                            { statisticsViewModel.AvailableYears.Add(year);}
                        }
                        statisticsViewModel.HasSelectedAccommodation = true;
                    }
                });
            });
        }

        private void SimulateYearSelection()
        {
            Task.Delay(500).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (statisticsViewModel != null)
                    {
                        statisticsViewModel.SelectedYear = 2024;
                    }
                });
            });
        }

        private void LoadDemoMonthlyData()
        {
            Task.Delay(600).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (statisticsViewModel != null)
                    {
                        var monthlyReservations = new ChartValues<int> { 3, 4, 6, 8, 9, 12, 15, 18, 11, 7, 4, 8 };
                        var monthlyCancellations = new ChartValues<int> { 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1 };
                        var monthlyReschedules = new ChartValues<int> { 1, 0, 1, 2, 1, 2, 3, 2, 1, 1, 0, 1 };

                        statisticsViewModel.MonthlyChartSeries = new SeriesCollection
                        {
                            new LineSeries{ Title = "Reservations",Values = monthlyReservations,Stroke = System.Windows.Media.Brushes.Blue,Fill = System.Windows.Media.Brushes.Transparent,PointGeometry = DefaultGeometries.Circle, StrokeThickness = 3 },
                            new LineSeries{Title = "Cancellations",Values = monthlyCancellations,Stroke = System.Windows.Media.Brushes.Red,Fill = System.Windows.Media.Brushes.Transparent, PointGeometry = DefaultGeometries.Square,StrokeThickness = 3 },
                            new LineSeries{ Title = "Reschedules",Values = monthlyReschedules,Stroke = System.Windows.Media.Brushes.Orange, Fill = System.Windows.Media.Brushes.Transparent, PointGeometry = DefaultGeometries.Triangle, StrokeThickness = 3}
                        };

                        statisticsViewModel.MonthLabels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                        statisticsViewModel.ShowMonthlyChart = true;
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
            if (statisticsViewModel != null)
            {
                statisticsViewModel.ShowMonthlyChart = false;
                statisticsViewModel.SelectedYear = null;
                statisticsViewModel.SelectedAccommodation = null;
                statisticsViewModel.HasSelectedAccommodation = false;
            }
        }
    }
}