using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Services
{
    public class OwnerForumService : IOwnerForumService
    {
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IForumManagementService _forumManagementService;
        private readonly IForumRepository _forumRepository;
        private readonly ILocationRepository _locationRepository;

        public OwnerForumService(
            IAccommodationRepository accommodationRepository,
            IForumManagementService forumManagementService,
            IForumRepository forumRepository,
            ILocationRepository locationRepository)
        {
            _accommodationRepository = accommodationRepository;
            _forumManagementService = forumManagementService;
            _forumRepository = forumRepository;
            _locationRepository = locationRepository;
        }

        public bool CanOwnerComment(int forumId, int ownerId)
        {
            var forum = _forumRepository.GetById(forumId);
            if (forum == null || forum.IsClosed)
                return false;
            var ownerAccommodations = _accommodationRepository.GetAll()
                .Where(a => a.OwnerId == ownerId)
                .ToList();
            return ownerAccommodations.Any(a => a.GeoLocation.Id == forum.LocationId);
        }

        public Comment AddOwnerComment(int forumId, int ownerId, string commentText)
        {
            if (string.IsNullOrWhiteSpace(commentText))
                throw new ArgumentException("Comment text cannot be empty");

            if (!CanOwnerComment(forumId, ownerId))
                throw new InvalidOperationException(
                    "You cannot comment on this forum. You need accommodation in this location.");

            return _forumManagementService.AddComment(forumId, commentText);
        }

        public List<int> GetForumsWhereOwnerCanComment(int ownerId)
        {
            var ownerAccommodations = _accommodationRepository.GetAll()
                .Where(a => a.OwnerId == ownerId)
                .ToList();

            var ownerLocationIds = ownerAccommodations
                .Select(a => a.GeoLocation.Id)
                .Distinct()
                .ToList();

            var forums = _forumRepository.GetAll()
                .Where(f => !f.IsClosed && ownerLocationIds.Contains(f.LocationId))
                .Select(f => f.Id)
                .ToList();

            return forums;
        }
    }
}