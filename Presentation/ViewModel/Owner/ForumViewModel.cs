using BookingApp.Utilities;
using BookingApp.Services.DTO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace BookingApp.Presentation.ViewModel.Owner
{
    public class ForumViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ForumItemDTO> _allForums;
        private ObservableCollection<ForumItemDTO> _displayItems;
        private string _searchText;
        private bool _hasNoForums;

        public ObservableCollection<ForumItemDTO> DisplayItems
        {
            get => _displayItems;
            set
            {
                _displayItems = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterForums();
            }
        }

        public bool HasNoForums
        {
            get => _hasNoForums;
            set
            {
                _hasNoForums = value;
                OnPropertyChanged();
            }
        }

       
       

        public ForumViewModel()
        {
            InitializeCommands();
            LoadMockData();
            FilterForums();
        }

        public ICommand RefreshCommand { get; private set; }
        public ICommand ShowCommentsCommand { get; private set; }

        private void InitializeCommands()
        {
            RefreshCommand = new RelayCommand(ExecuteRefresh);
            ShowCommentsCommand = new RelayCommand(ExecuteShowComments);
        }

        private void LoadMockData()
        {
            _allForums = new ObservableCollection<ForumItemDTO>
            {
                new ForumItemDTO
                {
                    Title = "Best restaurants in Belgrade downtown",
                    Location = "Belgrade, Serbia",
                    CreatedBy = "MarkoTraveler",
                    CreatedDate = DateTime.Now.AddDays(-15),
                    OwnerCommentsCount = 8,
                    GuestCommentsCount = 23
                },
                new ForumItemDTO
                {
                    Title = "Parking situation near Kalemegdan",
                    Location = "Belgrade, Serbia",
                    CreatedBy = "AnaExplorer",
                    CreatedDate = DateTime.Now.AddDays(-8),
                    OwnerCommentsCount = 12,
                    GuestCommentsCount = 31
                },
                new ForumItemDTO
                {
                    Title = "Public transport to Nikola Tesla Airport",
                    Location = "Belgrade, Serbia",
                    CreatedBy = "StefanBusiness",
                    CreatedDate = DateTime.Now.AddDays(-3),
                    OwnerCommentsCount = 6,
                    GuestCommentsCount = 15
                },
                new ForumItemDTO
                {
                    Title = "Nightlife recommendations in Novi Sad",
                    Location = "Novi Sad, Serbia",
                    CreatedBy = "MilaParty",
                    CreatedDate = DateTime.Now.AddDays(-12),
                    OwnerCommentsCount = 4,
                    GuestCommentsCount = 18
                },
                new ForumItemDTO
                {
                    Title = "Shopping centers and local markets",
                    Location = "Belgrade, Serbia",
                    CreatedBy = "JovanaShopaholic",
                    CreatedDate = DateTime.Now.AddDays(-20),
                    OwnerCommentsCount = 15,
                    GuestCommentsCount = 42
                },
                new ForumItemDTO
                {
                    Title = "Day trips from Nis - hidden gems",
                    Location = "Nis, Serbia",
                    CreatedBy = "PetarAdventure",
                    CreatedDate = DateTime.Now.AddDays(-5),
                    OwnerCommentsCount = 3,
                    GuestCommentsCount = 12
                }
            };
        }

        private void FilterForums()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                DisplayItems = new ObservableCollection<ForumItemDTO>(_allForums);
            }
            else
            {
                var filteredItems = _allForums.Where(forum =>
                    forum.Title.ToLower().Contains(SearchText.ToLower()) ||
                    forum.Location.ToLower().Contains(SearchText.ToLower())
                ).ToList();

                DisplayItems = new ObservableCollection<ForumItemDTO>(filteredItems);
            }

            HasNoForums = !DisplayItems.Any();
        }


        private void ExecuteRefresh(object parameter)
        {
            LoadMockData();
            FilterForums();
            MessageBox.Show("Forums refreshed successfully!", "Refresh", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        public event Action<ForumItemDTO> OnShowCommentsRequested;
        private void ExecuteShowComments(object parameter)
        {
            if (parameter is ForumItemDTO forum)
            {
                OnShowCommentsRequested?.Invoke(forum);
            }
        }
        

        

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}