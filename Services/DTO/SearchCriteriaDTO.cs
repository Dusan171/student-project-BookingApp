using System.Collections.Generic;

namespace BookingApp.Services.DTO
{
    public class SearchCriteriaDTO
    {
        public string City { get; set; }
        public string Country { get; set; }
        public string Language { get; set; }
        public int? MaxPeople { get; set; }
        public double? Duration { get; set; }

        public bool HasAnyCriteria()
        {
            return !string.IsNullOrWhiteSpace(City) ||
                   !string.IsNullOrWhiteSpace(Country) ||
                   !string.IsNullOrWhiteSpace(Language) ||
                   MaxPeople.HasValue ||
                   Duration.HasValue;
        }

        public string GetSearchSummary()
        {
            var criteria = new List<string>();

            if (!string.IsNullOrWhiteSpace(City))
                criteria.Add($"Grad: {City}");

            if (!string.IsNullOrWhiteSpace(Country))
                criteria.Add($"Država: {Country}");

            if (!string.IsNullOrWhiteSpace(Language))
                criteria.Add($"Jezik: {Language}");

            if (MaxPeople.HasValue)
                criteria.Add($"Broj ljudi: {MaxPeople}");

            if (Duration.HasValue)
                criteria.Add($"Trajanje: {Duration}h");

            return criteria.Count > 0 ? string.Join(", ", criteria) : "Svi kriterijumi";
        }

        public override string ToString()
        {
            return GetSearchSummary();
        }
    }
}