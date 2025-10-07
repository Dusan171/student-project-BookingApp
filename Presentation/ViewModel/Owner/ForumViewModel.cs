using BookingApp.Utilities;
using BookingApp.Services.DTO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;

namespace BookingApp.Presentation.ViewModel.Owner
{
    public class ForumViewModel : INotifyPropertyChanged
    {
        private readonly IForumService _forumService;
        private ObservableCollection<ForumDTO> _allForums;
        private ObservableCollection<ForumDTO> _displayItems;
        private string _searchText;
        private bool _hasNoForums;

        public ObservableCollection<ForumDTO> DisplayItems
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

        public ICommand RefreshCommand { get; private set; }
        public ICommand ShowCommentsCommand { get; private set; }

        public ForumViewModel(IForumService forumService)
        {
            _forumService = forumService;
            InitializeCommands();
            LoadForums();
            FilterForums();
        }

        private void InitializeCommands()
        {
            RefreshCommand = new RelayCommand(ExecuteRefresh);
            ShowCommentsCommand = new RelayCommand(ExecuteShowComments);
        }

        private void LoadForums()
        {
            var forums = _forumService.GetAll();
            _allForums = new ObservableCollection<ForumDTO>(forums);
        }

        private void FilterForums()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                DisplayItems = new ObservableCollection<ForumDTO>(_allForums);
            }
            else
            {
                var filteredItems = _allForums.Where(forum =>
                    forum.Title.ToLower().Contains(SearchText.ToLower()) ||
                    forum.LocationName.ToLower().Contains(SearchText.ToLower())
                ).ToList();

                DisplayItems = new ObservableCollection<ForumDTO>(filteredItems);
            }

            HasNoForums = !DisplayItems.Any();
        }
        public void RefreshForums()
        {
            LoadForums();
            FilterForums();
        }
        private void ExecuteRefresh(object parameter)
        {
            LoadForums();
            FilterForums();
            MessageBox.Show("Forums refreshed successfully!", "Refresh", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public event Action<int> OnShowCommentsRequested;

        private void ExecuteShowComments(object parameter)
        {
            if (parameter is ForumDTO forum)
            {
                OnShowCommentsRequested?.Invoke(forum.Id);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}