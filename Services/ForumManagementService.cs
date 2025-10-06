using System;
using System.Linq;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Utilities;

namespace BookingApp.Services
{
    public class ForumManagementService : IForumManagementService
    {
        private readonly IForumRepository _forumRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IForumCommentRepository _forumCommentRepository;
        private readonly ILocationRepository _locationRepository;

        public ForumManagementService(IForumRepository forumRepository, ICommentRepository commentRepository,
                                      IForumCommentRepository forumCommentRepository, ILocationRepository locationRepository)
        {
            _forumRepository = forumRepository;
            _commentRepository = commentRepository;
            _forumCommentRepository = forumCommentRepository;
            _locationRepository = locationRepository;
        }
        public Forum Create(string title, string locationName, string firstCommentText)
        {
            var location = FindOrCreateLocation(locationName);
            var savedForum = CreateAndSaveForum(title, location.Id);

            AddComment(savedForum.Id, firstCommentText);

            return savedForum;
        }

        public Comment AddComment(int forumId, string commentText)
        {
            var savedComment = CreateAndSaveComment(commentText);
            LinkCommentToForum(forumId, savedComment.Id);
            return savedComment;
        }

        public void CloseForum(int forumId)
        {
            var forum = _forumRepository.GetById(forumId);
            if (CanUserCloseForum(forum))
            {
                forum.IsClosed = true;
                _forumRepository.Update(forum);
            }
        }

        #region Private Helper Methods

        private Forum CreateAndSaveForum(string title, int locationId)
        {
            var newForum = new Forum
            {
                Title = title,
                LocationId = locationId,
                CreatorId = Session.CurrentUser.Id,
                CreationDate = DateTime.Now,
                IsClosed = false
            };
            return _forumRepository.Save(newForum);
        }
        private Comment CreateAndSaveComment(string commentText)
        {
            var newComment = new Comment
            {
                User = Session.CurrentUser,
                Text = commentText,
                CreationTime = DateTime.Now
            };
            return _commentRepository.Save(newComment);
        }
        private void LinkCommentToForum(int forumId, int commentId)
        {
            var forumCommentLink = new ForumComment { ForumId = forumId, CommentId = commentId };
            _forumCommentRepository.Save(forumCommentLink);
        }
        private bool CanUserCloseForum(Forum forum)
        {
            return forum != null && forum.CreatorId == Session.CurrentUser.Id;
        }
        private Location FindOrCreateLocation(string locationName)
        {
            var locationParts = locationName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();
            string city = locationParts.FirstOrDefault() ?? "Unknown";
            string country = locationParts.Count > 1 ? locationParts[1] : "Unknown";

            var location = _locationRepository.GetAll().FirstOrDefault(l =>
                l.City.Equals(city, StringComparison.OrdinalIgnoreCase) &&
                l.Country.Equals(country, StringComparison.OrdinalIgnoreCase));

            if (location == null)
            {
                location = _locationRepository.Save(new Location { City = city, Country = country });
            }

            return location;
        }

        #endregion
    }
}