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
        private readonly ComplexTourRequestPartRepository _complexPartRepository;

        public GuideAvailabilityService()
        {
            _tourRepository = new TourRepository();
            _complexPartRepository = new ComplexTourRequestPartRepository();
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
                if(tour.Status != TourStatus.NONE )
                    continue;
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

        public bool IsComplexTourPartConflictFree(int complexTourRequestId, int currentPartId, DateTime start, DateTime end)
        {
            if (end <= start)
                return false;

            var allParts = _complexPartRepository.GetByComplexRequestId(complexTourRequestId);
            
            foreach (var part in allParts)
            {
                if (part.Id == currentPartId)
                    continue;

                if (part.Status == TourRequestStatus.ACCEPTED && part.ScheduledDate.HasValue)
                {
                    var partStart = part.ScheduledDate.Value;
                    var partEnd = partStart.AddHours(2); 
                    
                    if (start < partEnd && end > partStart)
                    {
                        return false; 
                    }
                }
            }

            return true; 
        }
    }
}