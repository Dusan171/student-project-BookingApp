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
    public class CreateTourControlViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged(string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            RaiseCanExecuteChanged(); 
        }

        private void RaiseCanExecuteChanged()
        {
            CreateTourCommand?.RaiseCanExecuteChanged();
        }

        public event EventHandler? RequestClose;
        public event EventHandler? RequestCancel;

        private readonly TourRepository _tourRepository = new();
        private readonly KeyPointRepository _keyPointRepository = new();
        private readonly ImageRepository _imageRepository = new();
        private readonly StartTourTimeRepository _startTimeRepository = new();
        private readonly LocationRepository _locationRepository = new();

        private int _localKeyPointIdCounter = 0;
        private int _localStartTimeIdCounter = 0;
        private int _localImageIdCounter = 0;

        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(nameof(Name)); } }

        private string _city;
        public string City { get => _city; set { _city = value; OnPropertyChanged(nameof(City)); } }

        private string _country;
        public string Country { get => _country; set { _country = value; OnPropertyChanged(nameof(Country)); } }

        private string _description;
        public string Description { get => _description; set { _description = value; OnPropertyChanged(nameof(Description)); } }

        private string _language;
        public string Language { get => _language; set { _language = value; OnPropertyChanged(nameof(Language)); } }

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

        private string _duration;
        public string Duration
        {
            get => _duration;
            set
            {
                if (int.TryParse(value, out int parsed))
                {
                    if (parsed < 1)
                    {
                        MessageBox.Show("Duration must be greater than 0.");
                        return;
                    }
                    _duration = parsed.ToString();
                }
                else
                {
                    _duration = value;
                }
                OnPropertyChanged(nameof(Duration));
            }
        }

        public string KeyPointInput { get; set; } = string.Empty;

        private DateTime? _selectedStartDate;
        public DateTime? SelectedStartDate
        {
            get => _selectedStartDate;
            set { _selectedStartDate = value; OnPropertyChanged(nameof(SelectedStartDate)); }
        }

        public ObservableCollection<int> Hours { get; } = new ObservableCollection<int>(Enumerable.Range(0, 24));
        public ObservableCollection<int> Minutes { get; } = new ObservableCollection<int>(Enumerable.Range(0, 60));

        private int _selectedHour;
        public int SelectedHour { get => _selectedHour; set { _selectedHour = value; OnPropertyChanged(nameof(SelectedHour)); } }

        private int _selectedMinute;
        public int SelectedMinute { get => _selectedMinute; set { _selectedMinute = value; OnPropertyChanged(nameof(SelectedMinute)); } }

        private bool _isEditableLocation = true;
        private bool _isEditableLanguage = true;
        public bool IsEditableLocation { get => _isEditableLocation; set { _isEditableLocation = value; OnPropertyChanged(nameof(IsEditableLocation)); } }
        public bool IsEditableLanguage { get => _isEditableLanguage; set { _isEditableLanguage = value; OnPropertyChanged(nameof(IsEditableLanguage)); } }

        public ObservableCollection<KeyPoint> KeyPoints { get; } = new();
        public ObservableCollection<StartTourTime> StartTimes { get; } = new();
        public ObservableCollection<Images> Images { get; } = new();

        public KeyPoint? SelectedKeyPoint { get; set; }
        public StartTourTime? SelectedStartTime { get; set; }
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
        public RelayCommand AddStartTimeCommand { get; }
        public RelayCommand RemoveStartTimeCommand { get; }
        public RelayCommand CreateTourCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand<Images> OpenImageViewerCommand { get; }
        public RelayCommand NextImageCommand { get; }
        public RelayCommand PreviousImageCommand { get; }
        public RelayCommand RemoveCurrentImageCommand { get; }
        public RelayCommand CloseImageViewerCommand { get; }
        public Tour suggested { get; set; }

        public CreateTourControlViewModel(Tour suggestion = null)
        {
            suggested = suggestion ?? new Tour();
            FillSuggestion(suggested);

            AddKeyPointCommand = new RelayCommand(_ => AddKeyPoint());
            RemoveKeyPointCommand = new RelayCommand(p => RemoveKeyPoint(p as KeyPoint));
            BrowseImageCommand = new RelayCommand(_ => BrowseImage());
            RemoveImageCommand = new RelayCommand(_ => RemoveImage(SelectedImage));
            AddStartTimeCommand = new RelayCommand(_ => AddStartTime());
            RemoveStartTimeCommand = new RelayCommand(_ => RemoveStartTime(SelectedStartTime));

            CreateTourCommand = new RelayCommand(_ => CreateTour(), _ => CanCreateTour());

            CancelCommand = new RelayCommand(_ =>
            {
                var result = MessageBox.Show("Are you sure you want to cancel?", "Cancelation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes) RequestCancel?.Invoke(this, EventArgs.Empty);
            });

            OpenImageViewerCommand = new RelayCommand<Images>(img => OpenImageViewer(img));
            NextImageCommand = new RelayCommand(_ => ShowNextImage());
            PreviousImageCommand = new RelayCommand(_ => ShowPreviousImage());
            RemoveCurrentImageCommand = new RelayCommand(_ => RemoveCurrentImage());
            CloseImageViewerCommand = new RelayCommand(_ => CloseImageViewer());
        }

        private void CloseImageViewer()
        {
            IsImageViewerVisible = false;
            CurrentImage = null;
        }

        private void FillSuggestion(Tour suggested)
        {
            if (suggested == null) return;
            if (suggested.Location != null)
            {
                City = suggested.Location.City;
                Country = suggested.Location.Country;
                IsEditableLocation = false;
            }
            else if (suggested.Language != null)
            {
                Language = suggested.Language;
                IsEditableLanguage = false;
            }
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

        private void AddStartTime()
        {
            if (SelectedStartDate == null)
            {
                MessageBox.Show("Please select a date.");
                return;
            }
            DateTime combined = SelectedStartDate.Value.Date + new TimeSpan(SelectedHour, SelectedMinute, 0);
            if (StartTimes.Any(t => t.Time == combined))
            {
                MessageBox.Show("This start time already exists.");
                return;
            }
            var newStartTime = new StartTourTime { Id = _startTimeRepository.NextId() + _localStartTimeIdCounter++, Time = combined };
            StartTimes.Add(newStartTime);
            SelectedStartDate = null;
            SelectedHour = SelectedMinute = 0;
        }

        private void RemoveStartTime(StartTourTime? st)
        {
            if (!StartTimes.Any()) { MessageBox.Show("You didn't add any start times."); return; }
            if (st == null) { MessageBox.Show("You didn't chose a start time to remove."); return; }
            StartTimes.Remove(st);
            _localStartTimeIdCounter = Math.Max(0, _localStartTimeIdCounter - 1);
        }

        private bool CanCreateTour()
        {
            return !string.IsNullOrWhiteSpace(Name)
                && !string.IsNullOrWhiteSpace(City)
                && !string.IsNullOrWhiteSpace(Country)
                && !string.IsNullOrWhiteSpace(Description)
                && !string.IsNullOrWhiteSpace(Language)
                && int.TryParse(MaxTourists, out int max) && max > 0
                && int.TryParse(Duration, out int dur) && dur > 0
                && KeyPoints.Count > 1
                && StartTimes.Count > 0;
        }

        private void CreateTour()
        {
            var newTour = new Tour
            {
                GuideId = Session.CurrentUser.Id,
                Name = Name.Trim(),
                Location = new Location { Id = _locationRepository.NextId(), City = City.Trim(), Country = Country.Trim() },
                Description = Description?.Trim(),
                Language = Language?.Trim(),
                MaxTourists = int.Parse(MaxTourists),
                DurationHours = int.Parse(Duration),
                StartTimes = StartTimes.ToList(),
                KeyPoints = KeyPoints.ToList(),
                Images = Images.ToList()
            };

            _tourRepository.Save(newTour);
            _locationRepository.Save(newTour.Location);
            foreach (var st in newTour.StartTimes)
            {
                st.TourId = newTour.Id;
                _startTimeRepository.Save(st);
            }
            foreach (var kp in newTour.KeyPoints) _keyPointRepository.Save(kp);
            foreach (var img in newTour.Images) _imageRepository.Save(img);

            MessageBox.Show("Tour created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            RequestClose?.Invoke(this, EventArgs.Empty);
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
