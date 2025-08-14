using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace BookingApp.Presentation.Owner
{
   
    public partial class ImageGalleryView : UserControl
    {
        private List<string> _imagePaths = new List<string>();
        private int _currentIndex = 0;

        public ImageGalleryView()
        {
            InitializeComponent();
        }

        public void SetImages(List<string> images)
        {
            _imagePaths = images ?? new List<string>();
            _currentIndex = 0;
            ShowImage();
            this.Focus();  // fokus da hvata tastaturu odmah
        }

        private void ShowImage()
        {
            if (_imagePaths.Count == 0)
            {
                DisplayedImage.Source = null;
                return;
            }

            try
            {

                string baseDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images");
                string fullPath = System.IO.Path.Combine(baseDir, _imagePaths[_currentIndex]);

                var bitmap = new BitmapImage(new Uri(fullPath, UriKind.Absolute));
                DisplayedImage.Source = bitmap;
            }
            catch
            {
                DisplayedImage.Source = null;
            }
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (_imagePaths.Count == 0) return;
            _currentIndex = (_currentIndex - 1 + _imagePaths.Count) % _imagePaths.Count;
            ShowImage();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_imagePaths.Count == 0) return;
            _currentIndex = (_currentIndex + 1) % _imagePaths.Count;
            ShowImage();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                PrevButton_Click(null, null);
                e.Handled = true;
            }
            else if (e.Key == Key.Right)
            {
                NextButton_Click(null, null);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                CloseButton_Click(null, null);
                e.Handled = true;
            }
        }
    }
}
