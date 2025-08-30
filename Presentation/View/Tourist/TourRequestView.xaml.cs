using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Interfaces.ServiceInterfaces;
using BookingApp.Presentation.ViewModel.Tourist;
using BookingApp.Services;

namespace BookingApp.Presentation.View.Tourist
{
    public partial class TourRequestsView : UserControl
    {
        private TourRequestViewModel _viewModel;

        public TourRequestsView()
        {
            InitializeComponent();
        }

        public void InitializeViewModel(int userId)
        {
            var requestService = Injector.CreateInstance<ITourRequestService>();
            _viewModel = new TourRequestViewModel(requestService, userId);
            DataContext = _viewModel;
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TourRequestViewModel.IsCreatingRequest))
            {
                if (_viewModel.IsCreatingRequest)
                {
                    RequestsListView.Visibility = Visibility.Collapsed;
                    CreateRequestView.Visibility = Visibility.Visible;
                }
                else
                {
                    RequestsListView.Visibility = Visibility.Visible;
                    CreateRequestView.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = new SolidColorBrush(Color.FromRgb(240, 248, 255));
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(46, 134, 193));
                border.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Gray,
                    Direction = 315,
                    ShadowDepth = 3,
                    Opacity = 0.3
                };
            }
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = new SolidColorBrush(Color.FromRgb(248, 249, 250));
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(233, 236, 239));
                border.Effect = null;
            }
        }
    }
}