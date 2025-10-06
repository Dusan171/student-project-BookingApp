using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.View.Owner
{
    public partial class AccommodationsView : UserControl
    {
        private List<AccommodationImageDTO> _currentImages;
        private int _currentImageIndex = 0;

        public AccommodationsView()
        {
            InitializeComponent();
        }

        private void ImageBorder_Click(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border?.Tag is AccommodationDTO accommodation && accommodation.ImagePaths?.Count > 0)
            {
                OpenGallery(accommodation.ImagePaths);
            }
        }

        private void OpenGallery(List<AccommodationImageDTO> images)
        {
            _currentImages = images;
            _currentImageIndex = 0;
            ShowImage(_currentImageIndex);
            GalleryOverlay.Visibility = Visibility.Visible;
            GalleryOverlay.Focus();
        }

        private void PreviousImage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentImages == null) return;
            _currentImageIndex--;
            if (_currentImageIndex < 0)
                _currentImageIndex = _currentImages.Count - 1;
            ShowImage(_currentImageIndex);
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentImages == null) return;
            _currentImageIndex++;
            if (_currentImageIndex >= _currentImages.Count)
                _currentImageIndex = 0;
            ShowImage(_currentImageIndex);
        }

        private void CloseGallery(object sender, MouseButtonEventArgs e)
        {
            GalleryOverlay.Visibility = Visibility.Collapsed;
            _currentImages = null;
            _currentImageIndex = 0;
        }

        private void CloseGalleryButton_Click(object sender, RoutedEventArgs e)
        {
            GalleryOverlay.Visibility = Visibility.Collapsed;
            _currentImages = null;
            _currentImageIndex = 0;
        }

        private void ShowImage(int index)
        {
            if (_currentImages == null || index < 0 || index >= _currentImages.Count)
                return;

            var converter = new ImagePathConverter();
            MainGalleryImage.Source = converter.Convert(_currentImages[index].Path, null, null, null) as BitmapImage;
            _currentImageIndex = index;
            ImageCounter.Text = $"{index + 1} / {_currentImages.Count}";
        }

        private void PreventClose(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}