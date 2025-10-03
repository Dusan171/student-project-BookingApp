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
    public class ForumCommentsViewModel : INotifyPropertyChanged
    {
        private readonly IForumService _forumService;
        private readonly IOwnerForumService _ownerForumService;
        private readonly ICommentReportService _reportService; 

        private ForumDTO _selectedForum;
        private string _newCommentText;
        private bool _canAddComment;
        private bool _hasRestrictionMessage;
        private string _restrictionMessage;
        private string _userStatus;

        public ForumDTO SelectedForum
        {
            get => _selectedForum;
            set
            {
                _selectedForum = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasNoComments));
            }
        }

        public bool HasNoComments => SelectedForum?.Comments == null || !SelectedForum.Comments.Any();

        public string NewCommentText
        {
            get => _newCommentText;
            set
            {
                _newCommentText = value;
                OnPropertyChanged();
            }
        }

        public bool CanAddComment
        {
            get => _canAddComment;
            set
            {
                _canAddComment = value;
                OnPropertyChanged();
            }
        }

        public bool HasRestrictionMessage
        {
            get => _hasRestrictionMessage;
            set
            {
                _hasRestrictionMessage = value;
                OnPropertyChanged();
            }
        }

        public string RestrictionMessage
        {
            get => _restrictionMessage;
            set
            {
                _restrictionMessage = value;
                OnPropertyChanged();
            }
        }

        public string UserStatus
        {
            get => _userStatus;
            set
            {
                _userStatus = value;
                OnPropertyChanged();
            }
        }

        public ICommand BackToForumsCommand { get; private set; }
        public ICommand RefreshCommentsCommand { get; private set; }
        public ICommand PostCommentCommand { get; private set; }
        public ICommand ReportCommentCommand { get; private set; }

        public ForumCommentsViewModel(
        IForumService forumService,
        IOwnerForumService ownerForumService,
        ICommentReportService reportService, 
        int forumId)
        {
            _forumService = forumService;
            _ownerForumService = ownerForumService;
            _reportService = reportService; 

            InitializeCommands();
            LoadForum(forumId);
            DetermineUserPermissions();
        }

        private void InitializeCommands()
        {
            BackToForumsCommand = new RelayCommand(ExecuteBackToForums);
            RefreshCommentsCommand = new RelayCommand(ExecuteRefreshComments);
            PostCommentCommand = new RelayCommand(ExecutePostComment, CanExecutePostComment);
            ReportCommentCommand = new RelayCommand(ExecuteReportComment);
        }

        private void LoadForum(int forumId)
        {
            SelectedForum = _forumService.GetById(forumId);

            if (SelectedForum == null)
            {
                MessageBox.Show("Forum not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DetermineUserPermissions()
        {
            if (SelectedForum == null) return;

            if (SelectedForum.IsClosed)
            {
                CanAddComment = false;
                UserStatus = "This forum is closed";
                HasRestrictionMessage = true;
                RestrictionMessage = "This forum has been closed and no new comments can be added.";
                return;
            }

            CanAddComment = _ownerForumService.CanOwnerComment(SelectedForum.Id, Session.CurrentUser.Id);

            if (CanAddComment)
            {
                UserStatus = "You can comment as a property owner in this location";
                HasRestrictionMessage = false;
            }
            else
            {
                UserStatus = "You need accommodation in this location to comment";
                HasRestrictionMessage = true;
                RestrictionMessage = "Only property owners with accommodations in this location can leave comments on this forum.";
            }
        }

        private void ExecuteRefreshComments(object parameter)
        {
            if (SelectedForum == null) return;

            LoadForum(SelectedForum.Id);
            MessageBox.Show("Comments refreshed successfully!", "Refresh", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool CanExecutePostComment(object parameter)
        {
            return CanAddComment && !string.IsNullOrWhiteSpace(NewCommentText);
        }

        private void ExecutePostComment(object parameter)
        {
            if (SelectedForum == null) return;

            if (!CanAddComment)
            {
                MessageBox.Show("You cannot comment on this forum. You need to have accommodation in this location.",
                    "Comment Restricted", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(NewCommentText))
            {
                MessageBox.Show("Please enter your comment.", "Empty Comment", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _ownerForumService.AddOwnerComment(SelectedForum.Id, Session.CurrentUser.Id, NewCommentText);

                NewCommentText = "";
                LoadForum(SelectedForum.Id);

                MessageBox.Show("Your comment has been posted successfully!", "Comment Posted",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to post comment: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event Action OnBackToForumsRequested;

        private void ExecuteBackToForums(object parameter)
        {
            OnBackToForumsRequested?.Invoke();
        }

        
        private void ExecuteReportComment(object parameter)
        {
            if (parameter is CommentDTO comment)
            {
                // Vlasnik ne može da prijavi komentare drugih vlasnika
                if (comment.IsOwnerComment)
                {
                    MessageBox.Show("You cannot report comments from property owners.",
                        "Report Denied", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                if (comment.HasVerifiedStay)
                {
                    MessageBox.Show(
                        "This guest has a verified stay at this location and cannot be reported.",
                        "Cannot Report Verified Guest",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }
                // Proveri da li je već prijavio
                if (_reportService.HasUserReported(comment.Id, Session.CurrentUser.Id))
                {
                    MessageBox.Show("You have already reported this comment.",
                        "Already Reported", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var result = MessageBox.Show(
                    $"Are you sure you want to report this comment?\n\n\"{comment.CommentText}\"",
                    "Report Comment",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool success = _reportService.ReportComment(comment.Id, Session.CurrentUser.Id);

                    if (success)
                    {
                        // Refresh forum da dobiješ ažurirane reports count
                        LoadForum(SelectedForum.Id);

                        MessageBox.Show(
                            "Comment has been reported. Thank you for helping maintain forum quality.",
                            "Report Submitted",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(
                            "Failed to report comment. Please try again.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}