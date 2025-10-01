using BookingApp.Utilities;
using BookingApp.Services.DTO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;

namespace BookingApp.Presentation.ViewModel.Owner
{
    public class ForumCommentsViewModel : INotifyPropertyChanged
    {
        private readonly ForumItemDTO _selectedForum;
        private ObservableCollection<ForumCommentDTO> _comments;
        private string _newCommentText;
        private bool _canAddComment;
        private bool _hasRestrictionMessage;
        private string _restrictionMessage;
        private string _userStatus;
        private bool _hasNoComments;

        // Forum Info Properties
        public string ForumTitle => _selectedForum?.Title ?? "";
        public string ForumLocation => _selectedForum?.Location ?? "";
        public string ForumDescription { get; set; }
        public string CreatedBy => _selectedForum?.CreatedBy ?? "";
        public DateTime CreatedDate => _selectedForum?.CreatedDate ?? DateTime.Now;
        public bool IsVeryUseful => _selectedForum?.IsVeryUseful ?? false;
        public int OwnerCommentsCount => _selectedForum?.OwnerCommentsCount ?? 0;
        public int GuestCommentsCount => _selectedForum?.GuestCommentsCount ?? 0;
        public int TotalCommentsCount => Comments?.Count ?? 0;

        public ObservableCollection<ForumCommentDTO> Comments
        {
            get => _comments;
            set
            {
                _comments = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalCommentsCount));
            }
        }

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

        public bool HasNoComments
        {
            get => _hasNoComments;
            set
            {
                _hasNoComments = value;
                OnPropertyChanged();
            }
        }

        public ICommand BackToForumsCommand { get; private set; }
        public ICommand RefreshCommentsCommand { get; private set; }
        public ICommand PostCommentCommand { get; private set; }
        public ICommand ReportCommentCommand { get; private set; }

        public ForumCommentsViewModel(ForumItemDTO selectedForum)
        {
            _selectedForum = selectedForum;
            InitializeCommands();
            LoadMockData();
            DetermineUserPermissions();
        }

        private void InitializeCommands()
        {
            BackToForumsCommand = new RelayCommand(ExecuteBackToForums);
            RefreshCommentsCommand = new RelayCommand(ExecuteRefreshComments);
            PostCommentCommand = new RelayCommand(ExecutePostComment, CanExecutePostComment);
            ReportCommentCommand = new RelayCommand(ExecuteReportComment);
        }

        private void LoadMockData()
        {
            // Mock forum description
            ForumDescription = GetMockForumDescription();

            // Mock comments based on forum title
            Comments = new ObservableCollection<ForumCommentDTO>(GetMockComments());
            HasNoComments = !Comments.Any();
        }

        private string GetMockForumDescription()
        {
            return ForumTitle switch
            {
                "Best restaurants in Belgrade downtown" => "Looking for authentic local dining experiences in the heart of Belgrade. Share your favorite spots for traditional Serbian cuisine, trendy cafes, and must-visit restaurants!",
                "Parking situation near Kalemegdan" => "Visiting Kalemegdan fortress and surrounding areas. Need advice on parking options, fees, and best places to leave your car safely.",
                "Public transport to Nikola Tesla Airport" => "What's the most convenient and cost-effective way to get to Belgrade airport using public transportation? Bus routes, schedules, and tips welcome!",
                "Nightlife recommendations in Novi Sad" => "Planning a night out in Novi Sad! Looking for the best bars, clubs, and entertainment venues in the city.",
                "Shopping centers and local markets" => "Where to find the best shopping in Belgrade - both modern malls and traditional markets. Looking for local products and good deals.",
                "Day trips from Nis - hidden gems" => "Exploring the area around Nis. What are some interesting places to visit within a day trip? Historical sites, nature spots, local attractions welcome!",
                _ => "Discussion about this location and experiences shared by travelers and locals."
            };
        }

        private ForumCommentDTO[] GetMockComments()
        {
            return ForumTitle switch
            {
                "Best restaurants in Belgrade downtown" => new[]
                {
                    new ForumCommentDTO
                    {
                        CommentId = 1,
                        AuthorName = "Milan Petrovic",
                        IsOwnerComment = true,
                        IsGuestComment = false,
                        HasVerifiedStay = true,
                        CommentText = "As a local property owner, I highly recommend Skadarlija area for traditional restaurants. Tri Sesira and Dva Jelena are excellent choices. My guests always love these places!",
                        CreatedDate = DateTime.Now.AddDays(-10),
                        ReportsCount = 0,
                        HasReports = false,
                        CanReport = false,
                        StayDatesText = "Local business owner"
                    },
                    new ForumCommentDTO
                    {
                        CommentId = 2,
                        AuthorName = "Sarah Johnson",
                        IsOwnerComment = false,
                        IsGuestComment = true,
                        HasVerifiedStay = true,
                        CommentText = "Stayed here last month and tried Lorenzo & Kakalamba - amazing fusion food! Also loved the floating river restaurants. Book in advance!",
                        CreatedDate = DateTime.Now.AddDays(-8),
                        ReportsCount = 0,
                        HasReports = false,
                        CanReport = true,
                        StayDatesText = "15-22 Nov 2024"
                    },
                    new ForumCommentDTO
                    {
                        CommentId = 3,
                        AuthorName = "RandomUser123",
                        IsOwnerComment = false,
                        IsGuestComment = true,
                        HasVerifiedStay = false,
                        CommentText = "Belgrade restaurants are overpriced and food is terrible. Don't waste your money there.",
                        CreatedDate = DateTime.Now.AddDays(-3),
                        ReportsCount = 3,
                        HasReports = true,
                        CanReport = true,
                        StayDatesText = ""
                    },
                    new ForumCommentDTO
                    {
                        CommentId = 4,
                        AuthorName = "Ana Nikolic",
                        IsOwnerComment = true,
                        IsGuestComment = false,
                        HasVerifiedStay = true,
                        CommentText = "I run a guesthouse nearby and my guests love Kafana Question Mark for the atmosphere. Also check out Manufaktura for great coffee!",
                        CreatedDate = DateTime.Now.AddDays(-1),
                        ReportsCount = 0,
                        HasReports = false,
                        CanReport = false,
                        StayDatesText = "Property owner since 2020"
                    }
                },
                "Parking situation near Kalemegdan" => new[]
                {
                    new ForumCommentDTO
                    {
                        CommentId = 5,
                        AuthorName = "Marko Jovanovic",
                        IsOwnerComment = true,
                        IsGuestComment = false,
                        HasVerifiedStay = true,
                        CommentText = "I always tell my guests to use the underground parking at Rajiceva Shopping Center - it's safe and just 10 minutes walk to Kalemegdan. Around 200 RSD per day.",
                        CreatedDate = DateTime.Now.AddDays(-5),
                        ReportsCount = 0,
                        HasReports = false,
                        CanReport = false,
                        StayDatesText = "Local host"
                    },
                    new ForumCommentDTO
                    {
                        CommentId = 6,
                        AuthorName = "Tourist2024",
                        IsOwnerComment = false,
                        IsGuestComment = true,
                        HasVerifiedStay = false,
                        CommentText = "Just park anywhere for free, nobody checks. I never paid for parking in Belgrade.",
                        CreatedDate = DateTime.Now.AddDays(-2),
                        ReportsCount = 2,
                        HasReports = true,
                        CanReport = true,
                        StayDatesText = ""
                    }
                },
                _ => new[]
                {
                    new ForumCommentDTO
                    {
                        CommentId = 7,
                        AuthorName = "TestUser",
                        IsOwnerComment = false,
                        IsGuestComment = true,
                        HasVerifiedStay = true,
                        CommentText = "This is a sample comment for this forum topic.",
                        CreatedDate = DateTime.Now.AddDays(-1),
                        ReportsCount = 0,
                        HasReports = false,
                        CanReport = true,
                        StayDatesText = "01-05 Dec 2024"
                    }
                }
            };
        }

        private void DetermineUserPermissions()
        {
            bool hasAccommodationInLocation = ForumLocation.Contains("Belgrade");

            if (hasAccommodationInLocation)
            {
                CanAddComment = true;
                UserStatus = "You can comment as a property owner in this location";
                HasRestrictionMessage = false;
            }
            else
            {
                CanAddComment = false;
                UserStatus = "You need accommodation in this location to comment";
                HasRestrictionMessage = true;
                RestrictionMessage = "Only property owners with accommodations in this location can leave comments on this forum.";
            }
        }


        private void ExecuteRefreshComments(object parameter)
        {
            LoadMockData();
            MessageBox.Show("Comments refreshed successfully!", "Refresh", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool CanExecutePostComment(object parameter)
        {
            return CanAddComment && !string.IsNullOrWhiteSpace(NewCommentText);
        }

        private void ExecutePostComment(object parameter)
        {
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

            // Add new comment to the list
            var newComment = new ForumCommentDTO
            {
                CommentId = Comments.Count + 100,
                AuthorName = "Current User", 
                IsOwnerComment = true,
                IsGuestComment = false,
                HasVerifiedStay = true,
                CommentText = NewCommentText,
                CreatedDate = DateTime.Now,
                ReportsCount = 0,
                HasReports = false,
                CanReport = false,
                StayDatesText = "Property owner"
            };

            Comments.Add(newComment);
            NewCommentText = "";
            HasNoComments = false;
            OnPropertyChanged(nameof(Comments));
            OnPropertyChanged(nameof(TotalCommentsCount));
            OnPropertyChanged(nameof(OwnerCommentsCount));
            OnPropertyChanged(nameof(GuestCommentsCount));

            MessageBox.Show("Your comment has been posted successfully!", "Comment Posted",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public event Action OnBackToForumsRequested;

        private void ExecuteBackToForums(object parameter)
        {
            OnBackToForumsRequested?.Invoke();
        }
        private readonly HashSet<int> _reportedComments = new HashSet<int>();

        private void ExecuteReportComment(object parameter)
        {
            if (parameter is ForumCommentDTO comment)
            {
                if (comment.IsOwnerComment)
                {
                    MessageBox.Show("You cannot report comments from property owners.",
                        "Report Denied", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (_reportedComments.Contains(comment.CommentId))
                {
                    MessageBox.Show("You have already reported this comment.",
                        "Already Reported", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var result = MessageBox.Show($"Are you sure you want to report this comment?\n\n\"{comment.CommentText}\"",
                    "Report Comment", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _reportedComments.Add(comment.CommentId);

                    comment.ReportsCount++;
                    comment.HasReports = comment.ReportsCount > 0;

                    Comments.Remove(comment);
                    Comments.Insert(Comments.Count, comment);
                    MessageBox.Show("Comment has been reported. Thank you for helping maintain forum quality.",
                        "Report Submitted", MessageBoxButton.OK, MessageBoxImage.Information);
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