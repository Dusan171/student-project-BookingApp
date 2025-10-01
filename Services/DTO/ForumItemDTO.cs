using System;

namespace BookingApp.Services.DTO
{
    public class ForumItemDTO
    {
        public string Title { get; set; }
        public string Location { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int OwnerCommentsCount { get; set; }
        public int GuestCommentsCount { get; set; }

        public bool IsVeryUseful => OwnerCommentsCount >= 10 && GuestCommentsCount >= 20;
    }
}