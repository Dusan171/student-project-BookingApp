using BookingApp.Domain.Model;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookingApp.Services.DTO
{
    public class CommentDTO : INotifyPropertyChanged
    {
        private int _id;
        private DateTime _creationTime;
        private string _text;
        private int _userId;

        private string _authorName;
        private bool _isFromVisitor;
        private bool _isFromOwner;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public int Id
        {
            get => _id;
            set => _id = value;
        }
        public DateTime CreationTime
        {
            get => _creationTime;
            set
            {
                if (_creationTime != value)
                {
                    _creationTime = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged();
                }
            }
        }
        public int UserId
        {
            get => _userId;
            set
            {
                if (_userId != value)
                {
                    _userId = value;
                    OnPropertyChanged();
                }
            }
        }
        public string AuthorName 
        {
            get => _authorName;
            set { if (_authorName != value) { _authorName = value;OnPropertyChanged(); } }
        }
        public bool IsFromOwner
        {
            get => _isFromOwner;
            set { if (_isFromOwner != value) { _isFromOwner = value; OnPropertyChanged(); } }
        }
        public bool IsFromVisitor
        {
            get => _isFromVisitor;
            set { if (_isFromVisitor != value) { _isFromVisitor = value;OnPropertyChanged(); } }
        }
        public CommentDTO() { }
        public CommentDTO(Comment c)
        {
            _id = c.Id;
            _creationTime = c.CreationTime;
            _text = c.Text;
            _userId = c.User.Id;
            _authorName = c.User.Username;
        }
        public Comment ToComment()
        {
            return new Comment
            {
                Id = this.Id,
                CreationTime = this.CreationTime,
                Text = this.Text,
                User = new User { Id = this.UserId }
            };
        }
    }
}

