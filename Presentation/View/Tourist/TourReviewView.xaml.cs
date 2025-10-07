using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Tourist;
using System.Windows;
using System.Windows.Media;

namespace BookingApp.Presentation.View.Tourist
{
    public partial class TourReviewView : UserControl
    {
        public TourReviewView()
        {
            InitializeComponent();
            var viewModel = new TourReviewViewModel();
            DataContext = viewModel;

            // Pretplati se na web-like notifikacije
            viewModel.ShowSuccessMessage += ShowSuccessNotification;
            viewModel.ShowErrorMessage += ShowErrorNotification;
        }

        public TourReviewViewModel? ViewModel => DataContext as TourReviewViewModel;

        public void LoadCompletedTours()
        {
            ViewModel?.LoadCompletedTours();
        }

        // Web-like success notifikacija
        private void ShowSuccessNotification(string message)
        {
            // Kreiranje success overlay-a
            var overlay = new Grid
            {
                Background = new SolidColorBrush(Color.FromArgb(136, 0, 0, 0)),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Name = "NotificationOverlay"
            };

            var successPanel = new Border
            {
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(39, 174, 96)), // #27AE60
                BorderThickness = new Thickness(2, 2, 2, 2),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(25, 25, 25, 25),
                MaxWidth = 400,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var stackPanel = new StackPanel();

            // Header
            var headerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 15)
            };

            var checkIcon = new TextBlock
            {
                Text = "✓",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96)),
                Margin = new Thickness(0, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            var headerText = new TextBlock
            {
                Text = "USPEŠNO SAČUVANO",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96)),
                VerticalAlignment = VerticalAlignment.Center
            };

            headerPanel.Children.Add(checkIcon);
            headerPanel.Children.Add(headerText);

            // Poruka
            var messageText = new TextBlock
            {
                Text = message,
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141)),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 20)
            };

            // Dugme
            var closeButton = new Button
            {
                Content = "ZATVORI",
                Background = new SolidColorBrush(Color.FromRgb(39, 174, 96)),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                Padding = new Thickness(20, 8, 20, 8),
                BorderThickness = new Thickness(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Right,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            // Style za dugme
            var buttonStyle = new Style(typeof(Button));
            var template = new ControlTemplate(typeof(Button));
            var border = new FrameworkElementFactory(typeof(Border));
            border.SetBinding(Border.BackgroundProperty, new System.Windows.Data.Binding("Background")
            { RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent) });
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(4));
            border.SetBinding(Border.PaddingProperty, new System.Windows.Data.Binding("Padding")
            { RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent) });

            var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            border.AppendChild(contentPresenter);
            template.VisualTree = border;
            buttonStyle.Setters.Add(new Setter(Button.TemplateProperty, template));
            closeButton.Style = buttonStyle;

            closeButton.Click += (s, e) => {
                if (this.Parent is Grid parentGrid)
                {
                    parentGrid.Children.Remove(overlay);
                }
            };

            stackPanel.Children.Add(headerPanel);
            stackPanel.Children.Add(messageText);
            stackPanel.Children.Add(closeButton);
            successPanel.Child = stackPanel;
            overlay.Children.Add(successPanel);

            // Dodaj na parent
            if (this.Parent is Grid parent)
            {
                parent.Children.Add(overlay);
            }

            // Auto-close nakon 3 sekunde
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = System.TimeSpan.FromSeconds(3);
            timer.Tick += (s, e) => {
                timer.Stop();
                if (this.Parent is Grid parentGrid)
                {
                    parentGrid.Children.Remove(overlay);
                }
            };
            timer.Start();
        }

        // Web-like error notifikacija
        private void ShowErrorNotification(string message)
        {
            var overlay = new Grid
            {
                Background = new SolidColorBrush(Color.FromArgb(136, 0, 0, 0)),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Name = "ErrorOverlay"
            };

            var errorPanel = new Border
            {
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(231, 76, 60)), // #E74C3C
                BorderThickness = new Thickness(2, 2, 2, 2),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(25, 25, 25, 25),
                MaxWidth = 400,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var stackPanel = new StackPanel();

            // Header
            var headerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 15)
            };

            var errorIcon = new TextBlock
            {
                Text = "!",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60)),
                Margin = new Thickness(0, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            var headerText = new TextBlock
            {
                Text = "GREŠKA",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60)),
                VerticalAlignment = VerticalAlignment.Center
            };

            headerPanel.Children.Add(errorIcon);
            headerPanel.Children.Add(headerText);

            // Poruka
            var messageText = new TextBlock
            {
                Text = message,
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141)),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 20)
            };

            // Dugme
            var closeButton = new Button
            {
                Content = "ZATVORI",
                Background = new SolidColorBrush(Color.FromRgb(149, 165, 166)),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                Padding = new Thickness(20, 8, 20, 8),
                BorderThickness = new Thickness(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Right,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            // Style za dugme (isti kao success)
            var buttonStyle = new Style(typeof(Button));
            var template = new ControlTemplate(typeof(Button));
            var border = new FrameworkElementFactory(typeof(Border));
            border.SetBinding(Border.BackgroundProperty, new System.Windows.Data.Binding("Background")
            { RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent) });
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(4));
            border.SetBinding(Border.PaddingProperty, new System.Windows.Data.Binding("Padding")
            { RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent) });

            var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            border.AppendChild(contentPresenter);
            template.VisualTree = border;
            buttonStyle.Setters.Add(new Setter(Button.TemplateProperty, template));
            closeButton.Style = buttonStyle;

            closeButton.Click += (s, e) => {
                if (this.Parent is Grid parentGrid)
                {
                    parentGrid.Children.Remove(overlay);
                }
            };

            stackPanel.Children.Add(headerPanel);
            stackPanel.Children.Add(messageText);
            stackPanel.Children.Add(closeButton);
            errorPanel.Child = stackPanel;
            overlay.Children.Add(errorPanel);

            // Dodaj na parent
            if (this.Parent is Grid parent)
            {
                parent.Children.Add(overlay);
            }
        }
    }
}