using BookingApp.Domain.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System;
using BookingApp.Utilities;
using BookingApp.Repositories;

namespace BookingApp.Presentation.View.Guide
{
    public class DetailedTourViewModel : INotifyPropertyChanged
    {
        public Tour Tour { get; }

        private int _currentImageIndex;
        private bool _isOverlayVisible;
        private BitmapImage _largeImage;

        public ObservableCollection<Images> Images { get; set; }
        public ObservableCollection<StartTourTime> StartTimes { get; set; }
        public ObservableCollection<KeyPoint> KeyPoints { get; set; }

        public Images SelectedImage { get; set; }

        public bool IsOverlayVisible
        {
            get => _isOverlayVisible;
            set { _isOverlayVisible = value; OnPropertyChanged(); }
        }

        public BitmapImage LargeImage
        {
            get => _largeImage;
            set { _largeImage = value; OnPropertyChanged(); }
        }

        public ICommand OpenImageCommand { get; }
        public ICommand CloseOverlayCommand { get; }
        public ICommand NextImageCommand { get; }
        public ICommand PrevImageCommand { get; }

        private string _guideName;
        public string GuideName
        {
            get => _guideName;
            set
            {
                _guideName = value;
                OnPropertyChanged();
            }
        }

        public DetailedTourViewModel(Tour tour)
        {
            Tour = tour;

            OpenImageCommand = new RelayCommand(OpenImage);
            CloseOverlayCommand = new RelayCommand(_ => CloseOverlay());
            NextImageCommand = new RelayCommand(_ => NextImage());
            PrevImageCommand = new RelayCommand(_ => PrevImage());

            UserRepository repo = new UserRepository();
            var user = repo.GetById(tour.GuideId);

            GuideName = user.FirstName + " " + user.LastName;

            Images = new ObservableCollection<Images>(tour.Images);
            StartTimes = new ObservableCollection<StartTourTime>(tour.StartTimes);
            KeyPoints = new ObservableCollection<KeyPoint>(tour.KeyPoints);
        }

        private void OpenImage(object parameter)
        {
            if (parameter is Images img && Images != null && Images.Count > 0)
            {
                _currentImageIndex = Images.IndexOf(img);
                ShowImage();
                IsOverlayVisible = true;
            }
        }

        private void ShowImage()
        {
            if (_currentImageIndex >= 0 && _currentImageIndex < Images.Count)
            {
                var img = Images[_currentImageIndex];
                LargeImage = new BitmapImage(new Uri(img.FullPath, UriKind.RelativeOrAbsolute));
            }
        }

        private void NextImage()
        {
            if (Images == null || Images.Count == 0) return;
            _currentImageIndex = (_currentImageIndex + 1) % Images.Count;
            ShowImage();
        }

        private void PrevImage()
        {
            if (Images == null || Images.Count == 0) return;
            _currentImageIndex = (_currentImageIndex - 1 + Images.Count) % Images.Count;
            ShowImage();
        }

        private void CloseOverlay()
        {
            IsOverlayVisible = false;
            LargeImage = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
