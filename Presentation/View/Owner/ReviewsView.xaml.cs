using BookingApp.Services;
using BookingApp.Services.DTO;
using BookingApp.Utilities;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace BookingApp.Presentation.View.Owner
{
    public partial class ReviewsView : UserControl
    {
        private List<string> _currentGalleryImages;
        private int _currentImageIndex;
        private readonly ImagePathConverterReview _imageConverter;

        public ReviewsView()
        {
            InitializeComponent();
            _imageConverter = new ImagePathConverterReview();
            var viewModel = Injector.CreateReviewsViewModel();
            DataContext = viewModel;
        }

        private void ImagePreview_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is ReviewDisplayDTO review)
            {
                if (!string.IsNullOrEmpty(review.ImagePaths))
                {
                    var images = review.ImagePaths
                        .Split(';', StringSplitOptions.RemoveEmptyEntries);

                    _currentGalleryImages = new List<string>(images);
                    _currentImageIndex = 0;
                    UpdateGalleryImage();
                    GalleryOverlay.Visibility = Visibility.Visible;
                }
            }
        }

        private void UpdateGalleryImage()
        {
            if (_currentGalleryImages == null || _currentGalleryImages.Count == 0) return;

            var imagePath = _currentGalleryImages[_currentImageIndex];
            var convertedPath = _imageConverter.Convert(imagePath, null, null, null) as string;

            if (!string.IsNullOrEmpty(convertedPath))
            {
                MainGalleryImage.Source = new BitmapImage(new Uri(convertedPath, UriKind.Absolute));
            }

            ImageCounter.Text = $"{_currentImageIndex + 1} / {_currentGalleryImages.Count}";
        }

        private void PreviousImage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentGalleryImages == null || _currentGalleryImages.Count == 0) return;

            _currentImageIndex--;
            if (_currentImageIndex < 0)
                _currentImageIndex = _currentGalleryImages.Count - 1;

            UpdateGalleryImage();
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentGalleryImages == null || _currentGalleryImages.Count == 0) return;

            _currentImageIndex++;
            if (_currentImageIndex >= _currentGalleryImages.Count)
                _currentImageIndex = 0;

            UpdateGalleryImage();
        }

        private void CloseGallery_Background(object sender, MouseButtonEventArgs e)
        {
            GalleryOverlay.Visibility = Visibility.Collapsed;
            _currentGalleryImages = null;
        }

        private void CloseGalleryButton_Click(object sender, RoutedEventArgs e)
        {
            GalleryOverlay.Visibility = Visibility.Collapsed;
            _currentGalleryImages = null;
        }

        private void PreventClose(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}