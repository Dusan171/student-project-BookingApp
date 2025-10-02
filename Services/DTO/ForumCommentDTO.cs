using System;

namespace BookingApp.Services.DTO
{
    public class ForumCommentDTO
    {
        public int CommentId { get; set; }
        public string AuthorName { get; set; }
        public bool IsOwnerComment { get; set; }
        public bool IsGuestComment { get; set; }
        public bool HasVerifiedStay { get; set; }
        public bool HasUnverifiedStay => IsGuestComment && !HasVerifiedStay;
        public string CommentText { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ReportsCount { get; set; }
        public bool HasReports { get; set; }
        public bool CanReport { get; set; }
        public string StayDatesText { get; set; }
    }
}