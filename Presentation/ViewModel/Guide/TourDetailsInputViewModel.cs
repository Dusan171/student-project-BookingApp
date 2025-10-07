using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using BookingApp.Domain.Model;
using BookingApp.Repositories;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Guide
{
    public class TourDetailsInputViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged(string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event EventHandler? RequestClose;

        private readonly KeyPointRepository _keyPointRepository = new();
        private readonly ImageRepository _imageRepository = new();

        private int _localKeyPointIdCounter = 0;
        private int _localImageIdCounter = 0;

        private string _maxTourists;
        public string MaxTourists
        {
            get => _maxTourists;
            set
            {
                if (int.TryParse(value, out int parsed))
                {
                    if (parsed < 1)
                    {
                        MessageBox.Show("Max Tourists must be greater than 0.");
                        return;
                    }
                    _maxTourists = parsed.ToString();
                }
                else
                {
                    _maxTourists = value;
                }
                OnPropertyChanged(nameof(MaxTourists));
            }
        }

        public string KeyPointInput { get; set; } = string.Empty;

        public ObservableCollection<KeyPoint> KeyPoints { get; } = new();
        public ObservableCollection<Images> Images { get; } = new();

        public KeyPoint? SelectedKeyPoint { get; set; }
        public Images? SelectedImage { get; set; }

        private bool _isImageViewerVisible;
        public bool IsImageViewerVisible { get => _isImageViewerVisible; set { _isImageViewerVisible = value; OnPropertyChanged(); } }

        private Images? _currentImage;
        public Images? CurrentImage { get => _currentImage; set { _currentImage = value; OnPropertyChanged(); } }

        private int _currentImageIndex = -1;

        public RelayCommand AddKeyPointCommand { get; }
        public RelayCommand RemoveKeyPointCommand { get; }
        public RelayCommand BrowseImageCommand { get; }
        public RelayCommand RemoveImageCommand { get; }
        public RelayCommand ConfirmCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand<Images> OpenImageViewerCommand { get; }
        public RelayCommand NextImageCommand { get; }
        public RelayCommand PreviousImageCommand { get; }
        public RelayCommand RemoveCurrentImageCommand { get; }
        public RelayCommand CloseImageViewerCommand { get; }

        public bool DialogResult { get; private set; }

        public TourDetailsInputViewModel(int suggestedMaxTourists)
        {
            MaxTourists = suggestedMaxTourists.ToString();

            AddKeyPointCommand = new RelayCommand(_ => AddKeyPoint());
            RemoveKeyPointCommand = new RelayCommand(p => RemoveKeyPoint(p as KeyPoint));
            BrowseImageCommand = new RelayCommand(_ => BrowseImage());
            RemoveImageCommand = new RelayCommand(_ => RemoveImage(SelectedImage));

            ConfirmCommand = new RelayCommand(_ => {
                if (CanConfirm())
                {
                    DialogResult = true;
                    RequestClose?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show("Please add at least 2 key points and set valid max tourists number.", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            });

            CancelCommand = new RelayCommand(_ => {
                DialogResult = false;
                RequestClose?.Invoke(this, EventArgs.Empty);
            });

            OpenImageViewerCommand = new RelayCommand<Images>(img => OpenImageViewer(img));
            NextImageCommand = new RelayCommand(_ => ShowNextImage());
            PreviousImageCommand = new RelayCommand(_ => ShowPreviousImage());
            RemoveCurrentImageCommand = new RelayCommand(_ => RemoveCurrentImage());
            CloseImageViewerCommand = new RelayCommand(_ => CloseImageViewer());
        }

        private bool CanConfirm()
        {
            return KeyPoints.Count >= 2 && int.TryParse(MaxTourists, out int max) && max > 0;
        }

        private void CloseImageViewer()
        {
            IsImageViewerVisible = false;
            CurrentImage = null;
        }

        private void AddKeyPoint()
        {
            string name = KeyPointInput?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(name)) return;
            if (KeyPoints.Any(k => k.Name == name))
            {
                MessageBox.Show("Key point already exists.");
                return;
            }

            var newKeyPoint = new KeyPoint { Id = _keyPointRepository.NextId() + _localKeyPointIdCounter++, Name = name };
            KeyPoints.Add(newKeyPoint);
            KeyPointInput = string.Empty;
            OnPropertyChanged(nameof(KeyPointInput));
        }

        private void RemoveKeyPoint(KeyPoint? kp)
        {
            if (!KeyPoints.Any()) { MessageBox.Show("You didn't add any key points."); return; }
            if (kp == null) { MessageBox.Show("You didn't chose a key point to remove."); return; }
            KeyPoints.Remove(kp);
            _localKeyPointIdCounter = Math.Max(0, _localKeyPointIdCounter - 1);
        }

        private void BrowseImage()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            if (dlg.ShowDialog() == true)
            {
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string relativeFolder = Path.Combine("Resources", "Data", "Images");
                string targetFolder = Path.Combine(appDirectory, relativeFolder);

                if (!Directory.Exists(targetFolder))
                    Directory.CreateDirectory(targetFolder);

                string fileName = Path.GetFileName(dlg.FileName);
                string targetPath = Path.Combine(targetFolder, fileName);

                int counter = 1;
                while (File.Exists(targetPath))
                {
                    string newFileName = Path.GetFileNameWithoutExtension(fileName) + $"_{counter}" + Path.GetExtension(fileName);
                    targetPath = Path.Combine(targetFolder, newFileName);
                    counter++;
                }

                File.Copy(dlg.FileName, targetPath);

                string relativePath = Path.Combine(relativeFolder, Path.GetFileName(targetPath));

                if (Images.Any(i => i.Path == relativePath))
                {
                    MessageBox.Show("Image already added.");
                    return;
                }

                var newImage = new Images
                {
                    Id = _imageRepository.NextId() + _localImageIdCounter++,
                    Path = relativePath
                };
                Images.Add(newImage);
            }
        }

        private void RemoveImage(Images? img)
        {
            if (!Images.Any()) { MessageBox.Show("You didn't add any images."); return; }
            if (img == null) { MessageBox.Show("You didn't chose an image to remove."); return; }
            Images.Remove(img);
            _localImageIdCounter = Math.Max(0, _localImageIdCounter - 1);
        }

        private void OpenImageViewer(Images img)
        {
            _currentImageIndex = Images.IndexOf(img);
            CurrentImage = img;
            IsImageViewerVisible = true;
        }

        private void ShowNextImage()
        {
            if (_currentImageIndex < Images.Count - 1)
            {
                _currentImageIndex++;
                CurrentImage = Images[_currentImageIndex];
            }
        }

        private void ShowPreviousImage()
        {
            if (_currentImageIndex > 0)
            {
                _currentImageIndex--;
                CurrentImage = Images[_currentImageIndex];
            }
        }

        private void RemoveCurrentImage()
        {
            if (CurrentImage == null) return;
            Images.Remove(CurrentImage);
            if (_currentImageIndex >= Images.Count)
            {
                IsImageViewerVisible = false;
                CurrentImage = null;
            }
            else
            {
                CurrentImage = Images[_currentImageIndex];
            }
        }
    }
}