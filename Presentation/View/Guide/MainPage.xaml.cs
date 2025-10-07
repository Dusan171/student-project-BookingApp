using BookingApp.Presentation.ViewModel.Guide;
using BookingApp.Utilities;
using BookingApp.View;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BookingApp.Presentation.View.Guide
{
    public partial class MainPage : Page
    {
        private bool _areTooltipsEnabled = true;
        public MainPage()
        {
            InitializeComponent();

            MainFrame.Navigate(new ToursControl());
         
        }

        private void Back_Button(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
                MainFrame.GoBack();
        }

        private void ToursButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ToursControl());
        }

        private void ReviewsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ReviewsControl());
        }

        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new StatisticsSelectionControl());
        }

        private void RequestButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TourRequestsPage());
        }

        private void NavigateBack()
        {
            if (MainFrame.CanGoBack)
                MainFrame.GoBack();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Session.CurrentUser = null;
                var signIn = new SignInForm();
                signIn.Show();
                Window.GetWindow(this)?.Close();
            }
        }
        private void ToolTipButton_Click(object sender, RoutedEventArgs e)
        {
            _areTooltipsEnabled = !_areTooltipsEnabled;

            ToolTipToggleButton.Foreground = _areTooltipsEnabled
                ? new SolidColorBrush(Colors.Yellow)  
                : new SolidColorBrush(Colors.White);  

            foreach (Window window in Application.Current.Windows)
            {
                foreach (var child in FindVisualChildren<FrameworkElement>(window))
                {
                    ToolTipService.SetIsEnabled(child, _areTooltipsEnabled);
                }
            }
        }


        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
           // MainFrame.Navigate(new HelpPage());
        }

        private void WizardButton_Click(object sender, RoutedEventArgs e)
        {
           // MainFrame.Navigate(new WizardPage());
        }
    }
}
