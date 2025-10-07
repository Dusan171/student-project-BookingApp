using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain.Model;
using BookingApp.Repositories;

namespace BookingApp.Services
{
    public class GuideAvailabilityService 
    {
        private readonly TourRepository _tourRepository;

        public GuideAvailabilityService()
        {
            _tourRepository = new TourRepository();
        }

        public bool IsAvailable(int guideId, DateTime start, DateTime end)
        {
            if (end <= start)
                return false;

            var guideTours = _tourRepository.GetAll()
                .Where(t => t.GuideId == guideId)
                .ToList();

            foreach (var tour in guideTours)
            {
                if (tour.StartTimes == null || !tour.StartTimes.Any())
                    continue;

                foreach (var startTime in tour.StartTimes)
                {
                    var tourStart = startTime.Time;
                    var tourEnd = tourStart.AddHours(tour.DurationHours);
                    
                    if (start < tourEnd && end > tourStart)
                    {
                        return false;
                    }
                }
            }

            return true; 
        }
    }
}