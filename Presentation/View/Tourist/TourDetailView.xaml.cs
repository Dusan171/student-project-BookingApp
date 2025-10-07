using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BookingApp.Presentation.ViewModel.Tourist;
using BookingApp.Services.DTO;

namespace BookingApp.Presentation.View.Tourist
{
    public partial class TourDetailView : UserControl
    {
        public TourDetailViewModel ViewModel => (TourDetailViewModel)DataContext;
        public event Action BackRequested;
        public event Action<TourDTO> TourReserveRequested;

        private int _currentImageIndex = 0;

        public TourDetailView()
        {
            InitializeComponent();
            DataContext = new TourDetailViewModel();
            SetupEventHandlers();

            
            this.Focusable = true;
            this.KeyDown += TourDetailView_KeyDown;
        }

        private void SetupEventHandlers()
        {
            if (ViewModel != null)
            {
                ViewModel.BackRequested += OnBackRequested;
                ViewModel.TourReserveRequested += OnTourReserveRequested;
            }
        }

        public void SetTour(TourDTO tour)
        {
            ViewModel?.SetTour(tour);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackRequested?.Invoke();
        }

        private void OnBackRequested()
        {
            BackRequested?.Invoke();
        }

        private void OnTourReserveRequested(TourDTO tour)
        {
            TourReserveRequested?.Invoke(tour);
        }

        public void Cleanup()
        {
            if (ViewModel != null)
            {
                ViewModel.BackRequested -= OnBackRequested;
                ViewModel.TourReserveRequested -= OnTourReserveRequested;
            }
        }

      
        private void MainImage_Click(object sender, MouseButtonEventArgs e)
        {
            OpenImageOverlay(0);
        }

        private void Thumbnail1_Click(object sender, MouseButtonEventArgs e)
        {
            OpenImageOverlay(1);
        }

        private void Thumbnail2_Click(object sender, MouseButtonEventArgs e)
        {
            OpenImageOverlay(2);
        }

        private void ShowAllImages_Click(object sender, MouseButtonEventArgs e)
        {
            OpenImageOverlay(3);
        }

       
        private void OpenImageOverlay(int imageIndex)
        {
            if (ViewModel?.Images == null || ViewModel.Images.Count == 0)
            {
                MessageBox.Show("Nema dostupnih slika za prikaz.",
                               "Galerija slika",
                               MessageBoxButton.OK,
                               MessageBoxImage.Information);
                return;
            }

            _currentImageIndex = Math.Max(0, Math.Min(imageIndex, ViewModel.Images.Count - 1));
            ShowCurrentImage();
            UpdateNavigationButtons();
            ImageOverlay.Visibility = Visibility.Visible;

            
            ImageOverlay.Focus();
        }

        
        private void ShowCurrentImage()
        {
            if (ViewModel?.Images == null || _currentImageIndex < 0 || _currentImageIndex >= ViewModel.Images.Count)
                return;

            var currentImage = ViewModel.Images[_currentImageIndex];

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(currentImage.Path, UriKind.RelativeOrAbsolute);
                bitmap.EndInit();

                OverlayMainImage.Source = bitmap;
                OverlayImageTitle.Text = $"Slika {_currentImageIndex + 1}";
            }
            catch (Exception)
            {
               
                OverlayMainImage.Source = null;
                OverlayImageTitle.Text = $"Slika {_currentImageIndex + 1} - Greška prilikom učitavanja";
            }

            OverlayPositionIndicator.Text = $"{_currentImageIndex + 1} / {ViewModel.Images.Count}";
        }

        
        private void UpdateNavigationButtons()
        {
            OverlayPreviousButton.IsEnabled = _currentImageIndex > 0;
            OverlayNextButton.IsEnabled = _currentImageIndex < ViewModel.Images.Count - 1;

            
            if (ViewModel.Images.Count <= 1)
            {
                OverlayPreviousButton.Visibility = Visibility.Collapsed;
                OverlayNextButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                OverlayPreviousButton.Visibility = Visibility.Visible;
                OverlayNextButton.Visibility = Visibility.Visible;
            }
        }

        
        private void CloseOverlay_Click(object sender, RoutedEventArgs e)
        {
            CloseImageOverlay();
        }

        private void PreviousImage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentImageIndex > 0)
            {
                _currentImageIndex--;
                ShowCurrentImage();
                UpdateNavigationButtons();
            }
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentImageIndex < ViewModel.Images.Count - 1)
            {
                _currentImageIndex++;
                ShowCurrentImage();
                UpdateNavigationButtons();
            }
        }

        private void ImageOverlay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            if (e.OriginalSource == ImageOverlay)
            {
                CloseImageOverlay();
            }
        }

        
        private void TourDetailView_KeyDown(object sender, KeyEventArgs e)
        {
            if (ImageOverlay.Visibility == Visibility.Visible)
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        CloseImageOverlay();
                        e.Handled = true;
                        break;
                    case Key.Left:
                        if (_currentImageIndex > 0)
                        {
                            _currentImageIndex--;
                            ShowCurrentImage();
                            UpdateNavigationButtons();
                        }
                        e.Handled = true;
                        break;
                    case Key.Right:
                        if (_currentImageIndex < ViewModel.Images.Count - 1)
                        {
                            _currentImageIndex++;
                            ShowCurrentImage();
                            UpdateNavigationButtons();
                        }
                        e.Handled = true;
                        break;
                }
            }
        }

        
        private void CloseImageOverlay()
        {
            ImageOverlay.Visibility = Visibility.Collapsed;
            OverlayMainImage.Source = null; 
        }
    }
}