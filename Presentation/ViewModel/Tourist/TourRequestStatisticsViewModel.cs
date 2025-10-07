using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BookingApp.Utilities;
using System.Windows;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Diagnostics;
using System.IO;
using PdfSharp.Fonts;
using BookingApp.Domain.Interfaces;

namespace BookingApp.Presentation.ViewModel.Tourist
{
    public class TourRequestStatisticsViewModel : INotifyPropertyChanged
    {
        private readonly int _currentUserId;
        private readonly string _currentUserName;
        private readonly ITourRequestStatisticsService _statisticsService;

        private ObservableCollection<string> _availableYears;
        private string _selectedYear = "Sva vremena";
        private double _acceptanceRate;
        private int _acceptedCount;
        private int _notAcceptedCount;
        private double _averagePeopleCount;
        private ObservableCollection<LanguageStatistic> _languageStatistics;
        private ObservableCollection<LocationStatistic> _locationStatistics;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ObservableCollection<string> AvailableYears
        {
            get => _availableYears;
            set { _availableYears = value; OnPropertyChanged(); }
        }

        public string SelectedYear
        {
            get => _selectedYear;
            set
            {
                _selectedYear = value;
                OnPropertyChanged();
                LoadStatistics();
            }
        }

        public double AcceptanceRate
        {
            get => _acceptanceRate;
            set { _acceptanceRate = value; OnPropertyChanged(); }
        }

        public int AcceptedCount
        {
            get => _acceptedCount;
            set { _acceptedCount = value; OnPropertyChanged(); }
        }

        public int NotAcceptedCount
        {
            get => _notAcceptedCount;
            set { _notAcceptedCount = value; OnPropertyChanged(); }
        }

        public double AveragePeopleCount
        {
            get => _averagePeopleCount;
            set { _averagePeopleCount = value; OnPropertyChanged(); }
        }

        public ObservableCollection<LanguageStatistic> LanguageStatistics
        {
            get => _languageStatistics;
            set { _languageStatistics = value; OnPropertyChanged(); }
        }

        public ObservableCollection<LocationStatistic> LocationStatistics
        {
            get => _locationStatistics;
            set { _locationStatistics = value; OnPropertyChanged(); }
        }

        public ICommand GenerateReportCommand { get; }

        public TourRequestStatisticsViewModel(int currentUserId) : this(currentUserId, "N/A") { }

        public TourRequestStatisticsViewModel(int currentUserId, string userName, ITourRequestStatisticsService statisticsService = null)
        {
            _currentUserId = currentUserId;
            _currentUserName = string.IsNullOrWhiteSpace(userName) ? "N/A" : userName;
            _statisticsService = statisticsService ?? Services.Injector.CreateInstance<ITourRequestStatisticsService>();

            AvailableYears = new ObservableCollection<string>();
            LanguageStatistics = new ObservableCollection<LanguageStatistic>();
            LocationStatistics = new ObservableCollection<LocationStatistic>();

            GenerateReportCommand = new RelayCommand(GenerateReport);

            InitializeYears();
            LoadStatistics();
        }

        private void InitializeYears()
        {
            AvailableYears.Add("Sva vremena");

            // Dinamički dodaj godine na osnovu trenutne godine
            int currentYear = DateTime.Now.Year;
            for (int year = currentYear; year >= currentYear - 5; year--)
            {
                AvailableYears.Add(year.ToString());
            }
        }

