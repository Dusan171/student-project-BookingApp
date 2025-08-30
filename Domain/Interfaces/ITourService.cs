using BookingApp.Domain.Model;
using BookingApp.Services.DTO;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface ITourService
    {
        List<Tour> GetAvailableTours();
        List<Tour> SearchTours(SearchCriteriaDTO criteria);
        List<Tour> GetAlternativeTours(int originalTourId, int requiredSpots);
        Tour? GetTourById(int id);
        Tour? GetTourWithDetails(int id);
        bool ReserveSpots(int tourId, int numberOfSpots);
        int GetAvailableSpots(int tourId);
        void EnrichToursWithDetails(List<Tour> tours);
        bool ValidateTour(Tour tour);

        List<Tour> GetAllTours();
    }
}