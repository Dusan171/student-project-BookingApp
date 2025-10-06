using BookingApp.Domain.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookingApp.Services.DTO
{
    public class ForumDTO : INotifyPropertyChanged
    {
        private int _id;
        private string _title;
        private bool _isClosed;
        private bool _isVeryUseful;
        private bool _canBeClosed;
        private string _locationName;
        private string _creatorName;
        private int _commentCount;
        public DateTime CreatedDate { get; set; }
        public int OwnerCommentsCount { get; set; }
        public int GuestCommentsCount { get; set; }
        public ObservableCollection<CommentDTO> Comments { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int Id
        {
            get => _id;
            set { _id = value; } 
        }
        public string Title
        {
            get => _title;
            set { if (_title != value) { _title = value; OnPropertyChanged(); } }
        }
        public bool IsClosed
        {
            get => _isClosed;
            set { if (_isClosed != value) { _isClosed = value; OnPropertyChanged(); } }
        }
        public bool IsVeryUseful
        {
            get => _isVeryUseful;
            set { if (_isVeryUseful != value) { _isVeryUseful = value; OnPropertyChanged(); } }
        }
        public bool CanBeClosed
        {
            get => _canBeClosed;
            set { if (_canBeClosed != value) { _canBeClosed = value; OnPropertyChanged(); } }
        }
        public string LocationName
        {
            get => _locationName;
            set { if (_locationName != value) { _locationName = value; OnPropertyChanged(); } }
        }
        public string CreatorName
        {
            get => _creatorName;
            set { if (_creatorName != value) { _creatorName = value; OnPropertyChanged(); } }
        }
        public int CommentCount
        {
            get => _commentCount;
            set { if (_commentCount != value) { _commentCount = value; OnPropertyChanged(); } } 
        }

        public ForumDTO()
        {
            Comments = new ObservableCollection<CommentDTO>();
        }

        public ForumDTO(Forum forum)
        {
            Id = forum.Id;
            Title = forum.Title;
            IsClosed = forum.IsClosed;
            CreatedDate = forum.CreationDate; 
            LocationName = forum.Location != null ? $"{forum.Location.City}, {forum.Location.Country}" : "Unknown Location";
            CreatorName = forum.Creator != null ? forum.Creator.Username : "Unknown User";
            Comments = new ObservableCollection<CommentDTO>();
        }
    }
}