using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;

namespace BookingApp.Services
{
    public class ForumService : IForumService
    {
        private readonly IForumRepository _forumRepository;
        private readonly IForumDisplayService _displayService;

        public ForumService(IForumRepository forumRepository, IForumDisplayService displayService)
        {
            _forumRepository = forumRepository;
            _displayService = displayService;
        }

        public List<ForumDTO> GetAll()
        {
            var forums = _forumRepository.GetAll();
            return forums.Select(f => _displayService.AssembleForumDTO(f)).ToList();
        }

        public ForumDTO GetById(int forumId)
        {

            var forum = _forumRepository.GetById(forumId);
            return forum != null ? _displayService.AssembleForumDTO(forum) : null;
        }

    }
}