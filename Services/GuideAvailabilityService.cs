using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain.Model;
using BookingApp.Repositories;


namespace BookingApp.Services
{
    public class GuideAvailabilityService 
    {
        private readonly Dictionary<int, List<(DateTime start, DateTime end)>> _assignments = new Dictionary<int, List<(DateTime, DateTime)>>();
        private TourRepository _toursRepo = new TourRepository();


        public bool IsAvailable(int guideId, DateTime start, DateTime end)
        {
            if (!_assignments.ContainsKey(guideId)) return true;
            var assigned = _assignments[guideId];
            return !assigned.Any(a => start < a.end && end > a.start);
        }

        public void FillGuideWithTours(int id)
        {
            List<Tour> tours = _toursRepo.GetAll().Where(t => t.GuideId == id).ToList();
            foreach(var tour in tours)
            {
                AddTourToGuide(id, tour);
            }
        }

        public void AddTourToGuide(int guideId, Tour tour)
        {
            if (!_assignments.ContainsKey(guideId)) _assignments[guideId] = new List<(DateTime, DateTime)>();
            // For simplicity assume tour.StartTimes has one time and DurationHours set
            var start = tour.StartTimes.FirstOrDefault()?.Time ?? DateTime.Now;
            var end = start.AddHours(tour.DurationHours);
            _assignments[guideId].Add((start, end));
        }


        public void RemoveTourFromGuide(int guideId, int tourId)
        {
            // no-op for simple store
        }
    }
}