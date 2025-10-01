using System;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using BookingApp.Services;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Guest
{
    public class CreateForumViewModel : ViewModelBase
    {
        private readonly IForumManagementService _forumManagementService;

        #region Properties for Binding
        private string _forumTitle;
        public string ForumTitle
        {
            get => _forumTitle;
            set
            {
                if (_forumTitle != value)
                {
                    _forumTitle = value;
                    OnPropertyChanged();
                    ((RelayCommand)CreateCommand).RaiseCanExecuteChanged();
                }
            }
        }
        private string _location;
        public string Location
        {
            get => _location;
            set
            {
                if (_location != value)
                {
                    _location = value;
                    OnPropertyChanged();
                    ((RelayCommand)CreateCommand).RaiseCanExecuteChanged();
                }
            }
        }
        private string _firstComment;
        public string FirstComment
        {
            get => _firstComment;
            set
            {
                if (_firstComment != value)
                {
                    _firstComment = value;
                    OnPropertyChanged();
                    ((RelayCommand)CreateCommand).RaiseCanExecuteChanged();
                }
            }
        }
        #endregion

        public Action CloseAction { get; set; }
        public ICommand CreateCommand { get; }
        public CreateForumViewModel()
        {
            _forumManagementService = Injector.CreateInstance<IForumManagementService>();

            CreateCommand = new RelayCommand(Create, CanCreate);
        }
        private bool CanCreate(object obj)
        {
            return !string.IsNullOrWhiteSpace(ForumTitle) &&
                   !string.IsNullOrWhiteSpace(Location) &&
                   !string.IsNullOrWhiteSpace(FirstComment);
        }
        private void Create(object obj)
        {
            try
            {
                _forumManagementService.Create(ForumTitle, Location, FirstComment);

                MessageBox.Show("Forum created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while creating the forum: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}