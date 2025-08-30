using BookingApp.Domain.Model;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface ITourRepository
    {
        List<Tour> GetAll();
        Tour GetById(int id);
        List<Tour> GetByLocation(string city, string country);
        List<Tour> GetByLanguage(string language);
        List<Tour> GetByMaxTourists(int maxTourists);
        List<Tour> GetAvailableTours();

        Tour Add(Tour tour);
        Tour Save(Tour tour);  
        Tour Update(Tour tour);
        void Delete(int id);
        void SaveAll();
        int GetNextId();

        bool UpdateReservedSpots(int tourId, int newReservedSpots);
        //List<Tour> SearchTours(string location, string country, string language, int maxPeople, double duration);
        List<Tour> SearchTours(string location, string country, string language, int? maxPeople, double? duration);
        bool ReserveSpots(int tourId, int numberOfSpots);
        List<Tour> GetAlternativeTours(int originalTourId, int requiredSpots);
    }
}