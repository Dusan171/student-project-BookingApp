using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Interfaces.ServiceInterfaces;
using BookingApp.Presentation.View.Owner;
using BookingApp.Services.DTO;
using BookingApp.Utilities;
using LiveCharts;
using LiveCharts.Wpf;

namespace BookingApp.Presentation.ViewModel.Owner
{
    public class StatisticViewModel : INotifyPropertyChanged
    {
        private readonly IAccommodationService _accommodationService;
        private readonly IAccommodationStatisticsService _statisticsService;
        private readonly IUserService _userService;
        private readonly IPDFReportService _pdfReportService;
        private readonly IAccommodationReviewService _accommodationReviewService;

        private AccommodationDTO _selectedAccommodation;
        private bool _hasSelectedAccommodation;
        private int? _selectedYear;
        private SeriesCollection _yearlyChartSeries;
        private SeriesCollection _monthlyChartSeries;
        private string[] _yearLabels;
        private string[] _monthLabels;
        private bool _showMonthlyChart;
        private AccommodationStatisticsSummaryDTO _summary;

        public ObservableCollection<AccommodationDTO> Accommodations { get; set; }
        public ObservableCollection<int> AvailableYears { get; set; }

        public AccommodationDTO SelectedAccommodation
        {
            get => _selectedAccommodation;
            set
            {
                _selectedAccommodation = value;
                OnPropertyChanged();
            }
        }

        public bool HasSelectedAccommodation
        {
            get => _hasSelectedAccommodation;
            set
            {
                _hasSelectedAccommodation = value;
                OnPropertyChanged();
            }
        }

        public int? SelectedYear
        {
            get => _selectedYear;
            set
            {
                _selectedYear = value;
                OnPropertyChanged();
                if (value.HasValue && HasSelectedAccommodation)
                {
                    LoadMonthlyData(value.Value);
                }
                else
                {
                    ShowMonthlyChart = false;
                }
            }
        }

        public SeriesCollection YearlyChartSeries
        {
            get => _yearlyChartSeries;
            set
            {
                _yearlyChartSeries = value;
                OnPropertyChanged();
            }
        }

        public SeriesCollection MonthlyChartSeries
        {
            get => _monthlyChartSeries;
            set
            {
                _monthlyChartSeries = value;
                OnPropertyChanged();
            }
        }

        public string[] YearLabels
        {
            get => _yearLabels;
            set
            {
                _yearLabels = value;
                OnPropertyChanged();
            }
        }

        public string[] MonthLabels
        {
            get => _monthLabels;
            set
            {
                _monthLabels = value;
                OnPropertyChanged();
            }
        }

        public bool ShowMonthlyChart
        {
            get => _showMonthlyChart;
            set
            {
                _showMonthlyChart = value;
                OnPropertyChanged();
            }
        }

        // Summary properties for display
        public int TotalReservations => _summary?.TotalReservations ?? 0;
        public int TotalCancellations => _summary?.TotalCancellations ?? 0;
        public int TotalReschedules => _summary?.TotalReschedules ?? 0;
        public int BestYear => _summary?.BestYear ?? 0;
        public string BestYearText => _summary != null ? $"Best year: {_summary.BestYear}" : "Best year: N/A";
        public string BestPeriodDescription => _summary?.BestPeriodDescription ?? "No data";
        public double BestPeriodOccupancy => _summary?.BestPeriodOccupancy ?? 0;
        public string BestMonthText => _summary != null ? $"Best month: {_summary.BestPeriodDescription}" : "Best month: N/A";

        public ICommand LoadStatisticsCommand { get; }
        public ICommand GeneratePDFReportCommand { get; }

        public StatisticViewModel(IAccommodationService accommodationService, IAccommodationStatisticsService statisticsService, IUserService userService, IPDFReportService pdfReportService, IAccommodationReviewService accommodationReviewService)
        {
            _accommodationService = accommodationService;
            _statisticsService = statisticsService;
            _userService = userService;
            _pdfReportService = pdfReportService; 
            _accommodationReviewService = accommodationReviewService; 

            Accommodations = new ObservableCollection<AccommodationDTO>();
            AvailableYears = new ObservableCollection<int>();

            LoadAccommodations();
            LoadStatisticsCommand = new RelayCommand(ExecuteLoadStatistics);
            GeneratePDFReportCommand = new RelayCommand(ExecuteGeneratePDF);
        }

