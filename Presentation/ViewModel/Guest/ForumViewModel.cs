using System.Collections.ObjectModel;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using BookingApp.Services;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Guest
{
    public class ForumViewViewModel : ViewModelBase
    {
        private readonly IForumService _forumReadService; 
        private readonly IForumManagementService _forumWriteService; 

        private ForumDTO _forum; 

        #region Properties for Binding
        public string ForumTitle => _forum.Title;
        public ObservableCollection<CommentDTO> Comments => _forum.Comments;

        private string _newCommentText;
        public string NewCommentText
        {
            get => _newCommentText;
            set { _newCommentText = value; OnPropertyChanged(); }
        }

        public bool IsCommentingEnabled => !_forum.IsClosed;
        public bool IsCloseButtonVisible => _forum.CanBeClosed;
        #endregion

        #region Commands
        public ICommand PostCommentCommand { get; }
        public ICommand CloseForumCommand { get; }
        #endregion

        public ForumViewViewModel(ForumDTO forum)
        {
            _forumReadService = Injector.CreateInstance<IForumService>();
            _forumWriteService = Injector.CreateInstance<IForumManagementService>();

            _forum = _forumReadService.GetById(forum.Id);

            PostCommentCommand = new RelayCommand(PostComment, CanPostComment);
            CloseForumCommand = new RelayCommand(CloseForum, CanCloseForum);
        }

        #region Command Logic
        private bool CanPostComment(object obj)
        {
            return IsCommentingEnabled && !string.IsNullOrWhiteSpace(NewCommentText);
        }

        private void PostComment(object obj)
        {
            var newDomainComment = _forumWriteService.AddComment(_forum.Id, NewCommentText);

            _forum = _forumReadService.GetById(_forum.Id);
            OnPropertyChanged(nameof(Comments)); 

            NewCommentText = string.Empty;
        }

        private bool CanCloseForum(object obj)
        {
            return IsCloseButtonVisible;
        }

        private void CloseForum(object obj)
        {
            _forumWriteService.CloseForum(_forum.Id);

            _forum.IsClosed = true;
            _forum.CanBeClosed = false;
            OnPropertyChanged(nameof(IsCommentingEnabled));
            OnPropertyChanged(nameof(IsCloseButtonVisible));
        }
        #endregion
    }
}