        private void LoadStatistics()
        {
            try
            {
                int? selectedYearValue = null;
                if (SelectedYear != "Sva vremena" && int.TryParse(SelectedYear, out int year))
                {
                    selectedYearValue = year;
                }

                // Učitaj opšte statistike
                var stats = _statisticsService.GetStatisticsForTourist(_currentUserId, selectedYearValue);
                AcceptedCount = stats.AcceptedCount;
                NotAcceptedCount = stats.NotAcceptedCount;
                AcceptanceRate = stats.AcceptanceRate;
                AveragePeopleCount = stats.AveragePeopleInAcceptedRequests;

                // Učitaj statistike po jeziku
                LoadLanguageStatistics(selectedYearValue);

                // Učitaj statistike po lokaciji
                LoadLocationStatistics(selectedYearValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju statistike: {ex.Message}",
                    "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadLanguageStatistics(int? year)
        {
            LanguageStatistics.Clear();

            var langData = _statisticsService.GetLanguageStatistics(_currentUserId, year);
            int max = langData.Any() ? langData.Max(x => x.Count) : 0;

            foreach (var item in langData)
            {
                LanguageStatistics.Add(new LanguageStatistic
                {
                    Language = item.Language,
                    Count = item.Count,
                    BarWidth = max > 0 ? (item.Count * 300.0 / max) : 0
                });
            }
        }

        private void LoadLocationStatistics(int? year)
        {
            LocationStatistics.Clear();

            var locData = _statisticsService.GetLocationStatistics(_currentUserId, year);
            int max = locData.Any() ? locData.Max(x => x.Count) : 0;

            foreach (var item in locData)
            {
                LocationStatistics.Add(new LocationStatistic
                {
                    Location = item.Location,
                    Count = item.Count,
                    BarWidth = max > 0 ? (item.Count * 300.0 / max) : 0
                });
            }
        }

        private void GenerateReport()
        {
            try
            {
                if (GlobalFontSettings.FontResolver == null)
                    GlobalFontSettings.FontResolver = new AppFontResolver();

                var document = new PdfDocument();
                document.Info.Title = "Statistika zahteva za ture";

                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);

                var pdfOpts = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);

                var titleFont = new XFont("App Arial", 18, XFontStyleEx.Bold, pdfOpts);
                var headerFont = new XFont("App Arial", 14, XFontStyleEx.Bold, pdfOpts);
                var textFont = new XFont("App Arial", 12, XFontStyleEx.Regular, pdfOpts);
                var smallFont = new XFont("App Arial", 10, XFontStyleEx.Regular, pdfOpts);

                double marginL = 40;
                double y = 40;

                gfx.DrawString("Statistika zahteva za ture", titleFont, XBrushes.DarkBlue,
                    new XRect(0, y, page.Width, page.Height), XStringFormats.TopCenter);
                y += 36;

                gfx.DrawString($"Period: {SelectedYear}", textFont, XBrushes.Black, marginL, y);
                y += 18;
                gfx.DrawString("Napomena: statistika obuhvata SAMO obične ture.", smallFont, XBrushes.Black, marginL, y);
                y += 18;

                gfx.DrawString("Opšte statistike", headerFont, XBrushes.Black, marginL, y);
                y += 22;

                double success = AcceptanceRate;
                double reject = 100.0 - success;

                gfx.DrawString($"Prihvaćeni zahtevi: {AcceptedCount}", textFont, XBrushes.Black, marginL + 20, y); y += 18;
                gfx.DrawString($"Neprihvaćeni zahtevi: {NotAcceptedCount}", textFont, XBrushes.Black, marginL + 20, y); y += 18;
                gfx.DrawString($"Procenat uspešnosti (prihvaćeni): {success:F1}%", textFont, XBrushes.Black, marginL + 20, y); y += 18;
                gfx.DrawString($"Procenat neuspješnih (neprihvaćeni): {reject:F1}%", textFont, XBrushes.Black, marginL + 20, y); y += 18;
                gfx.DrawString($"Prosečan broj ljudi u prihvaćenim zahtevima: {AveragePeopleCount:F1}", textFont, XBrushes.Black, marginL + 20, y);
                y += 28;

                gfx.DrawString("Statistika po jeziku", headerFont, XBrushes.Black, marginL, y);
                y += 22;

                gfx.DrawString("Jezik", textFont, XBrushes.Black, marginL, y);
                gfx.DrawString("Broj zahteva", textFont, XBrushes.Black, marginL + 380, y);
                y += 16;

                double originX = marginL + 160;
                double maxBarWidth = 260;
                int maxLang = LanguageStatistics.Any() ? LanguageStatistics.Max(l => l.Count) : 0;
                double scaleLang = maxLang > 0 ? maxBarWidth / maxLang : 0;

                for (int tick = 0; tick <= maxLang; tick++)
                {
                    double xTick = originX + tick * scaleLang;
                    gfx.DrawLine(new XPen(XColors.LightGray, 0.5), xTick, y, xTick, y + LanguageStatistics.Count * 24 + 8);
                }

                foreach (var lang in LanguageStatistics)
                {
                    gfx.DrawString($"{lang.Language}: {lang.Count}", textFont, XBrushes.Black, marginL + 20, y + 11);
                    double w = lang.Count * scaleLang;
                    var bar = new XRect(originX, y, w, 16);
                    gfx.DrawRectangle(XBrushes.SteelBlue, bar);
                    y += 24;
                }
                y += 12;

                gfx.DrawString("Statistika po lokaciji", headerFont, XBrushes.Black, marginL, y);
                y += 22;

                gfx.DrawString("Lokacija", textFont, XBrushes.Black, marginL, y);
                gfx.DrawString("Broj zahteva", textFont, XBrushes.Black, marginL + 380, y);
                y += 16;

                int maxLoc = LocationStatistics.Any() ? LocationStatistics.Max(l => l.Count) : 0;
                double scaleLoc = maxLoc > 0 ? maxBarWidth / maxLoc : 0;

                for (int tick = 0; tick <= maxLoc; tick++)
                {
                    double xTick = originX + tick * scaleLoc;
                    gfx.DrawLine(new XPen(XColors.LightGray, 0.5), xTick, y, xTick, y + LocationStatistics.Count * 24 + 8);
                }

                foreach (var loc in LocationStatistics)
                {
                    gfx.DrawString($"{loc.Location}: {loc.Count}", textFont, XBrushes.Black, marginL + 20, y + 11);
                    double w = loc.Count * scaleLoc;
                    var bar = new XRect(originX, y, w, 16);
                    gfx.DrawRectangle(XBrushes.Orange, bar);
                    y += 24;
                }

                string userInfo = $"Korisnik: {(_currentUserName ?? "N/A")}";
                string footer = $"Generisano: {DateTime.Now:dd.MM.yyyy. HH:mm}   |   {userInfo}";
                gfx.DrawString(footer, smallFont, XBrushes.Gray,
                    new XRect(marginL, page.Height - 36, page.Width - 2 * marginL, 20), XStringFormats.TopLeft);

                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string filePath = Path.Combine(documentsPath, $"TourStatistics_{SelectedYear}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
                document.Save(filePath);

                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška prilikom generisanja izveštaja:\n{ex.Message}", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class LanguageStatistic
    {
        public string Language { get; set; }
        public int Count { get; set; }
        public double BarWidth { get; set; }
    }

    public class LocationStatistic
    {
        public string Location { get; set; }
        public int Count { get; set; }
        public double BarWidth { get; set; }
    }
}