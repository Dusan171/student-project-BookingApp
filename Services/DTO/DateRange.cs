using System;

namespace BookingApp.Services.DTO
{
    public class DateRange
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public override string ToString()
        {
            return $"{StartDate:dd.MM.yyyy} - {EndDate:dd.MM.yyyy}";
        }
    }
}