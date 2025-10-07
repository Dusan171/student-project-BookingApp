using System;
using System.Windows;
using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Tourist;
using BookingApp.Services;
using BookingApp.Services.DTO;

namespace BookingApp.Presentation.View.Tourist
{
    public partial class TourPartsDetailView : UserControl
    {
        public TourPartsDetailViewModel ViewModel => DataContext as TourPartsDetailViewModel;

        
        public event EventHandler BackToListRequested;

        public TourPartsDetailView()
        {
            InitializeComponent();
        }

        public void InitializeViewModel(object selectedRequest, int currentUserId)
        {
            try
            {
                
                var viewModel = new TourPartsDetailViewModel(selectedRequest as ComplexTourRequestDTO, currentUserId);
                DataContext = viewModel;

                
                viewModel.BackRequested += OnBackRequested;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri inicijalizaciji prikaza delova ture: {ex.Message}",
                               "Greška inicijalizacije", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnBackRequested()
        {
            
            BackToListRequested?.Invoke(this, EventArgs.Empty);
        }

        public void RefreshData()
        {
            try
            {
                ViewModel?.RefreshCommand?.Execute(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri osvežavanju podataka: {ex.Message}",
                               "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}