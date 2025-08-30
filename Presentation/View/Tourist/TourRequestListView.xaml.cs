using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BookingApp.Domain.Interfaces;
using BookingApp.Presentation.ViewModel.Tourist;
using BookingApp.Services;

namespace BookingApp.Presentation.View.Tourist
{
    public partial class TourRequestListView : UserControl
    {
        public TourRequestListView()
        {
            InitializeComponent();
        }

        public void InitializeViewModel(int userId)
        {
            var requestService = Injector.CreateInstance<ITourRequestService>();
            DataContext = new TourRequestViewModel(requestService, userId);
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
                    ShadowDepth = 5,
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