        private void LoadAccommodations()
        {
            try
            {
                int currentOwnerId = _userService.GetCurrentUserId();
                var ownerAccommodations = _accommodationService.GetAccommodationsByOwnerId(currentOwnerId);

                Accommodations.Clear();
                foreach (var accommodation in ownerAccommodations)
                {
                    Accommodations.Add(accommodation);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error loading accommodations: {ex.Message}");
            }
        }

        private void ExecuteLoadStatistics()
        {
            if (SelectedAccommodation == null)
            {
                System.Windows.MessageBox.Show("Please select an accommodation first!");
                return;
            }

            try
            {
                LoadStatisticsData();
                HasSelectedAccommodation = true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error loading statistics: {ex.Message}");
            }
        }

        private void LoadStatisticsData()
        {
            // Get summary
            _summary = _statisticsService.GetStatisticsSummary(SelectedAccommodation.Id);

            // Get yearly statistics
            var yearlyStats = _statisticsService.GetYearlyStatistics(SelectedAccommodation.Id);

            // Update available years
            AvailableYears.Clear();
            foreach (var stat in yearlyStats.OrderBy(s => s.Year))
            {
                AvailableYears.Add(stat.Year);
            }

            // Create yearly chart
            CreateYearlyChart(yearlyStats);

            // Reset monthly chart and selected year
            SelectedYear = null;
            ShowMonthlyChart = false;

            // Notify property changes for summary
            RefreshSummaryProperties();
        }

        private void CreateYearlyChart(System.Collections.Generic.List<YearlyStatisticDTO> yearlyStats)
        {
            if (!yearlyStats.Any())
            {
                YearlyChartSeries = new SeriesCollection();
                YearLabels = new string[0];
                return;
            }

            var reservations = new ChartValues<int>();
            var cancellations = new ChartValues<int>();
            var reschedules = new ChartValues<int>();
            var years = new string[yearlyStats.Count];

            for (int i = 0; i < yearlyStats.Count; i++)
            {
                var stat = yearlyStats[i];
                reservations.Add(stat.ReservationCount);
                cancellations.Add(stat.CancellationCount);
                reschedules.Add(stat.RescheduleCount);
                years[i] = stat.Year.ToString();
            }

            YearlyChartSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Reservations",
                    Values = reservations,
                    Stroke = System.Windows.Media.Brushes.Blue,
                    Fill = System.Windows.Media.Brushes.Transparent,
                    PointGeometry = LiveCharts.Wpf.DefaultGeometries.Circle
                },
                new LineSeries
                {
                    Title = "Cancellations",
                    Values = cancellations,
                    Stroke = System.Windows.Media.Brushes.Red,
                    Fill = System.Windows.Media.Brushes.Transparent,
                    PointGeometry = LiveCharts.Wpf.DefaultGeometries.Square
                },
                new LineSeries
                {
                    Title = "Reschedules",
                    Values = reschedules,
                    Stroke = System.Windows.Media.Brushes.Orange,
                    Fill = System.Windows.Media.Brushes.Transparent,
                    PointGeometry = LiveCharts.Wpf.DefaultGeometries.Triangle
                }
            };

            YearLabels = years;
        }

        private void LoadMonthlyData(int year)
        {
            try
            {
                var monthlyStats = _statisticsService.GetMonthlyStatistics(SelectedAccommodation.Id, year);
                CreateMonthlyChart(monthlyStats);
                ShowMonthlyChart = true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error loading monthly data: {ex.Message}");
                ShowMonthlyChart = false;
            }
        }

        private void CreateMonthlyChart(System.Collections.Generic.List<MonthlyStatisticDTO> monthlyStats)
        {
            var reservations = new ChartValues<int>();
            var cancellations = new ChartValues<int>();
            var reschedules = new ChartValues<int>();
            var months = new string[monthlyStats.Count];

            for (int i = 0; i < monthlyStats.Count; i++)
            {
                var stat = monthlyStats[i];
                reservations.Add(stat.ReservationCount);
                cancellations.Add(stat.CancellationCount);
                reschedules.Add(stat.RescheduleCount);
                months[i] = stat.MonthName.Substring(0, Math.Min(3, stat.MonthName.Length)); // Jan, Feb, etc.
            }

            MonthlyChartSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Reservations",
                    Values = reservations,
                    Stroke = System.Windows.Media.Brushes.Blue,
                    Fill = System.Windows.Media.Brushes.Transparent,
                    PointGeometry = LiveCharts.Wpf.DefaultGeometries.Circle,
                    StrokeThickness = 3
                },
                new LineSeries
                {
                    Title = "Cancellations",
                    Values = cancellations,
                    Stroke = System.Windows.Media.Brushes.Red,
                    Fill = System.Windows.Media.Brushes.Transparent,
                    PointGeometry = LiveCharts.Wpf.DefaultGeometries.Square,
                    StrokeThickness = 3
                },
                new LineSeries
                {
                    Title = "Reschedules",
                    Values = reschedules,
                    Stroke = System.Windows.Media.Brushes.Orange,
                    Fill = System.Windows.Media.Brushes.Transparent,
                    PointGeometry = LiveCharts.Wpf.DefaultGeometries.Triangle,
                    StrokeThickness = 3
                }
            };

            MonthLabels = months;
        }

        private void RefreshSummaryProperties()
        {
            OnPropertyChanged(nameof(TotalReservations));
            OnPropertyChanged(nameof(TotalCancellations));
            OnPropertyChanged(nameof(TotalReschedules));
            OnPropertyChanged(nameof(BestYear));
            OnPropertyChanged(nameof(BestYearText));
            OnPropertyChanged(nameof(BestPeriodDescription));
            OnPropertyChanged(nameof(BestPeriodOccupancy));
            OnPropertyChanged(nameof(BestMonthText));
        }

        private void ExecuteGeneratePDF(object parameter)
        {
            if (!HasSelectedAccommodation)
            {
                System.Windows.MessageBox.Show("Please load statistics first!");
                return;
            }

            var pdfDialog = new PDFSettingsDialog();
            pdfDialog.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}