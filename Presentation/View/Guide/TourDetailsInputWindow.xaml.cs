using System.Windows;
using BookingApp.Presentation.ViewModel.Guide;

namespace BookingApp.Presentation.View.Guide
{
    public partial class TourDetailsInputWindow : Window
    {
        public TourDetailsInputWindow(int suggestedMaxTourists)
        {
            InitializeComponent();
            var viewModel = new TourDetailsInputViewModel(suggestedMaxTourists);
            DataContext = viewModel;
            
            viewModel.RequestClose += (sender, e) => {
                DialogResult = viewModel.DialogResult;
                Close();
            };
        }

        public TourDetailsInputViewModel ViewModel => (TourDetailsInputViewModel)DataContext;
    }
}