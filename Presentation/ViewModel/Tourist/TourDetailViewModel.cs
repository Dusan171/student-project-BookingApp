using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BookingApp.Domain.Model;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Tourist
{
    public class TourDetailViewModel : ViewModelBase
    {
        private TourDTO _selectedTour;
        private bool _showReserveButton = true;

        public TourDTO SelectedTour
        {
            get => _selectedTour;
            set
            {
                SetProperty(ref _selectedTour, value);
                LoadMockData();
            }
        }


        public bool ShowReserveButton
        {
            get => _showReserveButton;
            set => SetProperty(ref _showReserveButton, value);
        }


        public ObservableCollection<KeyPointDTO> KeyPoints { get; set; } = new();
        public ObservableCollection<Images> Images { get; set; } = new();

        public bool HasKeyPoints => true;
        public bool HasImages => true;

       

        public ICommand ReserveTourCommand { get; private set; }
        public ICommand BackCommand { get; private set; }

        public event Action<TourDTO> TourReserveRequested;
        public event Action BackRequested;

        public TourDetailViewModel()
        {
            ReserveTourCommand = new RelayCommand<TourDTO>(tour => TourReserveRequested?.Invoke(tour));
            BackCommand = new RelayCommand(() => BackRequested?.Invoke());
        }

        private void LoadMockData()
        {
            KeyPoints.Clear();
            Images.Clear();

            
            KeyPoints.Add(new KeyPointDTO { Id = 1, Name = "Početna lokacija" });
            KeyPoints.Add(new KeyPointDTO { Id = 2, Name = "Istorijski spomenik" });
            KeyPoints.Add(new KeyPointDTO { Id = 3, Name = "Centralni muzej" });
            KeyPoints.Add(new KeyPointDTO { Id = 4, Name = "Panoramska tačka" });
            KeyPoints.Add(new KeyPointDTO { Id = 5, Name = "Završna lokacija" });

           
            Images.Add(new Images { Id = 1, Path = "/Resources/Images/tour_main.jpg" });
            Images.Add(new Images { Id = 2, Path = "/Resources/Images/slika1.jpg" });
            Images.Add(new Images { Id = 3, Path = "/Resources/Images/slika2.jpg" });
            Images.Add(new Images { Id = 4, Path = "/Resources/Images/slika3.jpg" });
            Images.Add(new Images { Id = 5, Path = "/Resources/Images/slika4.jpg" });
            Images.Add(new Images { Id = 6, Path = "/Resources/Images/slika5.jpg" });
        }

        public void SetTour(TourDTO tour, bool isFromNotification = false)
        {
            SelectedTour = tour;
  
        }
    }
}