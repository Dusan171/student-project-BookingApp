using BookingApp.Domain;
using BookingApp.Services.DTO;
using BookingApp.Domain.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;
using System.Windows;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Owner
{
    public class RegisterAccommodationViewModel : INotifyPropertyChanged
    {
        private readonly IAccommodationService _accommodationService;

        public AccommodationDTO Accommodation { get; set; }
        public ObservableCollection<AccommodationImageDTO> ImagePaths { get; set; }

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(); }
        }
    

        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddImageCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public RegisterAccommodationViewModel(IAccommodationService accommodationService)
        {
            _accommodationService = accommodationService;
            Accommodation = new AccommodationDTO();
            ImagePaths = new ObservableCollection<AccommodationImageDTO>();

            ConfirmCommand = new RelayCommand(Confirm);
            CancelCommand = new RelayCommand(Cancel);
            AddImageCommand = new RelayCommand(AddImage);
        }

        private void AddImage()
        {
            if (!string.IsNullOrWhiteSpace(ImagePath))
            {
                ImagePaths.Add(new AccommodationImageDTO { Path = ImagePath.Trim() });
                ImagePath = string.Empty;
            }
            else
            {
                ShowMessage("Please enter a valid image path.");
            }
        }

        private void Confirm()
        {
            Accommodation.ImagePaths = ImagePaths.ToList();

            var success = _accommodationService.RegisterAccommodation(Accommodation);
            if (success)
            {
                ShowMessage("Accommodation saved successfully!");
                Cancel();
            }
            else
                ShowMessage("Invalid information!");
        }
        private void Cancel()
        {
            Accommodation = new AccommodationDTO();
            ImagePaths.Clear();
            OnPropertyChanged(nameof(Accommodation));
        }

        private void ShowMessage(string text)
        {
            MessageBox.Show(text);
        }
    }
}
