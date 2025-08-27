using BookingApp.Domain.Interfaces;
using BookingApp.Services;
using BookingApp.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace BookingApp.Presentation.ViewModel.Owner
{
    public class ImageGalleryViewModel: INotifyPropertyChanged
    {
        private readonly IAccommodationImageService _service;
        private List<string> _imagePaths;
        private int _currentIndex;

        public event Action CloseRequested;
        public event PropertyChangedEventHandler PropertyChanged;

        public ImageGalleryViewModel(IAccommodationImageService service, List<string> images = null)
        {
            _service = service;
            _imagePaths = images ?? new List<string>();
            _currentIndex = 0;

            PrevCommand = new RelayCommand(_ => ShowPrevImage(), _ => _imagePaths.Count > 0);
            NextCommand = new RelayCommand(_ => ShowNextImage(), _ => _imagePaths.Count > 0);
            CloseCommand = new RelayCommand(_ => CloseRequested?.Invoke());

            ShowImage();
        }

        private BitmapImage _currentImage;
        public BitmapImage CurrentImage
        {
            get => _currentImage;
            set
            {
                _currentImage = value;
                OnPropertyChanged(nameof(CurrentImage));
            }
        }

        public ICommand PrevCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand CloseCommand { get; }

        private void ShowImage()
        {
            if (_imagePaths.Count == 0)
            {
                CurrentImage = null;
                return;
            }

            CurrentImage = _service.LoadImage(_imagePaths[_currentIndex]);
        }

        private void ShowPrevImage()
        {
            _currentIndex = (_currentIndex - 1 + _imagePaths.Count) % _imagePaths.Count;
            ShowImage();
        }

        private void ShowNextImage()
        {
            _currentIndex = (_currentIndex + 1) % _imagePaths.Count;
            ShowImage();
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void SetImages(List<string> images)
        {
            _imagePaths = images ?? new List<string>();
            _currentIndex = 0;
            ShowImage();
        }
    }
}

