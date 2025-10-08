using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Guide;

namespace BookingApp.Presentation.View.Guide
{
    public partial class ToursControl : Page
    {
        private readonly ToursControlViewModel _viewModel;
        private string _currentPdfPath;

        public ToursControl()
        {
            InitializeComponent();
            _viewModel = new ToursControlViewModel();
            DataContext = _viewModel;
            _viewModel.NavigationRequested += NavigateToPage;
            _viewModel.PDFGenerated += ShowPDFOverlay;
        }

        private void NavigateToPage(Page page)
        {
            NavigationService?.Navigate(page);
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            var startDate = ReportGenerator.StartDate;
            var endDate = ReportGenerator.EndDate;

            if (!startDate.HasValue || !endDate.HasValue)
            {
                MessageBox.Show("Molimo odaberite oba datuma za generisanje izveštaja.", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (startDate.Value > endDate.Value)
            {
                MessageBox.Show("Datum početka mora biti pre datuma kraja.", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var parameters = new object[] { startDate.Value, endDate.Value };
            _viewModel.GenerateReportCommand.Execute(parameters);
        }

        private void ShowPDFOverlay(string pdfPath)
        {
            _currentPdfPath = pdfPath;

            try
            {
                if (File.Exists(pdfPath))
                {
                    string fileUrl = new Uri(pdfPath).AbsoluteUri + "#zoom=40";
                    PdfModal.WebBrowser.Navigate(fileUrl);
                    PdfModal.Overlay.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("PDF fajl nije pronađen.", "Greška",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška prilikom učitavanja PDF-a: {ex.Message}\nPDF će biti otvoren spoljašnje.",
                    "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);

                try
                {
                    Process.Start(new ProcessStartInfo(pdfPath) { UseShellExecute = true });
                }
                catch
                {
                    MessageBox.Show("Ne mogu da otvorim PDF fajl.", "Greška",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClosePDFOverlay_Click(object sender, RoutedEventArgs e)
        {
            PdfModal.Overlay.Visibility = Visibility.Collapsed;

            try
            {
                PdfModal.WebBrowser.Navigate("about:blank");
            }
            catch
            {
                // Silent failure for navigate
            }
        }

        private void OpenPDFExternal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(_currentPdfPath) && File.Exists(_currentPdfPath))
                {
                    Process.Start(new ProcessStartInfo(_currentPdfPath) { UseShellExecute = true });
                }
                else
                {
                    MessageBox.Show("PDF fajl nije pronađen.", "Greška",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška prilikom otvaranja PDF-a: {ex.Message}",
                    "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}