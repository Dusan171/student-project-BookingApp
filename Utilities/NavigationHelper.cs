using System;
using System.Windows;
using System.Windows.Controls;
using BookingApp.Presentation.View;
using BookingApp.Presentation.View.Tourist;

namespace BookingApp.Utilities
{
    public static class NavigationHelper
    {
        private static ContentPresenter? _mainContentPresenter;

        public static void Initialize(ContentPresenter contentPresenter)
        {
            _mainContentPresenter = contentPresenter;
        }

        public static void NavigateTo(Type pageType)
        {
            if (_mainContentPresenter == null)
            {
                throw new InvalidOperationException("NavigationHelper not initialized. Call Initialize() first.");
            }
            try
            {
                var page = Activator.CreateInstance(pageType) as UserControl;
                if (page != null)
                {
                    _mainContentPresenter.Content = page;
                }
                else
                {
                    throw new ArgumentException($"Type {pageType.Name} is not a UserControl");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Navigation error: {ex.Message}", "Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void NavigateToTourSearch()
        {
            NavigateTo(typeof(TourSearchView));
        }

        public static void NavigateToWithParameter(Type pageType, object parameter)
        {
            if (_mainContentPresenter == null)
            {
                throw new InvalidOperationException("NavigationHelper not initialized. Call Initialize() first.");
            }
            try
            {
                var page = Activator.CreateInstance(pageType) as UserControl;
                if (page != null)
                {
                    var parameterMethod = pageType.GetMethod("SetParameter");
                    parameterMethod?.Invoke(page, new[] { parameter });
                    _mainContentPresenter.Content = page;
                }
                else
                {
                    throw new ArgumentException($"Type {pageType.Name} is not a UserControl");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Navigation error: {ex.Message}", "Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static UserControl? GetCurrentPage()
        {
            return _mainContentPresenter?.Content as UserControl;
        }

        public static bool IsCurrentPage(Type pageType)
        {
            var currentPage = GetCurrentPage();
            return currentPage?.GetType() == pageType;
        }
    }
}