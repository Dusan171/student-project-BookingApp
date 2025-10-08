using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using BookingApp.Domain.Interfaces.ServiceInterfaces;
using BookingApp.Services.DTO;
using BookingApp.Utilities;
using System.Linq;
using BookingApp.Domain.Interfaces;

namespace BookingApp.Presentation.ViewModel.Owner
{
    public class PDFSettingsViewModel : INotifyPropertyChanged
    {
        private readonly Action _closeWindowAction;
        private readonly IPDFReportService _pdfReportService;
        private readonly IAccommodationService _accommodationService;
        private readonly IAccommodationReviewService _accommodationReviewService;
        private DateTime? _fromDate;
        private DateTime? _toDate;

        public DateTime? FromDate
        {
            get => _fromDate;
            set
            {
                _fromDate = value;
                OnPropertyChanged();
                ((RelayCommand)GenerateCommand).RaiseCanExecuteChanged();
            }
        }

        public DateTime? ToDate
        {
            get => _toDate;
            set
            {
                _toDate = value;
                OnPropertyChanged();
                ((RelayCommand)GenerateCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand GenerateCommand { get; }
        public ICommand CancelCommand { get; }

        public PDFSettingsViewModel(Action closeWindowAction, IPDFReportService pdfReportService, IAccommodationService accommodationService, IAccommodationReviewService accommodationReviewService)
        {
            _closeWindowAction = closeWindowAction;
            _pdfReportService = pdfReportService;
            _accommodationService = accommodationService;
            _accommodationReviewService = accommodationReviewService;

            GenerateCommand = new RelayCommand(ExecuteGenerate, CanExecuteGenerate);
            CancelCommand = new RelayCommand(ExecuteCancel);

            FromDate = DateTime.Today.AddMonths(-1);
            ToDate = DateTime.Today;
        }

        private bool CanExecuteGenerate(object parameter)
        {
            return FromDate.HasValue && ToDate.HasValue && FromDate.Value <= ToDate.Value;
        }

        private void ExecuteGenerate(object parameter)
        {
            var ownerId = Session.CurrentUser.Id;
            var accommodations = _accommodationService.GetByOwnerId(ownerId);
            var ratings = new System.Collections.Generic.List<AccommodationRatingDTO>();

            foreach (var accommodation in accommodations)
            {
                var reviews = _accommodationReviewService.GetReviewsByAccommodationId(accommodation.Id);
                var filteredReviews = reviews.Where(r => r.CreatedAt >= FromDate.Value && r.CreatedAt <= ToDate.Value).ToList();

                if (filteredReviews.Any())
                {
                    var rating = new AccommodationRatingDTO{ AccommodationName = accommodation.Name, Location = _accommodationService.GetLocationString(accommodation.GeoLocation.Id),AverageCleanlinessRating = filteredReviews.Average(r => r.CleanlinessRating), AverageOwnerRating = filteredReviews.Average(r => r.OwnerRating),NumberOfReviews = filteredReviews.Count() };
                    ratings.Add(rating);
                }
            }
            string pdfFilePath = null;
            try
            {
                MessageBox.Show("Generating PDF Report . . . ", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                string pdfPath=_pdfReportService.GenerateAccommodationRatingsReport(ratings, FromDate.Value, ToDate.Value, Session.CurrentUser.Username);
                MessageBox.Show("PDF report generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                if (!string.IsNullOrEmpty(pdfPath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(pdfPath) { UseShellExecute = true });
                }
                _closeWindowAction();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while generating the PDF: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _closeWindowAction();
            }
        }

        private void ExecuteCancel(object parameter)
        {
            _closeWindowAction();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}