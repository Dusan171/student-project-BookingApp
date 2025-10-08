using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using System.Collections.Generic;

namespace BookingApp.Services.Enhancement
{
    public class TourDataEnhancer
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IUserRepository _userRepository;

        public TourDataEnhancer(ILocationRepository locationRepository, IUserRepository userRepository)
        {
            _locationRepository = locationRepository;
            _userRepository = userRepository;
        }

        public void PopulateToursWithData(List<Tour> tours) => tours.ForEach(PopulateTourWithData);

        public void PopulateTourWithData(Tour tour)
        {
            if (tour.Location?.Id > 0) 
            {
                var fullLocation = _locationRepository.GetById(tour.Location.Id);
                if (fullLocation != null) tour.Location = fullLocation;
            }

            if (tour.Guide?.Id > 0) 
            {
                var fullGuide = _userRepository.GetById(tour.Guide.Id);
                if (fullGuide != null) tour.Guide = fullGuide;
            }
        }
    }
}